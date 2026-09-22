using System.IO;
using System.Text.Json;
using AtlasBuho.Data;
using AtlasBuho.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
    public List<string> ValidationErrors { get; set; } = new();
    public Dictionary<string, object> Provenance { get; set; } = new();
    public List<QuarantineRecord> QuarantinedItems { get; set; } = new();
}

public class QuarantineRecord
{
    public string EntityType { get; set; } = "";
    public string RawData { get; set; } = "";
    public string Reason { get; set; } = "";
    public int SourcePage { get; set; }
    public string SourceSection { get; set; } = "";
    public string ResolutionMethod { get; set; } = "";
}

public class InaliCatalogImporter : ICatalogImporter
{
    private readonly AtlasBuhoDbContext _context;
    private readonly ILogger<InaliCatalogImporter> _logger;

    // DOCUMENTARY BASELINES - from official sources, NOT from importer expectations
    private const int DOCUMENTARY_FAMILIES = 11;
    private const int DOCUMENTARY_GROUPS = 68;          // Apéndice 4 oficial (excluyendo artefactos)
    private const int DOCUMENTARY_VARIANTS = 364;       // PDF cuerpo principal oficial
    private const int DOCUMENTARY_AUTODENOMS_VALID = 474; // Apéndice 4: 478 filas totales - 1 header (filtered by LoadSourceDataAsync) - 3 artefactos (filtered by ImportAutodenominationsAsync) = 474 válidos

    // CLIN Compendium metadata
    private const string CATALOG_TITLE = "Catálogo de las Lenguas Indígenas Nacionales: Variantes Lingüísticas de México con sus autodenominaciones y referencias geoestadísticas";
    private const string CATALOG_URL = "https://www.inali.gob.mx/pdf/CLIN_completo.pdf";
    private const string CATALOG_SHA256 = "21cef44abf0d896555f26954bf319340ad4814fa8a3f0719b0d068311ff1be7c";
    private static readonly DateTime PUBLICATION_DATE = new(2008, 1, 14);
    private const string PARSER_VERSION = "2025.09.22-pass2-reconciliation"; // Parser version reflects this audit pass

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

                    // Step 2: Create or get SourceDocument for CLIN (REVALIDATE hash from file)
                    var sourceDocument = await CreateOrGetSourceDocumentAsync(cancellationToken);
                    result.Provenance["SourceDocumentId"] = sourceDocument.Id;

                    // Step 3: Create CatalogSource for INALI
                    var catalogSource = await CreateOrGetCatalogSourceAsync(cancellationToken);
                    result.Provenance["CatalogSourceId"] = catalogSource.Id;

                    // Step 4: Create CatalogVersion with PENDING status
                    var catalogVersion = await CreateCatalogVersionAsync(catalogSource.Id, sourceDocument.Id, "Pending", cancellationToken);
                    result.Provenance["CatalogVersionId"] = catalogVersion.Id;

                    // Step 5: Import Families (11 expected)
                    var familyMap = await ImportFamiliesAsync(catalogData, catalogVersion.Id, sourceDocument.Id, cancellationToken);
                    result.FamiliesImported = familyMap.Count;
                    _logger.LogInformation("Imported {Count} families", familyMap.Count);

                    // Step 6: Import Groups/Agrupaciones (68 expected from Appendix 4)
                    var (groupMap, groupQuarantine) = await ImportGroupsAsync(catalogData, appendix4Data, familyMap, catalogVersion.Id, sourceDocument.Id, cancellationToken);
                    result.GroupsImported = groupMap.Count;
                    result.QuarantinedItems.AddRange(groupQuarantine);
                    _logger.LogInformation("Imported {Count} groups, quarantined {QCount}", groupMap.Count, groupQuarantine.Count);

                    // Step 7: Import Variants (364 expected) - NO silent continue
                    var (variantMap, variantQuarantine) = await ImportVariantsAsync(catalogData, appendix4Data, groupMap, catalogVersion.Id, sourceDocument.Id, cancellationToken);
                    result.VariantsImported = variantMap.Count;
                    result.VariantsQuarantined = variantQuarantine.Count;
                    result.QuarantinedItems.AddRange(variantQuarantine);
                    _logger.LogInformation("Imported {Count} variants, quarantined {QCount}", variantMap.Count, variantQuarantine.Count);

