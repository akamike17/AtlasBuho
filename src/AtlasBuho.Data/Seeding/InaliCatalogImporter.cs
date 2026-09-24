using System.IO;
using System.Text.Json;
using AtlasBuho.Data;
using AtlasBuho.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace AtlasBuho.Data.Seeding;

public interface ICatalogImporter
{
    Task<ImportResult> ImportInaliCatalogAsync(CancellationToken cancellationToken = default);
}

public class ImportResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int FamiliesImported { get; set; }
    public int GroupsImported { get; set; }
    public int VariantsImported { get; set; }
    public int VariantsQuarantined { get; set; }
    public int AutodenominationsImported { get; set; }
    public int AutodenominationsQuarantined { get; set; }
    // Artifacts are now pre-filtered in LoadSourceDataAsync, not tracked in result
    public int GroupsArtifactsQuarantined { get; set; }
    public List<string> ValidationErrors { get; set; } = new();
    public Dictionary<string, object> Provenance { get; set; } = new();
    // QuarantinedItems now persisted to DB via ImportQuarantine entity
}

public class InaliCatalogImporter : ICatalogImporter
{
    private readonly AtlasBuhoDbContext _context;
    private readonly ILogger<InaliCatalogImporter> _logger;

    // DOCUMENTARY BASELINES - from official sources, NOT from importer expectations
    private const int DOCUMENTARY_FAMILIES = 11;
    private const int DOCUMENTARY_GROUPS = 68;          // Apéndice 4 oficial (excluyendo artefactos)
    private const int DOCUMENTARY_VARIANTS = 364;       // PDF cuerpo principal oficial
    private const int DOCUMENTARY_AUTODENOMS_VALID = 474; // Apéndice 4: 478 filas totales - 1 header - 3 artefactos = 474 válidos

    // CLIN Compendium metadata
    private const string CATALOG_TITLE = "Catálogo de las Lenguas Indígenas Nacionales: Variantes Lingüísticas de México con sus autodenominaciones y referencias geoestadísticas";
    private const string CATALOG_URL = "https://www.inali.gob.mx/pdf/CLIN_completo.pdf";
    private const string CATALOG_SHA256 = "21cef44abf0d896555f26954bf319340ad4814fa8a3f0719b0d068311ff1be7c";
    private static readonly DateTime PUBLICATION_DATE = new(2008, 1, 14);
    private const string PARSER_VERSION = "2026.09.23-pass5-ledger-reconciliation"; // Parser version reflects this audit pass