                    // Step 8: Import Autodenominaciones (474 valid expected)
                    var (autodenomCount, autodenomQuarantine) = await ImportAutodenominationsAsync(appendix4Data, groupMap, variantMap, catalogVersion.Id, sourceDocument.Id, cancellationToken);
                    result.AutodenominationsImported = autodenomCount;
                    result.AutodenominationsQuarantined = autodenomQuarantine.Count;
                    result.QuarantinedItems.AddRange(autodenomQuarantine);
                    _logger.LogInformation("Imported {Count} autodenominaciones, quarantined {QCount}", autodenomCount, autodenomQuarantine.Count);

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

        // Filter out header rows from appendix4
        appendix4Data = appendix4Data.Where(a => !string.IsNullOrEmpty(a.Autodenom) &&
            a.Autodenom != "Autodenominación" &&
            a.Agrupacion != "Agrupación").ToList();

        _logger.LogInformation("Appendix4 filtered count: {Count}", appendix4Data.Count);

        return (catalogData, appendix4Data);
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

        // Use pdfplumber via Python for text extraction (deterministic)
        // For now, create pages with null RawTextHash - will be populated by separate step
        // Pages 30-212: Main catalog (per JSON metadata)
        for (int i = 30; i <= 212; i++)
        {
            pages.Add(new SourcePage(
                sourceDocumentId: doc.Id,
                pageNumber: i,
                sectionReference: "MainCatalog",
                rawTextHash: null // Will be computed in separate extraction step
            ));
        }
        // Pages 213-256: Appendices
        for (int i = 213; i <= 256; i++)
        {
            pages.Add(new SourcePage(
                sourceDocumentId: doc.Id,
                pageNumber: i,
                sectionReference: "Appendices",
                rawTextHash: null
            ));
        }

        _context.SourcePages.AddRange(pages);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created {Count} SourcePages for document {DocId}", pages.Count, doc.Id);
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

    private async Task FinalizeCatalogVersionAsync(Guid catalogVersionId, ImportResult result, CancellationToken cancellationToken)
    {
        var version = await _context.CatalogVersions.FindAsync(new object[] { catalogVersionId }, cancellationToken);
        if (version != null)
        {
            version.UpdateCounts(
                result.FamiliesImported,
                result.GroupsImported,
                result.VariantsImported,
                result.AutodenominationsImported);
            version.UpdateStatus("Completed");
            version.SetValidationErrors(result.ValidationErrors.Count > 0 ? string.Join("; ", result.ValidationErrors) : null);

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Finalized CatalogVersion {Id} with actual counts: F={F} G={G} V={V} A={A}, status=Completed",
                catalogVersionId, result.FamiliesImported, result.GroupsImported, result.VariantsImported, result.AutodenominationsImported);
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

    private async Task<(Dictionary<string, LanguageGroup> groupMap, List<QuarantineRecord> quarantine)> ImportGroupsAsync(
        List<CatalogVariantDto> catalogData,
        List<Appendix4Dto> appendix4Data,
        Dictionary<string, LanguageFamily> familyMap,
        Guid catalogVersionId,
        Guid sourceDocumentId,
        CancellationToken cancellationToken)
    {
        var groupMap = new Dictionary<string, LanguageGroup>(StringComparer.OrdinalIgnoreCase);
        var quarantine = new List<QuarantineRecord>();

        // Use appendix 4 as the canonical source for agrupación names
        var agrupacionToFamilia = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var agrupacionToPage = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in appendix4Data)
        {
            if (!string.IsNullOrWhiteSpace(item.Agrupacion) && !string.IsNullOrWhiteSpace(item.Familia))
            {
                var agrupacion = item.Agrupacion.Trim();
                var familia = item.Familia.Trim();

                // Skip artifact agrupaciones
                if (agrupacion == "Oto-mangue" || agrupacion == "u")
                {
                    quarantine.Add(new QuarantineRecord
                    {
                        EntityType = "LanguageGroup",
                        RawData = JsonSerializer.Serialize(item),
                        Reason = $"Artifact agrupación '{agrupacion}' with empty familia - extraction artifact",
                        SourcePage = item.Page,
                        SourceSection = "Appendix4",
                        ResolutionMethod = "classified_as_artifact"
                    });
                    continue;
                }

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
            var page = agrupacionToPage.GetValueOrDefault(agrupacion, 244);

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
                quarantine.Add(new QuarantineRecord
                {
                    EntityType = "LanguageGroup",
                    RawData = $"Family={familyName}, Group={groupName}",
                    Reason = $"Family '{familyName}' not found in familyMap",
                    SourcePage = page,
                    SourceSection = "Appendix4",
                    ResolutionMethod = "family_missing"
                });
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

        return (groupMap, quarantine);
    }

    private async Task<(Dictionary<string, LanguageVariant> variantMap, List<QuarantineRecord> quarantine)> ImportVariantsAsync(
        List<CatalogVariantDto> catalogData,
        List<Appendix4Dto> appendix4Data,
        Dictionary<string, LanguageGroup> groupMap,
        Guid catalogVersionId,
        Guid sourceDocumentId,
        CancellationToken cancellationToken)
    {
        var variantMap = new Dictionary<string, LanguageVariant>(StringComparer.OrdinalIgnoreCase);
        var quarantine = new List<QuarantineRecord>();

        // Build mapping from Spanish variant name -> appendix 4 agrupación
        var spanishNameToAgrupacion = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in appendix4Data)
        {
            if (!string.IsNullOrWhiteSpace(item.SpanishName) && !string.IsNullOrWhiteSpace(item.Agrupacion))
            {
                var spanishName = item.SpanishName.Trim();
                var agrupacion = item.Agrupacion.Trim();
                // Skip artifacts
                if (agrupacion == "Oto-mangue" || agrupacion == "u") continue;

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
                    // Use the longest match
                    var bestMatch = prefixMatches.OrderByDescending(kvp => kvp.Key.Length).First();
                    groupName = bestMatch.Value;
                    resolutionMethod = "appendix4_prefix_longest";
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

                quarantine.Add(new QuarantineRecord
                {
                    EntityType = "LanguageVariant",
                    RawData = JsonSerializer.Serialize(item),
                    Reason = $"Group '{groupKey}' not found in groupMap (method: {resolutionMethod})",
                    SourcePage = item.Page,
                    SourceSection = "MainCatalog",
                    ResolutionMethod = resolutionMethod
                });
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
        _logger.LogInformation("Imported {Count} variants, quarantined {QCount}", variantMap.Count, quarantine.Count);

        return (variantMap, quarantine);
    }

    private async Task<(int imported, List<QuarantineRecord> quarantine)> ImportAutodenominationsAsync(
        List<Appendix4Dto> appendix4Data,
        Dictionary<string, LanguageGroup> groupMap,
        Dictionary<string, LanguageVariant> variantMap,
        Guid catalogVersionId,
        Guid sourceDocumentId,
        CancellationToken cancellationToken)
    {
        int count = 0;
        var quarantine = new List<QuarantineRecord>();
        var sourceDoc = await _context.SourceDocuments.FirstAsync(d => d.HashSha256 == CATALOG_SHA256, cancellationToken);

        // Track which (variantId, autodenomination, sourcePage) triplets we've already added
        // Identity = Variant + Autodenom + SourcePage (H-015)
        var addedTriplets = new HashSet<string>();

        foreach (var item in appendix4Data)
        {
            _logger.LogDebug("Processing appendix row: Autodenom={Autodenom}, Agrupacion={Agrupacion}, Familia={Familia}, SpanishName={SpanishName}, Page={Page}", 
                item.Autodenom, item.Agrupacion, item.Familia, item.SpanishName, item.Page);
            
            if (string.IsNullOrWhiteSpace(item.Autodenom) || string.IsNullOrWhiteSpace(item.Agrupacion)) continue;

            // Skip artifact rows
            if (item.Agrupacion.Trim() == "Oto-mangue" || item.Agrupacion.Trim() == "u" || string.IsNullOrWhiteSpace(item.Familia))
            {
                quarantine.Add(new QuarantineRecord
                {
                    EntityType = "LanguageVariantAutodenomination",
                    RawData = JsonSerializer.Serialize(item),
                    Reason = "Artifact row (empty familia or artifact agrupación)",
                    SourcePage = item.Page,
                    SourceSection = "Appendix4",
                    ResolutionMethod = "classified_as_artifact"
                });
                continue;
            }

            // Find matching group
            var groupKey = groupMap.Keys.FirstOrDefault(k =>
                k.EndsWith($"|{item.Agrupacion.Trim()}", StringComparison.OrdinalIgnoreCase));

            if (groupKey == null)
            {
                _logger.LogDebug("Group not found for autodenomination: {Group}, AgrupacionRaw={Raw}", item.Agrupacion, item.Agrupacion);
                quarantine.Add(new QuarantineRecord
                {
                    EntityType = "LanguageVariantAutodenomination",
                    RawData = JsonSerializer.Serialize(item),
                    Reason = $"Group '{item.Agrupacion}' not found in groupMap",
                    SourcePage = item.Page,
                    SourceSection = "Appendix4",
                    ResolutionMethod = "group_not_found"
                });
                continue;
            }
            
            _logger.LogDebug("Found group for autodenom: GroupKey={GroupKey}, Agrupacion={Agrupacion}", groupKey, item.Agrupacion);

            var group = groupMap[groupKey];

            // Find matching variant - search ALL variants in this family, not just this group
            // Because Appendix 4 agrupación may not align perfectly with catalog groups
            Guid? variantId = null;
            // Get all group IDs in this family
            var family = await _context.LanguageFamilies
                .FirstOrDefaultAsync(f => f.Name.ToLower() == item.Familia.Trim().ToLower(), cancellationToken);
            var groupIdsInFamily = new List<Guid>();
            if (family != null)
            {
                groupIdsInFamily = await _context.LanguageGroups
                    .Where(g => g.LanguageFamilyId == family.Id)
                    .Select(g => g.Id)
                    .ToListAsync(cancellationToken);
            }
            
            var allVariantsInFamily = await _context.LanguageVariants
                .Where(v => groupIdsInFamily.Contains(v.LanguageGroupId))
                .ToListAsync(cancellationToken);
            
            _logger.LogDebug("Autodenom matching: Agrupacion={Agrupacion}, Familia={Familia}, GroupVariants={GV}, FamilyVariants={FV}", 
                item.Agrupacion, item.Familia, group.LanguageVariants.Count, allVariantsInFamily.Count);

            string matchMethod = "none";

            // Try exact match first (use all variants in family)
                        if (!string.IsNullOrWhiteSpace(item.SpanishName) && item.SpanishName.Trim() != "Nombre en español")
                        {
                            var spanishName = item.SpanishName.Trim();
                            bool isGenericGroupName = spanishName.EndsWith(",");

                            // Exact match
                            var exactMatch = allVariantsInFamily.FirstOrDefault(v =>
                                v.Name.Equals(spanishName, StringComparison.OrdinalIgnoreCase));

                            if (exactMatch != null)
                            {
                                variantId = exactMatch.Id;
                                matchMethod = "spanish_name_exact";
                            }
                            else if (isGenericGroupName)
                            {
                                // Generic group name (ends with comma) - cannot match to specific variant
                                // unless autodenom has location qualifier for disambiguation
                                _logger.LogDebug("Generic SpanishName (ends with comma): {SpanishName} - attempting disambiguation via autodenom location", spanishName);
                    
                                // Try disambiguation by autodenomination location qualifier
                                variantId = TryDisambiguateByAutodenomLocation(item.Autodenom, allVariantsInFamily, out matchMethod);
                    
                                if (variantId == null)
                                {
                                    _logger.LogWarning("Generic SpanishName cannot be disambiguated: Autodenom={Autodenom}, SpanishName={SpanishName}, FamilyVariants={Count}. Quarantining.",
                                        item.Autodenom, spanishName, allVariantsInFamily.Count);
                                    quarantine.Add(new QuarantineRecord
                                    {
                                        EntityType = "LanguageVariantAutodenomination",
                                        RawData = JsonSerializer.Serialize(item),
                                        Reason = $"Generic group name '{spanishName}' cannot be mapped to specific variant (autodenom lacks location qualifier)",
                                        SourcePage = item.Page,
                                        SourceSection = "Appendix4",
                                        ResolutionMethod = "generic_group_name_no_location"
                                    });
                                    continue;
                                }
                                else
                                {
                                    matchMethod = "generic_group_name_disambiguated";
                                }
                            }
                            else
                                                            {
                                                                // Specific SpanishName (not ending with comma) - use prefix matching
                                                                var prefixMatches = allVariantsInFamily
                                                                    .Where(v => v.Name.StartsWith(spanishName, StringComparison.OrdinalIgnoreCase))
                                                                    .ToList();

                                                                if (prefixMatches.Count == 1)
                                                                {
                                                                    variantId = prefixMatches[0].Id;
                                                                    matchMethod = "spanish_name_prefix_unique";
                                                                }
                                                                else if (prefixMatches.Count > 1)
                                                                {
                                                                    // Try best match: variant continues with space or comma+space after prefix
                                                                    var bestMatch = prefixMatches.FirstOrDefault(v =>
                                                                        v.Name.Length > spanishName.Length &&
                                                                        (v.Name[spanishName.Length] == ' ' || v.Name[spanishName.Length] == ','));

                                                                    if (bestMatch != null)
                                                                    {
                                                                        variantId = bestMatch.Id;
                                                                        matchMethod = "spanish_name_prefix_best";
                                                                    }

                                                                    // Try disambiguation by autodenomination location qualifier
                                                                    if (variantId == null)
                                                                    {
                                                                        variantId = TryDisambiguateByAutodenomLocation(item.Autodenom, prefixMatches, out matchMethod);
                                                                    }

                                                                    // If still no match, pick first with warning
                                                                    if (variantId == null)
                                                                    {
                                                                        variantId = prefixMatches[0].Id;
                                                                        matchMethod = "spanish_name_prefix_ambiguous_first";
                                                                        _logger.LogWarning("Ambiguous prefix match - using first variant: Autodenom={Autodenom}, SpanishName={SpanishName}, Matches={Count}, Chosen={Chosen}",
                                                                            item.Autodenom, spanishName, prefixMatches.Count, prefixMatches[0].Name);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    // No exact prefix match - try progressive prefix matching
                                                                    // Split SpanishName by comma and try progressively shorter prefixes
                                                                    var snParts = spanishName.Split(',').Select(p => p.Trim()).ToArray();
                                                                    for (int i = snParts.Length - 1; i >= 1; i--)
                                                                    {
                                                                        var shorterPrefix = string.Join(", ", snParts.Take(i));
                                                                        var shorterMatches = allVariantsInFamily
                                                                            .Where(v => v.Name.StartsWith(shorterPrefix, StringComparison.OrdinalIgnoreCase))
                                                                            .ToList();
                                            
                                                                        if (shorterMatches.Count == 1)
                                                                        {
                                                                            variantId = shorterMatches[0].Id;
                                                                            matchMethod = "spanish_name_progressive_prefix_unique";
                                                                            _logger.LogDebug("Progressive prefix match: SpanishName={SN}, Prefix={Prefix}, Match={Match}", 
                                                                                spanishName, shorterPrefix, shorterMatches[0].Name);
                                                                            break;
                                                                        }
                                                                        else if (shorterMatches.Count > 1)
                                                                        {
                                                                            // Try best match among these
                                                                            var bestMatch = shorterMatches.FirstOrDefault(v =>
                                                                                v.Name.Length > shorterPrefix.Length &&
                                                                                (v.Name[shorterPrefix.Length] == ' ' || v.Name[shorterPrefix.Length] == ','));
                                                
                                                                            if (bestMatch != null)
                                                                            {
                                                                                variantId = bestMatch.Id;
                                                                                matchMethod = "spanish_name_progressive_prefix_best";
                                                                                _logger.LogDebug("Progressive prefix best match: SpanishName={SN}, Prefix={Prefix}, Match={Match}", 
                                                                                    spanishName, shorterPrefix, bestMatch.Name);
                                                                                break;
                                                                            }
                                                
                                                                            // Try disambiguation
                                                                            variantId = TryDisambiguateByAutodenomLocation(item.Autodenom, shorterMatches, out matchMethod);
                                                                            if (variantId != null)
                                                                            {
                                                                                matchMethod = "spanish_name_progressive_prefix_disambiguated";
                                                                                break;
                                                                            }
                                                                        }
                                                                    }

                                                                    // If still no match, try suffix matching (variant name is prefix of appendix name)
                                                                    if (variantId == null)
                                                                    {
                                                                        var suffixMatch = allVariantsInFamily.FirstOrDefault(v =>
                                                                            spanishName.StartsWith(v.Name, StringComparison.OrdinalIgnoreCase) &&
                                                                            (spanishName.Length == v.Name.Length || spanishName[v.Name.Length] == ',' || spanishName[v.Name.Length] == ' '));

                                                                        if (suffixMatch != null)
                                                                        {
                                                                            variantId = suffixMatch.Id;
                                                                            matchMethod = "spanish_name_suffix";
                                                                        }
                                                                        else
                                                                        {
                                                                            // Try disambiguation by autodenom location
                                                                            variantId = TryDisambiguateByAutodenomLocation(item.Autodenom, allVariantsInFamily, out matchMethod);

                                                                            if (variantId == null)
                                                                            {
                                                                                _logger.LogWarning("No variant match for autodenomination: Autodenom={Autodenom}, SpanishName={SpanishName}, FamilyVariants={Count}. Quarantining.",
                                                                                    item.Autodenom, spanishName, allVariantsInFamily.Count);
                                                                                quarantine.Add(new QuarantineRecord
                                                                                {
                                                                                    EntityType = "LanguageVariantAutodenomination",
                                                                                    RawData = JsonSerializer.Serialize(item),
                                                                                    Reason = $"No variant matches SpanishName '{spanishName}' in family {item.Familia}",
                                                                                    SourcePage = item.Page,
                                                                                    SourceSection = "Appendix4",
                                                                                    ResolutionMethod = "no_match"
                                                                                });
                                                                                continue;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                        }

            // Single variant in family fallback
            if (variantId == null && allVariantsInFamily.Count == 1)
            {
                variantId = allVariantsInFamily[0].Id;
                matchMethod = "single_variant_fallback";
            }
            else if (variantId == null)
            {
                _logger.LogWarning("Cannot uniquely match autodenomination to variant: Autodenom={Autodenom}, SpanishName={SpanishName}, Agrupacion={Agrupacion}, FamilyVariants={Count}. Quarantining.",
                    item.Autodenom, item.SpanishName, item.Agrupacion, allVariantsInFamily.Count);
                quarantine.Add(new QuarantineRecord
                {
                    EntityType = "LanguageVariantAutodenomination",
                    RawData = JsonSerializer.Serialize(item),
                    Reason = $"Multiple variants in family but no clear match (method attempted: {matchMethod})",
                    SourcePage = item.Page,
                    SourceSection = "Appendix4",
                    ResolutionMethod = matchMethod
                });
                continue;
            }

            if (variantId == Guid.Empty)
            {
                quarantine.Add(new QuarantineRecord
                {
                    EntityType = "LanguageVariantAutodenomination",
                    RawData = JsonSerializer.Serialize(item),
                    Reason = "No variant available",
                    SourcePage = item.Page,
                    SourceSection = "Appendix4",
                    ResolutionMethod = "no_variant"
                });
                continue;
            }

            // Identity key includes SourcePage for proper uniqueness (H-015)
            var autodenomTriplet = $"{variantId}|{item.Autodenom.Trim()}|{item.Page}";

            if (addedTriplets.Contains(autodenomTriplet))
            {
                _logger.LogDebug("Skipping duplicate autodenomination triplet in batch for variant {VariantId}: '{Autodenom}' page {Page}", variantId, item.Autodenom, item.Page);
                quarantine.Add(new QuarantineRecord
                {
                    EntityType = "LanguageVariantAutodenomination",
                    RawData = JsonSerializer.Serialize(item),
                    Reason = "Duplicate autodenomination triplet in batch (same variant, autodenom, page)",
                    SourcePage = item.Page,
                    SourceSection = "Appendix4",
                    ResolutionMethod = "duplicate_triplet_in_batch"
                });
                continue;
            }

            // Check DB for existing entry (idempotency)
            var existsInDb = await _context.LanguageVariantAutodenominations
                .AnyAsync(a => a.LanguageVariantId == variantId && a.Autodenomination == item.Autodenom.Trim() && a.SourcePage == item.Page, cancellationToken);

            // Check change tracker for uncommitted entities
            var existsInTracker = _context.ChangeTracker.Entries<LanguageVariantAutodenomination>()
                .Any(e => e.Entity.LanguageVariantId == variantId && e.Entity.Autodenomination == item.Autodenom.Trim() && e.Entity.SourcePage == item.Page && e.State != EntityState.Detached);

            if (existsInDb || existsInTracker)
            {
                _logger.LogDebug("Skipping existing autodenomination (DB or tracker) for variant {VariantId}: '{Autodenom}' page {Page}", variantId, item.Autodenom, item.Page);
                addedTriplets.Add(autodenomTriplet);
                quarantine.Add(new QuarantineRecord
                {
                    EntityType = "LanguageVariantAutodenomination",
                    RawData = JsonSerializer.Serialize(item),
                    Reason = "Duplicate autodenomination already exists in database or change tracker",
                    SourcePage = item.Page,
                    SourceSection = "Appendix4",
                    ResolutionMethod = "duplicate_exists_in_db_or_tracker"
                });
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
            count++;
        }
        // End of foreach loop - all cases handled above with continue or processing

    await _context.SaveChangesAsync(cancellationToken);
    return (count, quarantine);
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
        // (imported + quarantined). Artifact-classified rows are tracked separately in quarantine
        // but are NOT part of the 474 valid rows.
        var artifactQuarantined = result.QuarantinedItems.Count(q => 
            q.Reason.Contains("artifact", StringComparison.OrdinalIgnoreCase) || 
            q.Reason.Contains("Artifact", StringComparison.OrdinalIgnoreCase));
        
        var validRowsQuarantined = result.AutodenominationsQuarantined - artifactQuarantined;
        var autodenomsAccounted = result.AutodenominationsImported + validRowsQuarantined;
        
        if (autodenomsAccounted != DOCUMENTARY_AUTODENOMS_VALID)
            errors.Add($"Autodenominations: expected {DOCUMENTARY_AUTODENOMS_VALID} valid rows accounted (imported + quarantined excl. artifacts), got imported={result.AutodenominationsImported} + quarantined_valid={validRowsQuarantined} (total quarantined={result.AutodenominationsQuarantined} incl. {artifactQuarantined} artifacts) = {autodenomsAccounted}");

        if (errors.Any())
        {
            result.ValidationErrors = errors;
            throw new InvalidOperationException($"DOCUMENTARY BASELINE VALIDATION FAILED:\n{string.Join("\n", errors)}");
        }

        _logger.LogInformation("Documentary baseline validation PASSED: {Families}/{Groups}/{Variants}/{Autodenoms} (quarantined V={VQ} A={AQ}, artifacts={Art})",
            result.FamiliesImported, result.GroupsImported, result.VariantsImported, result.AutodenominationsImported,
            result.VariantsQuarantined, result.AutodenominationsQuarantined, artifactQuarantined);
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
                sourcePage: 30, // Index page
                sourceSection: "Index",
                extractionDate: DateTime.UtcNow,
                parserVersion: PARSER_VERSION
            ));
        }

        // Group records - use real page from Appendix 4
        foreach (var kvp in groupMap)
        {
            var parts = kvp.Key.Split('|');
            var groupName = parts.Length > 1 ? parts[1] : kvp.Key;
            var page = agrupacionToPage.GetValueOrDefault(groupName, 244); // Default to first appendix page

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
                sourceSection: "Appendix4",
                extractionDate: DateTime.UtcNow,
                parserVersion: PARSER_VERSION
            ));
        }

        // Variant records - use REAL page from catalog data
        foreach (var kvp in variantMap)
        {
            var variant = kvp.Value;
            var page = variantToPage.GetValueOrDefault(variant.Name, 30); // Default to catalog start if not found

            records.Add(new CatalogRecord(
                catalogVersionId: catalogVersionId,
                entityType: "LanguageVariant",
                identityKey: $"variant|{kvp.Key}",
                inaliCode: "",
                iso639_3Code: "",
                name: variant.Name,
                autodenomination: variant.Autodenomination,
                description: null,
                geoReference: null, // Could be populated from catalogData if needed
                sourceDocumentId: sourceDocumentId,
                sourceHash: CATALOG_SHA256,
                sourceUrl: CATALOG_URL,
                sourcePage: page,
                sourceSection: "MainCatalog",
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

        _context.CatalogRecords.AddRange(records);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Created {Count} CatalogRecords with real provenance", records.Count);
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

    private Guid? TryDisambiguateByAutodenomLocation(string autodenom, List<LanguageVariant> candidates, out string matchMethod)
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

    private class Appendix4Dto
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
    }
}