    public InaliCatalogImporter(AtlasBuhoDbContext context, ILogger<InaliCatalogImporter> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ImportResult> ImportInaliCatalogAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting INALI catalog import with documentary baseline validation");

        var result = new ImportResult();
        var strategy = _context.Database.CreateExecutionStrategy();

        try
        {
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

                try
                {
                    // Step 1: Load and validate source data
                    var (catalogData, appendix4Data) = await LoadSourceDataAsync();
                    _logger.LogInformation("After LoadSourceDataAsync: catalogData.Count={Count}", catalogData.Count);

                    // 5B.md FASE 12: validate the documentary row budget BEFORE any persistence.
                    // If the source data is incomplete/corrupt there is nothing to roll back,
                    // so a failed import leaves the previous state provably untouched (FASE 14 G).
                    if (appendix4Data.Count != DOCUMENTARY_AUTODENOMS_VALID)
                    {
                        throw new InvalidOperationException(
                            $"DOCUMENTARY BASELINE VALIDATION FAILED:\nAutodenominations: expected {DOCUMENTARY_AUTODENOMS_VALID} " +
                            $"valid appendix rows in the source, got {appendix4Data.Count}. " +
                            $"Import aborted before persisting anything.");
                    }

                    // Step 2: Create or get SourceDocument for CLIN (REVALIDATE hash from file)
                    var sourceDocument = await CreateOrGetSourceDocumentAsync(cancellationToken);
                    result.Provenance["SourceDocumentId"] = sourceDocument.Id;

                    // Step 3: Create CatalogSource for INALI
                    var catalogSource = await CreateOrGetCatalogSourceAsync(cancellationToken);
                    result.Provenance["CatalogSourceId"] = catalogSource.Id;

                    // Step 4: Create CatalogVersion with PENDING status (or get existing for idempotent update)
                    var catalogVersion = await GetOrCreateCatalogVersionForUpdateAsync(catalogSource.Id, sourceDocument.Id, "Pending", cancellationToken);
                    result.Provenance["CatalogVersionId"] = catalogVersion.Id;

                    // Step 5: Import Families (11 expected)
                    var familyMap = await ImportFamiliesAsync(catalogData, catalogVersion.Id, sourceDocument.Id, cancellationToken);
                    result.FamiliesImported = familyMap.Count;
                    _logger.LogInformation("Imported {Count} families", familyMap.Count);

                    // Step 6: Import Groups/Agrupaciones (68 expected from Appendix 4)
                    var (groupMap, groupQuarantineCount) = await ImportGroupsAsync(catalogData, appendix4Data, familyMap, catalogVersion.Id, sourceDocument.Id, cancellationToken);
                    result.GroupsImported = groupMap.Count;
                    // Artifacts are pre-filtered, no artifact quarantine count
                    _logger.LogInformation("Imported {Count} groups, quarantined {QCount}", groupMap.Count, groupQuarantineCount);

                    // Step 7: Import Variants (364 expected) - NO silent continue
                    var (variantMap, variantQuarantineCount) = await ImportVariantsAsync(catalogData, appendix4Data, groupMap, catalogVersion.Id, sourceDocument.Id, cancellationToken);
                    result.VariantsImported = variantMap.Count;
                    result.VariantsQuarantined = variantQuarantineCount;
                    _logger.LogInformation("Imported {Count} variants, quarantined {QCount}", variantMap.Count, variantQuarantineCount);

                    // Step 8: Import Autodenominaciones (474 valid expected)
                    var (autodenomCount, autodenomQuarantineCount) = await ImportAutodenominationsAsync(appendix4Data, groupMap, variantMap, catalogVersion.Id, sourceDocument.Id, cancellationToken);
                    result.AutodenominationsImported = autodenomCount;
                    result.AutodenominationsQuarantined = autodenomQuarantineCount;
                    // Artifacts are now pre-filtered in LoadSourceDataAsync, tracked separately
                    _logger.LogInformation("Imported {Count} autodenominaciones, quarantined {QCount}", autodenomCount, autodenomQuarantineCount);

                    // Step 9: Validate documentary baseline counts - FAIL if mismatch
                    ValidateDocumentaryBaselines(result);

                    // Step 10: Create CatalogRecords for provenance tracking with REAL page numbers
                    await CreateCatalogRecordsAsync(catalogVersion.Id, familyMap, groupMap, variantMap, sourceDocument.Id, appendix4Data, cancellationToken);

                    // Step 11: Update CatalogVersion to COMPLETED with actual counts
                    await FinalizeCatalogVersionAsync(catalogVersion.Id, result, cancellationToken);

                    await transaction.CommitAsync(cancellationToken);

                    result.Success = true;
                    _logger.LogInformation("INALI catalog import completed successfully");
                    return result;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    _logger.LogError(ex, "Import failed, transaction rolled back");
                    result.Success = false;
                    result.ErrorMessage = ex.Message;
                    return result;
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Import execution strategy failed");
            result.Success = false;
            result.ErrorMessage = ex.Message;
            return result;
        }
    }

    private async Task<(List<CatalogVariantDto> catalogData, List<Appendix4Dto> appendix4Data)> LoadSourceDataAsync()
    {
        var basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
        var catalogPath = Path.Combine(basePath, "catalogs", "parsed", "inali_final_catalog.json");
        var appendix4Path = Path.Combine(basePath, "catalogs", "parsed", "appendix4_parsed.json");

        _logger.LogInformation("Loading catalog from: {Path}", catalogPath);
        _logger.LogInformation("Loading appendix4 from: {Path}", appendix4Path);

        var catalogJson = await File.ReadAllTextAsync(catalogPath);
        var appendix4Json = await File.ReadAllTextAsync(appendix4Path);

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var catalogRoot = JsonSerializer.Deserialize<CatalogRootDto>(catalogJson, options);

        _logger.LogInformation("Catalog root deserialized: Source={Source}, Families={Families}, VariantsCount={Count}",
            catalogRoot?.Source, catalogRoot?.Families, catalogRoot?.VariantsDetail?.Count ?? 0);

        var catalogData = catalogRoot?.VariantsDetail ?? new();

        // Debug: print first 3 items
        for (int i = 0; i < Math.Min(3, catalogData.Count); i++)
        {
            _logger.LogInformation("DEBUG Item {Index}: Page={Page}, Family={Family}, VariantName={VariantName}, Auto={Auto}, Geo={Geo}",
                i, catalogData[i].Page, catalogData[i].Family, catalogData[i].VariantName, catalogData[i].Autodenomination, catalogData[i].GeoReference);
        }

        var appendix4Data = JsonSerializer.Deserialize<List<Appendix4Dto>>(appendix4Json, options) ?? new();
        _logger.LogInformation("Appendix4 raw count: {Count}", appendix4Data.Count);

        // 5B.md FASE 2: assign deterministic source-row identity (ordinal in raw JSON)
        // BEFORE any filtering so every documentary row keeps a unique, reproducible identity.
        for (int i = 0; i < appendix4Data.Count; i++)
        {
            appendix4Data[i].SourceOrdinal = i;
        }

        // Filter out header rows from appendix4
        appendix4Data = appendix4Data.Where(a => !string.IsNullOrEmpty(a.Autodenom) &&
            a.Autodenom != "Autodenominación" &&
            a.Agrupacion != "Agrupación").ToList();

        // Separate artifacts from valid rows BEFORE passing to importers
        var artifactRows = appendix4Data.Where(a =>
            a.Agrupacion.Trim() == "Oto-mangue" ||
            a.Agrupacion.Trim() == "u" ||
            string.IsNullOrWhiteSpace(a.Familia)).ToList();

        // Use value-based filtering instead of Except (which uses reference equality for DTOs)
        var artifactKeys = new HashSet<string>(artifactRows.Select(a =>
            $"{a.Agrupacion}|{a.Familia}|{a.Autodenom}|{a.SpanishName}|{a.Page}"));

        var validAppendix4Rows = appendix4Data.Where(a =>
            !artifactKeys.Contains($"{a.Agrupacion}|{a.Familia}|{a.Autodenom}|{a.SpanishName}|{a.Page}")).ToList();

        _logger.LogInformation("Appendix4: {Total} total, {Artifacts} artifacts, {Valid} valid rows",
            appendix4Data.Count, artifactRows.Count, validAppendix4Rows.Count);

        return (catalogData, validAppendix4Rows);
    }

    private async Task<SourceDocument> CreateOrGetSourceDocumentAsync(CancellationToken cancellationToken)
    {
        var basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
        var pdfPath = Path.Combine(basePath, "catalogs", "raw", "inali", "CLIN_completo.pdf");

        // CRITICAL: Re-validate hash from ACTUAL FILE before trusting DB
        _logger.LogInformation("Re-validating PDF hash from file: {Path}", pdfPath);
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        using var stream = File.OpenRead(pdfPath);
        var computedHash = Convert.ToHexString(await sha256.ComputeHashAsync(stream)).ToLowerInvariant();

        if (computedHash != CATALOG_SHA256.ToLowerInvariant())
        {
            throw new InvalidOperationException($"PDF HASH MISMATCH: expected {CATALOG_SHA256}, got {computedHash}. File may have been modified.");
        }

        _logger.LogInformation("PDF hash validated: {Hash}", computedHash);

        var fileInfo = new FileInfo(pdfPath);

        var existing = await _context.SourceDocuments
            .FirstOrDefaultAsync(d => d.HashSha256 == CATALOG_SHA256, cancellationToken);

        if (existing != null)
        {
            _logger.LogInformation("SourceDocument already exists: {Id}", existing.Id);
            // Verify the stored hash still matches file
            if (existing.HashSha256 != CATALOG_SHA256)
            {
                throw new InvalidOperationException($"Stored SourceDocument hash mismatch: DB={existing.HashSha256}, expected={CATALOG_SHA256}");
            }
            return existing;
        }

        var doc = new SourceDocument(
            title: CATALOG_TITLE,
            url: CATALOG_URL,
            hashSha256: CATALOG_SHA256,
            contentType: "application/pdf",
            sizeBytes: fileInfo.Length,
            retrievedAt: DateTime.UtcNow, // This is the retrieval timestamp
            totalPages: 256,
            catalogRange: "30-212", // From JSON metadata - main catalog range
            parserVersion: PARSER_VERSION
        );

        _context.SourceDocuments.Add(doc);
        await _context.SaveChangesAsync(cancellationToken);

        // Create SourcePages for catalog pages WITH text extraction for RawTextHash
        var pages = new List<SourcePage>();
        _logger.LogInformation("Creating SourcePages with text extraction...");

        // Extract text and compute SHA256 for each page using PdfPig
        using (var pdf = PdfDocument.Open(pdfPath))
        {
            for (int i = 30; i <= 212; i++)
            {
                var pageNumber = i;
                if (pageNumber <= pdf.NumberOfPages)
                {
                    var page = pdf.GetPage(pageNumber);
                    var text = page.Text ?? string.Empty;
                    // Normalize: replace multiple whitespace with single space, trim
                    var normalizedText = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ").Trim();
                    var rawTextHash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(
                        System.Text.Encoding.UTF8.GetBytes(normalizedText))).ToLowerInvariant();

                    pages.Add(new SourcePage(
                        sourceDocumentId: doc.Id,
                        pageNumber: pageNumber,
                        sectionReference: "MainCatalog",
                        rawTextHash: rawTextHash
                    ));
                }
                else
                {
                    // Page doesn't exist in PDF - quarantine
                    _logger.LogWarning("Page {PageNumber} exceeds PDF page count ({PageCount}), creating quarantine marker", pageNumber, pdf.NumberOfPages);
                    pages.Add(new SourcePage(
                        sourceDocumentId: doc.Id,
                        pageNumber: pageNumber,
                        sectionReference: "MainCatalog",
                        rawTextHash: "PAGE_EXCEEDS_PDF"
                    ));
                }
            }
            // Pages 213-256: Appendices
            for (int i = 213; i <= 256; i++)
            {
                var pageNumber = i;
                if (pageNumber <= pdf.NumberOfPages)
                {
                    var page = pdf.GetPage(pageNumber);
                    var text = page.Text ?? string.Empty;
                    var normalizedText = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ").Trim();
                    var rawTextHash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(
                        System.Text.Encoding.UTF8.GetBytes(normalizedText))).ToLowerInvariant();

                    pages.Add(new SourcePage(
                        sourceDocumentId: doc.Id,
                        pageNumber: pageNumber,
                        sectionReference: "Appendices",
                        rawTextHash: rawTextHash
                    ));
                }
                else
                {
                    pages.Add(new SourcePage(
                        sourceDocumentId: doc.Id,
                        pageNumber: pageNumber,
                        sectionReference: "Appendices",
                        rawTextHash: "PAGE_EXCEEDS_PDF"
                    ));
                }
            }
        }

        _context.SourcePages.AddRange(pages);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created {Count} SourcePages with RawTextHash for document {DocId}", pages.Count, doc.Id);
        return doc;
    }

    private async Task<CatalogSource> CreateOrGetCatalogSourceAsync(CancellationToken cancellationToken)
    {
        var existing = await _context.CatalogSources
            .FirstOrDefaultAsync(cs => cs.Name == "INALI-CLIN-2008", cancellationToken);

        if (existing != null)
        {
            _logger.LogInformation("CatalogSource already exists: {Id}", existing.Id);
            return existing;
        }

        var source = new CatalogSource(
            name: "INALI-CLIN-2008",
            description: "Catálogo de las Lenguas Indígenas Nacionales (INALI) - Diario Oficial 2008-01-14",
            baseUrl: CATALOG_URL
        );

        _context.CatalogSources.Add(source);
        await _context.SaveChangesAsync(cancellationToken);

        return source;
    }

    private async Task<CatalogVersion> CreateCatalogVersionAsync(Guid catalogSourceId, Guid sourceDocumentId, string initialStatus, CancellationToken cancellationToken)
    {
        // Version number derived from: source document hash + parser version
        var versionIdentity = $"{CATALOG_SHA256.Substring(0, 12)}-{PARSER_VERSION}";
        var versionNumber = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(versionIdentity))).Substring(0, 12);

        // Check if this exact version already exists (idempotency)
        var existing = await _context.CatalogVersions
            .FirstOrDefaultAsync(cv => cv.CatalogSourceId == catalogSourceId && cv.VersionNumber == versionNumber, cancellationToken);

        if (existing != null)
        {
            // Version already exists - check if it's completed or failed
            if (existing.ImportStatus == "Completed")
            {
                _logger.LogInformation("CatalogVersion {Id} already exists with status Completed - returning existing", existing.Id);
                return existing;
            }
            else if (existing.ImportStatus == "Failed")
            {
                _logger.LogInformation("CatalogVersion {Id} exists with status Failed - will retry by updating status to Importing", existing.Id);
                existing.UpdateStatus("Importing");
                await _context.SaveChangesAsync(cancellationToken);
                return existing;
            }
            else
            {
                _logger.LogInformation("CatalogVersion {Id} exists with status {Status} - resuming", existing.Id, existing.ImportStatus);
                existing.UpdateStatus("Importing");
                await _context.SaveChangesAsync(cancellationToken);
                return existing;
            }
        }

        var version = new CatalogVersion(
            catalogSourceId: catalogSourceId,
            versionNumber: versionNumber,
            sourceDocumentHash: CATALOG_SHA256,
            retrievedAt: DateTime.UtcNow, // This is the import execution timestamp
            familyCount: 0, // Will be updated after import
            groupCount: 0,
            variantCount: 0,
            autodenominationCount: 0,
            parserVersion: PARSER_VERSION,
            importStatus: initialStatus // "Pending" or "Importing"
        );

        _context.CatalogVersions.Add(version);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created CatalogVersion {Id} with versionNumber={Version}, status={Status}", version.Id, versionNumber, initialStatus);
        return version;
    }

    private async Task<CatalogVersion> GetOrCreateCatalogVersionForUpdateAsync(Guid catalogSourceId, Guid sourceDocumentId, string initialStatus, CancellationToken cancellationToken)
    {
        // Version number derived from: source document hash + parser version
        var versionIdentity = $"{CATALOG_SHA256.Substring(0, 12)}-{PARSER_VERSION}";
        var versionNumber = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(versionIdentity))).Substring(0, 12);

        var existing = await _context.CatalogVersions
            .FirstOrDefaultAsync(cv => cv.CatalogSourceId == catalogSourceId && cv.VersionNumber == versionNumber, cancellationToken);

        if (existing != null)
        {
            // Always reset status to Importing for update - this allows full idempotent re-run
            _logger.LogInformation("CatalogVersion {Id} exists with status {Status} - resetting to Importing for idempotent re-run", existing.Id, existing.ImportStatus);
            existing.UpdateStatus("Importing");
            await _context.SaveChangesAsync(cancellationToken);
            return existing;
        }

        var version = new CatalogVersion(
            catalogSourceId: catalogSourceId,
            versionNumber: versionNumber,
            sourceDocumentHash: CATALOG_SHA256,
            retrievedAt: DateTime.UtcNow,
            familyCount: 0,
            groupCount: 0,
            variantCount: 0,
            autodenominationCount: 0,
            parserVersion: PARSER_VERSION,
            importStatus: initialStatus
        );

        _context.CatalogVersions.Add(version);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created CatalogVersion {Id} with versionNumber={Version}, status={Status}", version.Id, versionNumber, initialStatus);
        return version;
    }

    private async Task FinalizeCatalogVersionAsync(Guid catalogVersionId, ImportResult result, CancellationToken cancellationToken)
    {
        var version = await _context.CatalogVersions.FindAsync(new object[] { catalogVersionId }, cancellationToken);
        if (version != null)
        {
            // Use ACTUAL database counts for finalization (handles idempotent runs correctly)
            var actualFamilyCount = await _context.LanguageFamilies.CountAsync(cancellationToken);
            var actualGroupCount = await _context.LanguageGroups.CountAsync(cancellationToken);
            var actualVariantCount = await _context.LanguageVariants.CountAsync(cancellationToken);
            var actualAutodenominationCount = await _context.LanguageVariantAutodenominations.CountAsync(cancellationToken);

            version.UpdateCounts(
                actualFamilyCount,
                actualGroupCount,
                actualVariantCount,
                actualAutodenominationCount);
            version.UpdateStatus("Completed");
            version.SetValidationErrors(result.ValidationErrors.Count > 0 ? string.Join("; ", result.ValidationErrors) : string.Empty);

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Finalized CatalogVersion {Id} with actual counts: F={F} G={G} V={V} A={A}, status=Completed",
                catalogVersionId, actualFamilyCount, actualGroupCount, actualVariantCount, actualAutodenominationCount);
        }
    }

    private async Task<Dictionary<string, LanguageFamily>> ImportFamiliesAsync(
        List<CatalogVariantDto> catalogData,
        Guid catalogVersionId,
        Guid sourceDocumentId,
        CancellationToken cancellationToken)
    {
        var familyMap = new Dictionary<string, LanguageFamily>(StringComparer.OrdinalIgnoreCase);
        var familiesSeen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in catalogData)
        {
            if (string.IsNullOrWhiteSpace(item.Family)) continue;
            familiesSeen.Add(item.Family.Trim());
        }

        _logger.LogInformation("Found {Count} unique families in source data", familiesSeen.Count);

        foreach (var familyName in familiesSeen.OrderBy(f => f))
        {
            var existing = await _context.LanguageFamilies
                .FirstOrDefaultAsync(f => f.Name.ToLower() == familyName.ToLower(), cancellationToken);

            LanguageFamily family;
            if (existing != null)
            {
                family = existing;
                _logger.LogDebug("Family already exists: {Name}", familyName);
            }
            else
            {
                family = new LanguageFamily(
                    name: familyName,
                    nameEnglish: null,
                    description: null,
                    source: CATALOG_TITLE
                );
                _context.LanguageFamilies.Add(family);
            }

            await _context.SaveChangesAsync(cancellationToken);
            familyMap[familyName] = family;
        }

        return familyMap;
    }

    private async Task PersistQuarantineAsync(ImportQuarantine quarantine, CancellationToken cancellationToken)
    {
        // Idempotency: Check if quarantine already exists with same identity
        // Identity = CatalogVersionId + SourceDocumentId + EntityType + SourcePage + ResolutionMethod + RawDataHash
        var existing = await _context.ImportQuarantines
            .FirstOrDefaultAsync(q =>
                q.CatalogVersionId == quarantine.CatalogVersionId &&
                q.SourceDocumentId == quarantine.SourceDocumentId &&
                q.EntityType == quarantine.EntityType &&
                q.SourcePage == quarantine.SourcePage &&
                q.ResolutionMethod == quarantine.ResolutionMethod &&
                q.RawDataHash == quarantine.RawDataHash, cancellationToken);

        if (existing == null)
        {
            _context.ImportQuarantines.Add(quarantine);
            await _context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            _logger.LogDebug("Quarantine already exists, skipping duplicate: {EntityType} Page={Page} Method={Method}",
                quarantine.EntityType, quarantine.SourcePage, quarantine.ResolutionMethod);
        }
    }

    // 5B.md FASE 2/3: Compute deterministic hash for a source appendix row (for reconciliation identity)
    private static string ComputeRawDataHash(Appendix4Dto item)
    {
        var normalized = $"{item.Agrupacion.Trim()}|{item.Familia.Trim()}|{item.Autodenom.Trim()}|{item.SpanishName.Trim()}|{item.Page}";
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(normalized));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private async Task<(Dictionary<string, LanguageGroup> groupMap, int quarantineCount)> ImportGroupsAsync(
        List<CatalogVariantDto> catalogData,
        List<Appendix4Dto> appendix4Data,
        Dictionary<string, LanguageFamily> familyMap,
        Guid catalogVersionId,
        Guid sourceDocumentId,
        CancellationToken cancellationToken)
    {
        var groupMap = new Dictionary<string, LanguageGroup>(StringComparer.OrdinalIgnoreCase);
        int quarantineCount = 0;

        // Use appendix 4 as the canonical source for agrupación names
        var agrupacionToFamilia = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var agrupacionToPage = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in appendix4Data)
        {
            if (!string.IsNullOrWhiteSpace(item.Agrupacion) && !string.IsNullOrWhiteSpace(item.Familia))
            {
                var agrupacion = item.Agrupacion.Trim();
                var familia = item.Familia.Trim();

                if (!agrupacionToFamilia.ContainsKey(agrupacion))
                {
                    agrupacionToFamilia[agrupacion] = familia;
                    agrupacionToPage[agrupacion] = item.Page;
                }
            }
        }

        _logger.LogInformation("Built agrupación map with {Count} unique agrupaciones (excl. artifacts)", agrupacionToFamilia.Count);

        var groupKeyToFamily = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var groupKeyToPage = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var kvp in agrupacionToFamilia)
        {
            var agrupacion = kvp.Key;
            var familia = kvp.Value;
            var page = agrupacionToPage[agrupacion]; // Direct lookup - no fallback

            var key = $"{familia}|{agrupacion}";

            if (!groupKeyToFamily.ContainsKey(key))
            {
                groupKeyToFamily[key] = familia;
                groupKeyToPage[key] = page;
            }
        }

        _logger.LogInformation("Found {Count} unique groups from Appendix 4", groupKeyToFamily.Count);

        foreach (var kvp in groupKeyToFamily.OrderBy(k => k.Key))
        {
            var groupName = kvp.Key.Split('|')[1];
            var familyName = kvp.Value;
            var page = groupKeyToPage[kvp.Key];

            if (!familyMap.TryGetValue(familyName, out var family))
            {
                _logger.LogWarning("Family not found for group {Group}: {Family}", groupName, familyName);
                await PersistQuarantineAsync(new ImportQuarantine(
                    catalogVersionId,
                    sourceDocumentId,
                    "LanguageGroup",
                    $"Family={familyName}, Group={groupName}",
                    null,
                    page,
                    "Appendix4",
                    $"Family '{familyName}' not found in familyMap",
                    "family_missing"
                ), cancellationToken);
                quarantineCount++;
                continue;
            }

            var existing = await _context.LanguageGroups
                .FirstOrDefaultAsync(g => g.LanguageFamilyId == family.Id && g.Name.ToLower() == groupName.ToLower(), cancellationToken);

            LanguageGroup group;
            if (existing != null)
            {
                group = existing;
            }
            else
            {
                group = new LanguageGroup(
                    languageFamilyId: family.Id,
                    name: groupName,
                    nameEnglish: null,
                    description: null,
                    source: CATALOG_TITLE
                );
                _context.LanguageGroups.Add(group);
            }

            await _context.SaveChangesAsync(cancellationToken);
            groupMap[kvp.Key] = group;
        }

        // Verify documentary baseline
        if (groupMap.Count != DOCUMENTARY_GROUPS)
        {
            _logger.LogWarning("Group count mismatch: expected {Expected}, got {Actual}", DOCUMENTARY_GROUPS, groupMap.Count);
        }

        return (groupMap, quarantineCount);
    }

    private async Task<(Dictionary<string, LanguageVariant> variantMap, int quarantineCount)> ImportVariantsAsync(
        List<CatalogVariantDto> catalogData,
        List<Appendix4Dto> appendix4Data,
        Dictionary<string, LanguageGroup> groupMap,
        Guid catalogVersionId,
        Guid sourceDocumentId,
        CancellationToken cancellationToken)
    {
        var variantMap = new Dictionary<string, LanguageVariant>(StringComparer.OrdinalIgnoreCase);
        int quarantineCount = 0;

        // Build mapping from Spanish variant name -> appendix 4 agrupación
        var spanishNameToAgrupacion = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in appendix4Data)
        {
            if (!string.IsNullOrWhiteSpace(item.SpanishName) && !string.IsNullOrWhiteSpace(item.Agrupacion))
            {
                var spanishName = item.SpanishName.Trim();
                var agrupacion = item.Agrupacion.Trim();

                if (!spanishNameToAgrupacion.ContainsKey(spanishName))
                {
                    spanishNameToAgrupacion[spanishName] = agrupacion;
                }
            }
        }

        _logger.LogInformation("Built SpanishName->Agrupacion map with {Count} entries", spanishNameToAgrupacion.Count);

        foreach (var item in catalogData)
        {
            if (string.IsNullOrWhiteSpace(item.Family) || string.IsNullOrWhiteSpace(item.VariantName)) continue;

            // Normalize variant name for matching (remove newlines from PDF extraction artifacts)
            var normalizedVariantName = item.VariantName.Replace("\n", " ").Replace("\r", " ").Trim();

            // Determine group: appendix 4 match OR derivation with tracking
            string groupName;
            string resolutionMethod;

            // Try exact match first
            if (spanishNameToAgrupacion.TryGetValue(normalizedVariantName, out var foundAgrupacion))
            {
                groupName = foundAgrupacion;
                resolutionMethod = "appendix4_exact_match";
            }
            else
            {
                // Try prefix matching: appendix SpanishName is prefix of catalog variant name
                var prefixMatches = spanishNameToAgrupacion
                    .Where(kvp => normalizedVariantName.StartsWith(kvp.Key, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (prefixMatches.Count == 1)
                {
                    groupName = prefixMatches[0].Value;
                    resolutionMethod = "appendix4_prefix_unique";
                }
                else if (prefixMatches.Count > 1)
                {
                    // Multiple prefix matches - ambiguity must be quarantined (H-103)
                    _logger.LogWarning("Ambiguous prefix match for variant '{VariantName}': {Count} candidates. Quarantining.",
                        normalizedVariantName, prefixMatches.Count);
                    await PersistQuarantineAsync(new ImportQuarantine(
                        catalogVersionId,
                        sourceDocumentId,
                        "LanguageVariant",
                        JsonSerializer.Serialize(item),
                        null,
                        item.Page,
                        "MainCatalog",
                        $"Ambiguous prefix match for SpanishName '{normalizedVariantName}' - {prefixMatches.Count} candidates: {string.Join(", ", prefixMatches.Select(p => p.Key))}",
                        "appendix4_prefix_ambiguous"
                    ), cancellationToken);
                    quarantineCount++;
                    continue;
                }
                else
                {
                    // No appendix match - use derivation
                    groupName = DeriveGroupName(item);
                    resolutionMethod = "derived_fallback";
                }
            }

            var groupKey = $"{item.Family.Trim()}|{groupName}";

            if (!groupMap.TryGetValue(groupKey, out var group))
            {
                _logger.LogWarning("Group not found for variant: {Key} (Variant={Variant}, Family={Family}, DerivedGroup={Group}, Method={Method})",
                    groupKey, item.VariantName, item.Family, groupName, resolutionMethod);

                await PersistQuarantineAsync(new ImportQuarantine(
                    catalogVersionId,
                    sourceDocumentId,
                    "LanguageVariant",
                    JsonSerializer.Serialize(item),
                    null,
                    item.Page,
                    "MainCatalog",
                    $"Group '{groupKey}' not found in groupMap (method: {resolutionMethod})",
                    resolutionMethod
                ), cancellationToken);
                quarantineCount++;
                continue; // This variant is quarantined, not silently dropped
            }

            // IDEMPOTENCY: Check if variant already exists by identity
            var existingVariant = await _context.LanguageVariants
                .FirstOrDefaultAsync(v => v.LanguageGroupId == group.Id && v.Name.ToLower() == normalizedVariantName.ToLower(), cancellationToken);

            LanguageVariant variant;
            if (existingVariant != null)
            {
                variant = existingVariant;
            }
            else
            {
                variant = new LanguageVariant(
                    languageGroupId: group.Id,
                    name: normalizedVariantName,
                    autodenomination: string.IsNullOrWhiteSpace(item.Autodenomination) ? null : item.Autodenomination.Trim(),
                    iso639_3Code: null,
                    inaliCode: null,
                    description: null,
                    writingSystem: null,
                    orthography: null,
                    source: CATALOG_TITLE
                );
                _context.LanguageVariants.Add(variant);
            }

            await _context.SaveChangesAsync(cancellationToken);

            var variantKey = $"{groupKey}|{normalizedVariantName}";
            variantMap[variantKey] = variant;
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Imported {Count} variants, quarantined {QCount}", variantMap.Count, quarantineCount);

        return (variantMap, quarantineCount);
    }

    private async Task<(int imported, int quarantineCount)> ImportAutodenominationsAsync(
        List<Appendix4Dto> appendix4Data,
        Dictionary<string, LanguageGroup> groupMap,
        Dictionary<string, LanguageVariant> variantMap,
        Guid catalogVersionId,
        Guid sourceDocumentId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("ImportAutodenominationsAsync received {Count} appendix4 rows", appendix4Data.Count);

        var sourceDoc = await _context.SourceDocuments.FirstAsync(d => d.HashSha256 == CATALOG_SHA256, cancellationToken);

        // 5B.md FASE 3 + FASE 7: the per-row reconciliation ledger is the SINGLE accounting
        // source of truth. The imported/quarantined counters are DERIVED from it at the end;
        // they are never mutated independently inside a branch.
        var reconciliationLedger = new List<AutodenominationReconciliationEntry>();
        var accountedOrdinals = new HashSet<int>();

        // Identity = Variant + Autodenom + SourcePage (H-015)
        var addedTriplets = new HashSet<string>();

        // 5B.md FASE 3: one source row -> exactly ONE terminal outcome.
        // Enforced at insertion so a second accounting event for the same source row is
        // impossible by construction (not hidden afterwards with Distinct()/GroupBy()).
        AutodenominationReconciliationEntry RecordOutcome(
            Appendix4Dto item,
            string outcome,
            string resolutionMethod,
            string reason,
            Guid? catalogRecordId)
        {
            if (!accountedOrdinals.Add(item.SourceOrdinal))
            {
                throw new InvalidOperationException(
                    $"ACCOUNTING DUPLICATE: source ordinal {item.SourceOrdinal} (source page {item.Page}, " +
                    $"autodenominación '{item.Autodenom}') produced more than one terminal outcome.");
            }

            var entry = new AutodenominationReconciliationEntry
            {
                SourceOrdinal = item.SourceOrdinal,
                SourcePage = item.Page,
                SourceDocumentId = sourceDoc.Id,
                RawDataHash = ComputeRawDataHash(item),
                SpanishName = item.SpanishName?.Trim() ?? "",
                IndigenousName = item.Autodenom?.Trim() ?? "",
                Grouping = item.Agrupacion?.Trim() ?? "",
                Familia = item.Familia?.Trim() ?? "",
                TerminalOutcome = outcome,
                CatalogRecordId = catalogRecordId,
                ResolutionMethod = resolutionMethod,
                Reason = reason
            };

            reconciliationLedger.Add(entry);
            return entry;
        }

        async Task QuarantineRowAsync(Appendix4Dto item, string resolutionMethod, string reason)
        {
            await PersistQuarantineAsync(new ImportQuarantine(
                catalogVersionId,
                sourceDocumentId,
                "LanguageVariantAutodenomination",
                JsonSerializer.Serialize(item),
                null,
                item.Page,
                "Appendix4",
                reason,
                resolutionMethod
            ), cancellationToken);

            RecordOutcome(item, "QUARANTINED", resolutionMethod, reason, null);
        }

        foreach (var item in appendix4Data)
        {
            _logger.LogDebug("Processing appendix row Ordinal={Ordinal}: Autodenom={Autodenom}, Agrupacion={Agrupacion}, Familia={Familia}, SpanishName={SpanishName}, Page={Page}",
                item.SourceOrdinal, item.Autodenom, item.Agrupacion, item.Familia, item.SpanishName, item.Page);

            // A row without usable source identity cannot be imported, but it is never silently
            // discarded: it becomes an explicit, persisted quarantine outcome.
            if (string.IsNullOrWhiteSpace(item.Autodenom) || string.IsNullOrWhiteSpace(item.Agrupacion))
            {
                await QuarantineRowAsync(item, "empty_source_fields",
                    "Source row has an empty autodenominación or agrupación");
                continue;
            }

            // Artifacts are pre-filtered in LoadSourceDataAsync; this is the explicit safety net.
            if (item.Agrupacion.Trim() == "Oto-mangue" || item.Agrupacion.Trim() == "u" || string.IsNullOrWhiteSpace(item.Familia))
            {
                await QuarantineRowAsync(item, "classified_as_artifact",
                    "Artifact row (empty familia or artifact agrupación) - should have been pre-filtered");
                continue;
            }

            var groupKey = groupMap.Keys.FirstOrDefault(k =>
                k.EndsWith($"|{item.Agrupacion.Trim()}", StringComparison.OrdinalIgnoreCase));

            if (groupKey == null)
            {
                await QuarantineRowAsync(item, "group_not_found",
                    $"Group '{item.Agrupacion}' not found in groupMap");
                continue;
            }

            // Find matching variant by searching ALL variants in this family, because the
            // Appendix 4 agrupación does not always align one-to-one with the catalog groups.
            var family = await _context.LanguageFamilies
                .FirstOrDefaultAsync(f => f.Name.ToLower() == item.Familia.Trim().ToLower(), cancellationToken);

            var groupIdsInFamily = family == null
                ? new List<Guid>()
                : await _context.LanguageGroups
                    .Where(g => g.LanguageFamilyId == family.Id)
                    .Select(g => g.Id)
                    .ToListAsync(cancellationToken);

            var allVariantsInFamily = await _context.LanguageVariants
                .Where(v => groupIdsInFamily.Contains(v.LanguageGroupId))
                .ToListAsync(cancellationToken);

            _logger.LogDebug("Autodenom matching: Ordinal={Ordinal}, Agrupacion={Agrupacion}, Familia={Familia}, FamilyVariants={FV}",
                item.SourceOrdinal, item.Agrupacion, item.Familia, allVariantsInFamily.Count);

            // H-103: pure resolution - an ambiguous candidate set is NEVER resolved by picking one.
            var resolution = ResolveVariantForAutodenomination(item, allVariantsInFamily);

            if (resolution.VariantId == null || resolution.VariantId == Guid.Empty)
            {
                await QuarantineRowAsync(item, resolution.FailureMethod, resolution.FailureReason);
                continue;
            }

            var variantId = resolution.VariantId;
            var matchMethod = resolution.Method;

            // Identity key includes SourcePage for proper uniqueness (H-015)
            var autodenomTriplet = $"{variantId}|{item.Autodenom.Trim()}|{item.Page}";

            if (addedTriplets.Contains(autodenomTriplet))
            {
                await QuarantineRowAsync(item, "duplicate_triplet_in_batch",
                    "Duplicate autodenomination triplet in batch (same variant, autodenom, page)");
                continue;
            }

            // Idempotency: existing row in the database or still-uncommitted in the change tracker.
            var existsInDb = await _context.LanguageVariantAutodenominations
                .AnyAsync(a => a.LanguageVariantId == variantId && a.Autodenomination == item.Autodenom.Trim() && a.SourcePage == item.Page, cancellationToken);

            var existsInTracker = _context.ChangeTracker.Entries<LanguageVariantAutodenomination>()
                .Any(e => e.Entity.LanguageVariantId == variantId && e.Entity.Autodenomination == item.Autodenom.Trim() && e.Entity.SourcePage == item.Page && e.State != EntityState.Detached);

            if (existsInDb || existsInTracker)
            {
                // Idempotency (H-111): the exact (variant, autodenominación, source page) identity is
                // already persisted for this source document + parser version. The source row IS
                // imported - it is NOT quarantined - and neither a new record nor a new quarantine
                // row is created, so a re-import leaves the semantic dataset untouched.
                addedTriplets.Add(autodenomTriplet);
                RecordOutcome(item, "IMPORTED", "idempotent_existing_record",
                    "Already persisted for this source document and parser version", null);
                continue;
            }

            addedTriplets.Add(autodenomTriplet);

            _logger.LogDebug("Adding autodenomination: VariantId={VariantId}, Autodenom='{Autodenom}', SpanishName={SpanishName}, Agrupacion={Agrupacion}, Page={Page}, Method={Method}",
                variantId, item.Autodenom, item.SpanishName, item.Agrupacion, item.Page, matchMethod);

            var autodenom = new LanguageVariantAutodenomination(
                languageVariantId: variantId.Value,
                autodenomination: item.Autodenom.Trim(),
                spanishName: item.SpanishName.Trim(),
                agrupacion: item.Agrupacion.Trim(),
                familia: item.Familia.Trim(),
                sourcePage: item.Page,
                sourceSection: "Appendix4",
                sourceDocumentId: sourceDoc.Id,
                sourceHash: CATALOG_SHA256,
                parserVersion: PARSER_VERSION
            );

            _context.LanguageVariantAutodenominations.Add(autodenom);
            RecordOutcome(item, "IMPORTED", matchMethod, "", autodenom.Id);
        }

        await _context.SaveChangesAsync(cancellationToken);

        // 5B.md FASE 7: counters are derived from the ledger, never mutated in branches.
        var importedCount = reconciliationLedger.Count(e => e.TerminalOutcome == "IMPORTED");
        var quarantinedCount = reconciliationLedger.Count(e => e.TerminalOutcome == "QUARANTINED");
        var nonTerminalCount = reconciliationLedger.Count - importedCount - quarantinedCount;

        _logger.LogInformation("=== RECONCILIATION LEDGER ===");
        _logger.LogInformation("Source rows received: {Received} | Ledger entries: {Ledger} | Imported: {Imp} | Quarantined: {Q} | Non-terminal: {Other}",
            appendix4Data.Count, reconciliationLedger.Count, importedCount, quarantinedCount, nonTerminalCount);

        foreach (var e in reconciliationLedger.OrderBy(x => x.SourceOrdinal))
        {
            _logger.LogInformation("  Ordinal={Ordinal} Page={Page} Hash={Hash} Outcome={Outcome} Method={Method} Reason={Reason}",
                e.SourceOrdinal, e.SourcePage, e.RawDataHash.Length >= 12 ? e.RawDataHash.Substring(0, 12) : e.RawDataHash,
                e.TerminalOutcome, e.ResolutionMethod, e.Reason);
        }

        // 5B.md FASE 10: documentary source rows == ledger entries == imported + quarantined.
        if (reconciliationLedger.Count != appendix4Data.Count || nonTerminalCount != 0)
        {
            throw new InvalidOperationException(
                $"ACCOUNTING INVARIANT VIOLATED: received {appendix4Data.Count} source rows but produced " +
                $"{reconciliationLedger.Count} ledger entries (imported={importedCount}, quarantined={quarantinedCount}, " +
                $"non-terminal={nonTerminalCount}). Every source row must have exactly one terminal outcome.");
        }

        return (importedCount, quarantinedCount);
    }

    // 5B.md FASE 7: pure, side-effect-free variant resolution.
    // It never persists and never silently selects among materially different candidates:
    // on ambiguity it returns a null VariantId together with the resolution method and the
    // reason the caller must quarantine with (H-103).
    internal static VariantResolution ResolveVariantForAutodenomination(Appendix4Dto item, List<LanguageVariant> allVariantsInFamily)
    {
        if (string.IsNullOrWhiteSpace(item.SpanishName) || item.SpanishName.Trim() == "Nombre en español")
        {
            var rawSpanishName = item.SpanishName ?? "";
            return VariantResolution.Unresolved("no_spanish_name",
                $"Source row has no usable SpanishName (raw value '{rawSpanishName}')");
        }

        var spanishName = item.SpanishName.Trim();

        // 1. Exact match.
        var exactMatches = allVariantsInFamily
            .Where(v => v.Name.Equals(spanishName, StringComparison.OrdinalIgnoreCase))
            .ToList();
        if (exactMatches.Count == 1)
            return VariantResolution.Resolved(exactMatches[0].Id, "spanish_name_exact");
        if (exactMatches.Count > 1)
            return VariantResolution.Unresolved("spanish_name_exact_ambiguous",
                $"Exact SpanishName '{spanishName}' matches {exactMatches.Count} variants in the family");

        // 2. Generic group name (ends with comma): only an autodenomination location qualifier may resolve it.
        if (spanishName.EndsWith(","))
        {
            var disambiguated = TryDisambiguateByAutodenomLocation(item.Autodenom, allVariantsInFamily, out var genericMethod);
            if (disambiguated != null)
                return VariantResolution.Resolved(disambiguated.Value, genericMethod);
            return VariantResolution.Unresolved("generic_group_name_no_location",
                $"Generic group name '{spanishName}' cannot be mapped to a specific variant (autodenom lacks a usable location qualifier)");
        }

        // 3. Unique prefix match.
        var prefixMatches = allVariantsInFamily
            .Where(v => v.Name.StartsWith(spanishName, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (prefixMatches.Count == 1)
            return VariantResolution.Resolved(prefixMatches[0].Id, "spanish_name_prefix_unique");

        if (prefixMatches.Count > 1)
        {
            // The prefix may continue with a space or a comma in exactly one candidate.
            var continuationMatches = prefixMatches
                .Where(v => v.Name.Length > spanishName.Length &&
                            (v.Name[spanishName.Length] == ' ' || v.Name[spanishName.Length] == ','))
                .ToList();
            if (continuationMatches.Count == 1)
                return VariantResolution.Resolved(continuationMatches[0].Id, "spanish_name_prefix_best");

            var disambiguated = TryDisambiguateByAutodenomLocation(item.Autodenom, prefixMatches, out var prefixMethod);
            if (disambiguated != null)
                return VariantResolution.Resolved(disambiguated.Value, prefixMethod);

            return VariantResolution.Unresolved("spanish_name_prefix_ambiguous",
                $"Ambiguous prefix match for SpanishName '{spanishName}' - {prefixMatches.Count} candidates, no disambiguation");
        }

        // 4. Progressive prefix matching over the comma-separated components.
        var snParts = spanishName.Split(',').Select(p => p.Trim()).ToArray();
        Guid? progressiveMatch = null;
        var progressiveMethod = "";
        var progressiveFailureMethod = "";
        var progressiveFailureReason = "";
        for (int i = snParts.Length - 1; i >= 1; i--)
        {
            var shorterPrefix = string.Join(", ", snParts.Take(i));
            var shorterMatches = allVariantsInFamily
                .Where(v => v.Name.StartsWith(shorterPrefix, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (shorterMatches.Count == 1)
            {
                progressiveMatch = shorterMatches[0].Id;
                progressiveMethod = "spanish_name_progressive_prefix_unique";
                break;
            }

            if (shorterMatches.Count > 1)
            {
                var disambiguated = TryDisambiguateByAutodenomLocation(item.Autodenom, shorterMatches, out var progressiveDisambiguationMethod);
                if (disambiguated != null)
                {
                    progressiveMatch = disambiguated.Value;
                    progressiveMethod = progressiveDisambiguationMethod;
                    break;
                }

                // Ambiguity at this level is terminal for the row: stop shortening. The original
                // implementation kept iterating, which produced a second terminal accounting
                // event for the same source row (the 474 -> 484 defect).
                progressiveFailureMethod = "spanish_name_progressive_prefix_ambiguous";
                progressiveFailureReason = $"Ambiguous progressive prefix match for SpanishName '{spanishName}' at prefix '{shorterPrefix}' - {shorterMatches.Count} candidates, no disambiguation";
                break;
            }
        }

        if (progressiveMatch != null)
            return VariantResolution.Resolved(progressiveMatch.Value, progressiveMethod);
        if (progressiveFailureMethod.Length > 0)
            return VariantResolution.Unresolved(progressiveFailureMethod, progressiveFailureReason);

        // 5. Suffix matching: the variant name is a prefix of the appendix name.
        var suffixMatches = allVariantsInFamily
            .Where(v => spanishName.StartsWith(v.Name, StringComparison.OrdinalIgnoreCase) &&
                        (spanishName.Length == v.Name.Length || spanishName[v.Name.Length] == ',' || spanishName[v.Name.Length] == ' '))
            .ToList();

        if (suffixMatches.Count == 1)
            return VariantResolution.Resolved(suffixMatches[0].Id, "spanish_name_suffix_unique");

        if (suffixMatches.Count > 1)
        {
            var disambiguated = TryDisambiguateByAutodenomLocation(item.Autodenom, suffixMatches, out _);
            if (disambiguated != null)
                return VariantResolution.Resolved(disambiguated.Value, "spanish_name_suffix_disambiguated");
            return VariantResolution.Unresolved("spanish_name_suffix_ambiguous",
                $"Ambiguous suffix match for SpanishName '{spanishName}' - {suffixMatches.Count} candidates, no disambiguation");
        }

        // 6. Last resort: autodenomination-based disambiguation across the whole family.
        var byAutodenom = TryDisambiguateByAutodenomLocation(item.Autodenom, allVariantsInFamily, out var autodenomMethod);
        if (byAutodenom != null)
            return VariantResolution.Resolved(byAutodenom.Value, autodenomMethod);

        return VariantResolution.Unresolved("no_match",
            $"No variant matches SpanishName '{spanishName}' in family '{item.Familia}' ({allVariantsInFamily.Count} family variants)");
    }

    private void ValidateDocumentaryBaselines(ImportResult result)
    {
        var errors = new List<string>();

        if (result.FamiliesImported != DOCUMENTARY_FAMILIES)
            errors.Add($"Families: expected {DOCUMENTARY_FAMILIES} (documentary), got {result.FamiliesImported}");

        if (result.GroupsImported != DOCUMENTARY_GROUPS)
            errors.Add($"Groups: expected {DOCUMENTARY_GROUPS} (documentary - Appendix 4), got {result.GroupsImported}");

        // Variants must be fully accounted for (imported + quarantined)
        var variantsAccounted = result.VariantsImported + result.VariantsQuarantined;
        if (variantsAccounted != DOCUMENTARY_VARIANTS)
            errors.Add($"Variants: expected {DOCUMENTARY_VARIANTS} accounted (imported + quarantined), got imported={result.VariantsImported} + quarantined={result.VariantsQuarantined} = {variantsAccounted}");

        // Autodenominations: all 474 valid appendix rows must be accounted for
        // (imported + quarantined). Artifacts are now pre-filtered, so all quarantine is valid rows.
        var validRowsQuarantined = result.AutodenominationsQuarantined;
        var autodenomsAccounted = result.AutodenominationsImported + validRowsQuarantined;

        _logger.LogInformation("VALIDATION DEBUG: AutodenominationsImported={Imported}, AutodenominationsQuarantined={Quarantined}, Total={Total}, Expected={Expected}",
            result.AutodenominationsImported, result.AutodenominationsQuarantined, autodenomsAccounted, DOCUMENTARY_AUTODENOMS_VALID);

        if (autodenomsAccounted != DOCUMENTARY_AUTODENOMS_VALID)
            errors.Add($"Autodenominations: expected {DOCUMENTARY_AUTODENOMS_VALID} valid rows accounted (imported + quarantined), got imported={result.AutodenominationsImported} + quarantined_valid={validRowsQuarantined} = {autodenomsAccounted}");

        if (errors.Any())
        {
            result.ValidationErrors = errors;
            throw new InvalidOperationException($"DOCUMENTARY BASELINE VALIDATION FAILED:\n{string.Join("\n", errors)}");
        }

        _logger.LogInformation("Documentary baseline validation PASSED: {Families}/{Groups}/{Variants}/{Autodenoms} (quarantined V={VQ} A={AQ})",
            result.FamiliesImported, result.GroupsImported, result.VariantsImported, result.AutodenominationsImported,
            result.VariantsQuarantined, result.AutodenominationsQuarantined);
    }

    private async Task CreateCatalogRecordsAsync(
        Guid catalogVersionId,
        Dictionary<string, LanguageFamily> familyMap,
        Dictionary<string, LanguageGroup> groupMap,
        Dictionary<string, LanguageVariant> variantMap,
        Guid sourceDocumentId,
        List<Appendix4Dto> appendix4Data,
        CancellationToken cancellationToken)
    {
        var records = new List<CatalogRecord>();

        // Build lookup: variant name -> page from catalog data
        var variantToPage = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var catalogData = await LoadCatalogDataForPageLookupAsync(cancellationToken);
        foreach (var v in catalogData)
        {
            if (!string.IsNullOrWhiteSpace(v.VariantName))
            {
                variantToPage[v.VariantName.Trim()] = v.Page;
            }
        }

        // Build lookup: agrupacion -> page from appendix4
        var agrupacionToPage = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in appendix4Data)
        {
            if (!string.IsNullOrWhiteSpace(item.Agrupacion) && item.Agrupacion != "Agrupación" && item.Agrupacion != "Oto-mangue" && item.Agrupacion != "u")
            {
                if (!agrupacionToPage.ContainsKey(item.Agrupacion.Trim()))
                {
                    agrupacionToPage[item.Agrupacion.Trim()] = item.Page;
                }
            }
        }

        // Family records - use first page where family appears in main catalog (page 30 is index)
        foreach (var kvp in familyMap)
        {
            // Family entries appear in index page 30 - use that as explicit evidence
            records.Add(new CatalogRecord(
                catalogVersionId: catalogVersionId,
                entityType: "LanguageFamily",
                identityKey: $"family|{kvp.Key}",
                inaliCode: "",
                iso639_3Code: "",
                name: kvp.Key,
                autodenomination: null,
                description: null,
                geoReference: null,
                sourceDocumentId: sourceDocumentId,
                sourceHash: CATALOG_SHA256,
                sourceUrl: CATALOG_URL,
                sourcePage: 30,
                sourceSection: "Index",
                extractionDate: DateTime.UtcNow,
                parserVersion: PARSER_VERSION
            ));
        }

        // Group records - MUST have explicit page from Appendix 4, NO fallback
        foreach (var kvp in groupMap)
        {
            var parts = kvp.Key.Split('|');
            var groupName = parts.Length > 1 ? parts[1] : kvp.Key;

            if (!agrupacionToPage.TryGetValue(groupName.Trim(), out var page))
            {
                // This should not happen if groups were imported from Appendix 4 correctly
                _logger.LogWarning("Group '{GroupName}' has no resolved source page in Appendix 4 - quarantining CatalogRecord", groupName);
                // We'll create a record with explicit UNRESOLVED provenance rather than defaulting
                page = -1; // Mark as unresolved
            }

            records.Add(new CatalogRecord(
                catalogVersionId: catalogVersionId,
                entityType: "LanguageGroup",
                identityKey: $"group|{kvp.Key}",
                inaliCode: "",
                iso639_3Code: "",
                name: groupName,
                autodenomination: null,
                description: null,
                geoReference: null,
                sourceDocumentId: sourceDocumentId,
                sourceHash: CATALOG_SHA256,
                sourceUrl: CATALOG_URL,
                sourcePage: page,
                sourceSection: page > 0 ? "Appendix4" : "UNRESOLVED",
                extractionDate: DateTime.UtcNow,
                parserVersion: PARSER_VERSION
            ));
        }

        // Variant records - MUST have REAL page from catalog data, NO fallback
        foreach (var kvp in variantMap)
        {
            var variant = kvp.Value;

            if (!variantToPage.TryGetValue(variant.Name, out var page))
            {
                _logger.LogWarning("Variant '{VariantName}' has no resolved source page in catalog data - quarantining CatalogRecord", variant.Name);
                page = -1; // Mark as unresolved
            }

            records.Add(new CatalogRecord(
                catalogVersionId: catalogVersionId,
                entityType: "LanguageVariant",
                identityKey: $"variant|{kvp.Key}",
                inaliCode: "",
                iso639_3Code: "",
                name: variant.Name,
                autodenomination: variant.Autodenomination,
                description: null,
                geoReference: null,
                sourceDocumentId: sourceDocumentId,
                sourceHash: CATALOG_SHA256,
                sourceUrl: CATALOG_URL,
                sourcePage: page,
                sourceSection: page > 0 ? "MainCatalog" : "UNRESOLVED",
                extractionDate: DateTime.UtcNow,
                parserVersion: PARSER_VERSION
            ));
        }

        // Autodenomination records - one per autodenom entry with its appendix page
        var sourceDoc = await _context.SourceDocuments.FirstAsync(d => d.HashSha256 == CATALOG_SHA256, cancellationToken);
        var autodenoms = await _context.LanguageVariantAutodenominations
            .Where(a => a.SourceDocumentId == sourceDoc.Id)
            .ToListAsync(cancellationToken);

        foreach (var auto in autodenoms)
        {
            var variant = await _context.LanguageVariants.FindAsync(new object[] { auto.LanguageVariantId }, cancellationToken);
            if (variant != null)
            {
                records.Add(new CatalogRecord(
                    catalogVersionId: catalogVersionId,
                    entityType: "LanguageVariantAutodenomination",
                    identityKey: $"autodenom|{auto.LanguageVariantId}|{auto.Autodenomination}|{auto.SourcePage}",
                    inaliCode: "",
                    iso639_3Code: "",
                    name: auto.Autodenomination,
                    autodenomination: auto.Autodenomination,
                    description: null,
                    geoReference: null,
                    sourceDocumentId: auto.SourceDocumentId,
                    sourceHash: auto.SourceHash,
                    sourceUrl: CATALOG_URL,
                    sourcePage: auto.SourcePage,
                    sourceSection: "Appendix4",
                    extractionDate: DateTime.UtcNow,
                    parserVersion: PARSER_VERSION
                ));
            }
        }

        // Idempotency: Only add records that don't already exist for this CatalogVersion
        var existingRecords = await _context.CatalogRecords
            .Where(cr => cr.CatalogVersionId == catalogVersionId)
            .Select(cr => cr.IdentityKey)
            .ToListAsync(cancellationToken);

        var existingKeys = new HashSet<string>(existingRecords);
        var newRecords = records.Where(r => !existingKeys.Contains(r.IdentityKey)).ToList();

        if (newRecords.Count > 0)
        {
            _context.CatalogRecords.AddRange(newRecords);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Created {Count} new CatalogRecords with real provenance (skipped {Skipped} existing)", newRecords.Count, existingKeys.Count);
        }
        else
        {
            _logger.LogInformation("All {Count} CatalogRecords already exist for this version - idempotent run", existingKeys.Count);
        }
    }

    private async Task<List<CatalogVariantDto>> LoadCatalogDataForPageLookupAsync(CancellationToken cancellationToken)
    {
        var basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
        var catalogPath = Path.Combine(basePath, "catalogs", "parsed", "inali_final_catalog.json");
        var catalogJson = await File.ReadAllTextAsync(catalogPath);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var catalogRoot = JsonSerializer.Deserialize<CatalogRootDto>(catalogJson, options);

        // Normalize variant names (remove newlines from PDF extraction)
        var variants = catalogRoot?.VariantsDetail ?? new();
        foreach (var v in variants)
        {
            if (!string.IsNullOrWhiteSpace(v.VariantName))
            {
                v.VariantName = v.VariantName.Replace("\n", " ").Replace("\r", " ").Trim();
            }
        }

        return variants;
    }

    // ... DeriveGroupName methods unchanged but should be marked as fallback only ...
    private string DeriveGroupName(CatalogVariantDto item)
    {
        return item.Family switch
        {
            "álgica" => "kickapoo",
            "yuto-nahua" => DeriveYutoNahuaGroup(item.VariantName),
            "cochimí-yumana" => "cochimí-yumana",
            "seri" => "seri",
            "oto-mangue" => DeriveOtomangueGroup(item.VariantName),
            "maya" => DeriveMayaGroup(item.VariantName),
            "totonaco-tepehua" => DeriveTotonacoTepehuaGroup(item.VariantName),
            "tarasca" => "tarasca",
            "mixe-zoque" => DeriveMixeZoqueGroup(item.VariantName),
            "chontal de oaxaca" => "chontal de oaxaca",
            "huave" => "huave",
            _ => item.VariantName.Split(' ')[0].ToLower()
        };
    }

    private static Guid? TryDisambiguateByAutodenomLocation(string autodenom, List<LanguageVariant> candidates, out string matchMethod)
    {
        // Look for location qualifiers in autodenomination that match variant names
        // e.g., autodenom "tu'un savi (de Atlamajalcingo)" -> variant "mixteco de Atlamajalcingo"
        matchMethod = "autodenom_location_disambiguation";

        // Extract location in parentheses from autodenom
        var startIdx = autodenom.IndexOf('(');
        var endIdx = autodenom.IndexOf(')');
        LanguageVariant? match = null;
        if (startIdx >= 0 && endIdx > startIdx)
        {
            var location = autodenom.Substring(startIdx + 1, endIdx - startIdx - 1).Trim();
            // Remove common prefixes like "de ", "del ", "del "
            location = location.Replace("de ", "").Replace("del ", "").Replace("dela ", "");

            // Try to find variant containing this location
            match = candidates.FirstOrDefault(v =>
                v.Name.Contains(location, StringComparison.OrdinalIgnoreCase));
            if (match != null)
            {
                matchMethod = "autodenom_location_disambiguation";
                return match.Id;
            }
        }

        // Try matching autodenom directly as substring of variant name
        match = candidates.FirstOrDefault(v =>
            v.Name.Contains(autodenom, StringComparison.OrdinalIgnoreCase) ||
            autodenom.Contains(v.Name, StringComparison.OrdinalIgnoreCase));
        if (match != null)
        {
            matchMethod = "autodenom_substring_match";
            return match.Id;
        }

        matchMethod = "no_disambiguation_possible";
        return null;
    }

    private string DeriveYutoNahuaGroup(string variantName)
    {
        var lower = variantName.ToLower();
        if (lower.Contains("náhuatl")) return "náhuatl";
        if (lower.Contains("pápago") || lower.Contains("o'odham")) return "pápago";
        if (lower.Contains("cora")) return "cora";
        if (lower.Contains("huichol")) return "huichol";
        if (lower.Contains("tepehuano")) return "tepehuano";
        if (lower.Contains("mexicano")) return "mexicano";
        if (lower.Contains("pima")) return "pima";
        return "yuto-nahua";
    }

    private string DeriveOtomangueGroup(string variantName)
    {
        var lower = variantName.ToLower();
        if (lower.Contains("mazateco")) return "mazateco";
        if (lower.Contains("mixteco")) return "mixteco";
        if (lower.Contains("zapoteco")) return "zapoteco";
        if (lower.Contains("chinanteco")) return "chinanteco";
        if (lower.Contains("chocholteco")) return "chocholteco";
        if (lower.Contains("cuicateco")) return "cuicateco";
        if (lower.Contains("ixcateco")) return "ixcateco";
        if (lower.Contains("popoloca")) return "popoloca";
        if (lower.Contains("tlapaneco")) return "tlapaneco";
        if (lower.Contains("amuzgo")) return "amuzgo";
        if (lower.Contains("chatino")) return "chatino";
        if (lower.Contains("triqui")) return "triqui";
        if (lower.Contains("tlacuilo")) return "tlacuilo";
        return "oto-mangue";
    }

    private string DeriveMayaGroup(string variantName)
    {
        var lower = variantName.ToLower();
        if (lower.Contains("maya") && !lower.Contains("q'anjob") && !lower.Contains("kanjobal")) return "maya";
        if (lower.Contains("tzeltal")) return "tzeltal";
        if (lower.Contains("tzotzil")) return "tzotzil";
        if (lower.Contains("chol")) return "chol";
        if (lower.Contains("tojolabal")) return "tojolabal";
        if (lower.Contains("chuj")) return "chuj";
        if (lower.Contains("kanjobal") || lower.Contains("q'anjob'al")) return "q'anjob'al";
        if (lower.Contains("jacalteco")) return "jacalteco";
        if (lower.Contains("acateco")) return "acateco";
        if (lower.Contains("ixil")) return "ixil";
        if (lower.Contains("awakatek")) return "awakatek";
        if (lower.Contains("mochó")) return "mochó";
        if (lower.Contains("tektiteko")) return "tektiteko";
        return "maya";
    }

    private string DeriveTotonacoTepehuaGroup(string variantName)
    {
        var lower = variantName.ToLower();
        if (lower.Contains("totonaco")) return "totonaco";
        if (lower.Contains("tepehua")) return "tepehua";
        return "totonaco-tepehua";
    }

    private string DeriveMixeZoqueGroup(string variantName)
    {
        var lower = variantName.ToLower();
        if (lower.Contains("mixe")) return "mixe";
        if (lower.Contains("zoque")) return "zoque";
        if (lower.Contains("popoluca")) return "popoluca";
        if (lower.Contains("tapachulteco")) return "tapachulteco";
        return "mixe-zoque";
    }

    // DTOs
    private class CatalogRootDto
    {
        public string Source { get; set; } = "";
        public string Resource { get; set; } = "";
        public string ResourceUrl { get; set; } = "";
        public string PublicationDate { get; set; } = "";
        public string RetrievedAt { get; set; } = "";
        public string Sha256 { get; set; } = "";
        public int TotalPages { get; set; }
        public string CatalogPages { get; set; } = "";
        public int Families { get; set; }
        public Dictionary<string, int> VariantsByFamily { get; set; } = new();
        public Dictionary<string, int> GroupsByFamily { get; set; } = new();
        [System.Text.Json.Serialization.JsonPropertyName("variants_detail")]
        public List<CatalogVariantDto> VariantsDetail { get; set; } = new();
    }

    private class CatalogVariantDto
    {
        public int Page { get; set; }
        public string Family { get; set; } = "";
        [System.Text.Json.Serialization.JsonPropertyName("variant_name")]
        public string VariantName { get; set; } = "";
        [System.Text.Json.Serialization.JsonPropertyName("autodenomination")]
        public string Autodenomination { get; set; } = "";
        [System.Text.Json.Serialization.JsonPropertyName("geo_reference")]
        public string GeoReference { get; set; } = "";
    }

    internal class Appendix4Dto
    {
        public int Page { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("autodenom")]
        public string Autodenom { get; set; } = "";
        [System.Text.Json.Serialization.JsonPropertyName("spanish_name")]
        public string SpanishName { get; set; } = "";
        [System.Text.Json.Serialization.JsonPropertyName("agrupacion")]
        public string Agrupacion { get; set; } = "";
        [System.Text.Json.Serialization.JsonPropertyName("familia")]
        public string Familia { get; set; } = "";
        // 5B.md FASE 2: deterministic source-row identity = ordinal position in the raw
        // appendix4_parsed.json array (assigned in LoadSourceDataAsync before any filtering).
        public int SourceOrdinal { get; set; }
    }

    // 5B.md FASE 3: Reconciliation ledger entry for row-by-row forensic accounting
    private class AutodenominationReconciliationEntry
    {
        public int SourceOrdinal { get; set; }
        public int SourcePage { get; set; }
        public Guid SourceDocumentId { get; set; }
        public string RawDataHash { get; set; } = "";
        public string SpanishName { get; set; } = "";
        public string IndigenousName { get; set; } = "";
        public string Grouping { get; set; } = "";
        public string Familia { get; set; } = "";
        public string TerminalOutcome { get; set; } = ""; // IMPORTED | QUARANTINED (exactly one per source row)
        public Guid? CatalogRecordId { get; set; }
        public Guid? QuarantineId { get; set; }
        public string ResolutionMethod { get; set; } = "";
        public string Reason { get; set; } = "";
    }

    // 5B.md FASE 7: outcome of the pure variant-resolution step. A null VariantId always carries
    // the resolution method and reason the caller must persist as an explicit quarantine.
    internal readonly record struct VariantResolution(Guid? VariantId, string Method, string FailureMethod, string FailureReason)
    {
        public static VariantResolution Resolved(Guid variantId, string method) => new(variantId, method, "", "");

        public static VariantResolution Unresolved(string failureMethod, string reason) => new(null, failureMethod, failureMethod, reason);
    }
}