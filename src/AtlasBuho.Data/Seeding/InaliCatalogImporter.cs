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
    public int AutodenominationsImported { get; set; }
    public List<string> ValidationErrors { get; set; } = new();
    public Dictionary<string, object> Provenance { get; set; } = new();
}

public class InaliCatalogImporter : ICatalogImporter
{
    private readonly AtlasBuhoDbContext _context;
    private readonly ILogger<InaliCatalogImporter> _logger;
    
    // Baseline constants from 3b.md and Operation 69 verification
    private const int EXPECTED_FAMILIES = 11;
    private const int EXPECTED_GROUPS = 68;
    private const int EXPECTED_VARIANTS = 361;
    private const int EXPECTED_AUTODENOMINATIONS = 424;
    
    // CLIN Compendium metadata
    private const string CATALOG_TITLE = "Catálogo de las Lenguas Indígenas Nacionales: Variantes Lingüísticas de México con sus autodenominaciones y referencias geoestadísticas";
    private const string CATALOG_URL = "https://www.inali.gob.mx/pdf/CLIN_completo.pdf";
    private const string CATALOG_SHA256 = "21cef44abf0d896555f26954bf319340ad4814fa8a3f0719b0d068311ff1be7c";
    private static readonly DateTime PUBLICATION_DATE = new(2008, 1, 14);
    private const string PARSER_VERSION = "1.0.0";

    public InaliCatalogImporter(AtlasBuhoDbContext context, ILogger<InaliCatalogImporter> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ImportResult> ImportInaliCatalogAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting INALI catalog import with baseline validation");
        
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
                    
                                    // Step 2: Create or get SourceDocument for CLIN
                    var sourceDocument = await CreateOrGetSourceDocumentAsync(cancellationToken);
                    result.Provenance["SourceDocumentId"] = sourceDocument.Id;
                    
                    // Step 3: Create CatalogSource for INALI
                    var catalogSource = await CreateOrGetCatalogSourceAsync(cancellationToken);
                    result.Provenance["CatalogSourceId"] = catalogSource.Id;
                    
                    // Step 4: Create CatalogVersion
                    var catalogVersion = await CreateCatalogVersionAsync(catalogSource.Id, sourceDocument.Id, cancellationToken);
                    result.Provenance["CatalogVersionId"] = catalogVersion.Id;
                    
                    // Step 5: Import Families (11 expected)
                    var familyMap = await ImportFamiliesAsync(catalogData, catalogVersion.Id, sourceDocument.Id, cancellationToken);
                    result.FamiliesImported = familyMap.Count;
                    _logger.LogInformation("Imported {Count} families", familyMap.Count);
                    
                    // Step 6: Import Groups/Agrupaciones (68 expected)
                    var groupMap = await ImportGroupsAsync(catalogData, appendix4Data, familyMap, catalogVersion.Id, sourceDocument.Id, cancellationToken);
                    result.GroupsImported = groupMap.Count;
                    _logger.LogInformation("Imported {Count} groups", groupMap.Count);
                    
                    // Step 7: Import Variants (364 expected)
                    var variantMap = await ImportVariantsAsync(catalogData, appendix4Data, groupMap, catalogVersion.Id, sourceDocument.Id, cancellationToken);
                    result.VariantsImported = variantMap.Count;
                    _logger.LogInformation("Imported {Count} variants", variantMap.Count);
                    
                    // Step 8: Import Autodenominaciones (475 expected)
                    var autodenomCount = await ImportAutodenominationsAsync(appendix4Data, groupMap, variantMap, catalogVersion.Id, sourceDocument.Id, cancellationToken);
                    result.AutodenominationsImported = autodenomCount;
                    _logger.LogInformation("Imported {Count} autodenominaciones", autodenomCount);
                    
                    // Step 9: Validate baseline counts - FAIL if mismatch
                    ValidateBaselineCounts(result);
                    
                    // Step 10: Create CatalogRecords for provenance tracking
                    await CreateCatalogRecordsAsync(catalogVersion.Id, familyMap, groupMap, variantMap, sourceDocument.Id, cancellationToken);
                    
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
        
        var existing = await _context.SourceDocuments
            .FirstOrDefaultAsync(d => d.HashSha256 == CATALOG_SHA256, cancellationToken);
        
        if (existing != null)
        {
            _logger.LogInformation("SourceDocument already exists: {Id}", existing.Id);
            return existing;
        }
        
        var fileInfo = new FileInfo(Path.Combine(basePath, "catalogs", "raw", "inali", "CLIN_completo.pdf"));
        var doc = new SourceDocument(
            title: CATALOG_TITLE,
            url: CATALOG_URL,
            hashSha256: CATALOG_SHA256,
            contentType: "application/pdf",
            sizeBytes: fileInfo.Length,
            retrievedAt: DateTime.UtcNow,
            totalPages: 256,
            catalogRange: "30-252",
            parserVersion: PARSER_VERSION
        );
        
        _context.SourceDocuments.Add(doc);
        await _context.SaveChangesAsync(cancellationToken);
        
        // Create SourcePages for catalog pages
        var pages = new List<SourcePage>();
        for (int i = 30; i <= 212; i++)
        {
            pages.Add(new SourcePage(
                sourceDocumentId: doc.Id,
                pageNumber: i,
                sectionReference: "MainCatalog",
                rawTextHash: null
            ));
        }
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

    private async Task<CatalogVersion> CreateCatalogVersionAsync(Guid catalogSourceId, Guid sourceDocumentId, CancellationToken cancellationToken)
    {
        var version = new CatalogVersion(
            catalogSourceId: catalogSourceId,
            versionNumber: "1",
            sourceDocumentHash: CATALOG_SHA256,
            retrievedAt: DateTime.UtcNow,
            familyCount: EXPECTED_FAMILIES,
            groupCount: EXPECTED_GROUPS,
            variantCount: EXPECTED_VARIANTS,
            autodenominationCount: EXPECTED_AUTODENOMINATIONS,
            parserVersion: PARSER_VERSION
        );
        
        _context.CatalogVersions.Add(version);
        await _context.SaveChangesAsync(cancellationToken);
        
        return version;
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

    private async Task<Dictionary<string, LanguageGroup>> ImportGroupsAsync(
        List<CatalogVariantDto> catalogData,
        List<Appendix4Dto> appendix4Data,
        Dictionary<string, LanguageFamily> familyMap,
        Guid catalogVersionId,
        Guid sourceDocumentId,
        CancellationToken cancellationToken)
    {
        var groupMap = new Dictionary<string, LanguageGroup>(StringComparer.OrdinalIgnoreCase);
        var groupKeyToFamily = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var groupKeyToPage = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        
        // Use appendix 4 as the canonical source for agrupación names
        // Build mapping from agrupación -> (family, page)
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
        
        _logger.LogInformation("Built agrupación map with {Count} unique agrupaciones", agrupacionToFamilia.Count);
        foreach (var kvp in agrupacionToFamilia.OrderBy(k => k.Key))
        {
            _logger.LogDebug("Agrupación: {Agrupacion} -> Familia: {Familia}", kvp.Key, kvp.Value);
        }
        
        // Build unique group key from appendix 4 agrupaciones
        foreach (var kvp in agrupacionToFamilia)
        {
            var agrupacion = kvp.Key;
            var familia = kvp.Value;
            var page = agrupacionToPage.GetValueOrDefault(agrupacion, 244);
            
            var key = $"{familia}|{agrupacion}";
            
            _logger.LogDebug("Agrupación: Familia={Familia}, Agrupacion={Agrupacion}, Key={Key}", 
                familia, agrupacion, key);
            
            if (!groupKeyToFamily.ContainsKey(key))
            {
                groupKeyToFamily[key] = familia;
                groupKeyToPage[key] = page;
            }
        }
        
        _logger.LogInformation("After loop: groupKeyToFamily.Count={Count}", groupKeyToFamily.Count);
        foreach (var kvp in groupKeyToFamily)
        {
            _logger.LogDebug("Group key: {Key} -> Family: {Family}", kvp.Key, kvp.Value);
        }
        
        _logger.LogInformation("Found {Count} unique groups in source data", groupKeyToFamily.Count);
        
        foreach (var kvp in groupKeyToFamily.OrderBy(k => k.Key))
        {
            var groupName = kvp.Key.Split('|')[1];
            var familyName = kvp.Value;
            var page = groupKeyToPage[kvp.Key];
            
            if (!familyMap.TryGetValue(familyName, out var family))
            {
                _logger.LogWarning("Family not found for group {Group}: {Family}", groupName, familyName);
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
        
        return groupMap;
    }

    private async Task<Dictionary<string, LanguageVariant>> ImportVariantsAsync(
        List<CatalogVariantDto> catalogData,
        List<Appendix4Dto> appendix4Data,
        Dictionary<string, LanguageGroup> groupMap,
        Guid catalogVersionId,
        Guid sourceDocumentId,
        CancellationToken cancellationToken)
    {
        var variantMap = new Dictionary<string, LanguageVariant>(StringComparer.OrdinalIgnoreCase);
        
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
            
            // Use appendix 4 agrupación if available, otherwise fall back to derivation
            string groupName;
            if (spanishNameToAgrupacion.TryGetValue(item.VariantName.Trim(), out var foundAgrupacion))
            {
                groupName = foundAgrupacion;
            }
            else
            {
                groupName = DeriveGroupName(item);
            }
            
            var groupKey = $"{item.Family.Trim()}|{groupName}";
            
            if (!groupMap.TryGetValue(groupKey, out var group))
            {
                _logger.LogWarning("Group not found for variant: {Key} (Variant={Variant}, Family={Family}, DerivedGroup={Group})", 
                    groupKey, item.VariantName, item.Family, groupName);
                continue;
            }
            
            var variant = new LanguageVariant(
                languageGroupId: group.Id,
                name: item.VariantName.Trim(),
                autodenomination: string.IsNullOrWhiteSpace(item.Autodenomination) ? null : item.Autodenomination.Trim(),
                iso639_3Code: null,
                inaliCode: null,
                description: null,
                writingSystem: null,
                orthography: null,
                source: CATALOG_TITLE
            );
            
            _context.LanguageVariants.Add(variant);
            await _context.SaveChangesAsync(cancellationToken);
            
            var variantKey = $"{groupKey}|{item.VariantName.Trim()}";
            variantMap[variantKey] = variant;
            
            // NOTE: Evidence creation skipped for Phase 1 catalog import.
            // The polymorphic Evidence table has FK constraints that conflict with LanguageVariant entityIds.
            // Provenance is maintained via SourceDocument -> SourcePage -> CatalogRecord chain.
            // Evidence will be used in later phases for linguistic data documentation.
            /*
            var sourceDoc = await _context.SourceDocuments.FirstAsync(d => d.HashSha256 == CATALOG_SHA256, cancellationToken);
            var evidence = new Evidence(
                sourceId: sourceDoc.Id,
                entityType: "LanguageVariant",
                entityId: variant.Id,
                quote: $"{item.VariantName} - {item.Family} - {groupName}",
                pageReference: item.Page.ToString(),
                sectionReference: item.Family,
                url: CATALOG_URL,
                notes: null,
                confidence: 1.0
            );
            _context.Evidence.Add(evidence);
            */
        }
        
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Imported {Count} variants", variantMap.Count);
        
        return variantMap;
    }

    private async Task<int> ImportAutodenominationsAsync(
        List<Appendix4Dto> appendix4Data,
        Dictionary<string, LanguageGroup> groupMap,
        Dictionary<string, LanguageVariant> variantMap,
        Guid catalogVersionId,
        Guid sourceDocumentId,
        CancellationToken cancellationToken)
    {
        int count = 0;
        var sourceDoc = await _context.SourceDocuments.FirstAsync(d => d.HashSha256 == CATALOG_SHA256, cancellationToken);
        
        // Track which (variantId, autodenomination) pairs we've already added to avoid DB unique constraint violation
        // Use exact strings to match DB collation behavior
        var addedPairs = new HashSet<string>();
        
        foreach (var item in appendix4Data)
        {
            if (string.IsNullOrWhiteSpace(item.Autodenom) || string.IsNullOrWhiteSpace(item.Agrupacion)) continue;
            
            // Find matching group
            var groupKey = groupMap.Keys.FirstOrDefault(k => 
                k.EndsWith($"|{item.Agrupacion.Trim()}", StringComparison.OrdinalIgnoreCase));
            
            if (groupKey == null)
            {
                _logger.LogDebug("Group not found for autodenomination: {Group}", item.Agrupacion);
                continue;
            }
            
            var group = groupMap[groupKey];
            
            // Find matching variant in the group using SpanishName
            Guid? variantId = null;
            var groupVariants = group.LanguageVariants.ToList();
            
            if (!string.IsNullOrWhiteSpace(item.SpanishName) && item.SpanishName.Trim() != "Nombre en español")
            {
                var spanishName = item.SpanishName.Trim();
                
                // First try exact match
                var exactMatch = groupVariants.FirstOrDefault(v => 
                    v.Name.Equals(spanishName, StringComparison.OrdinalIgnoreCase));
                
                if (exactMatch != null)
                {
                    variantId = exactMatch.Id;
                }
                else
                {
                    // Try prefix matching: appendix name is a prefix of variant name
                    // Appendix names often end with comma and catalog continues with location details
                    var prefixMatches = groupVariants
                        .Where(v => v.Name.StartsWith(spanishName, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    
                    if (prefixMatches.Count == 1)
                    {
                        // Exactly one variant starts with this prefix - safe to match
                        variantId = prefixMatches[0].Id;
                    }
                    else if (prefixMatches.Count > 1)
                    {
                        // Multiple prefix matches - check if the spanishName is a "generic" name ending with comma
                        // If it ends with comma, it's likely a generic group name, not a specific variant
                        // Don't match to avoid misattribution
                        if (spanishName.EndsWith(","))
                        {
                            _logger.LogWarning("Generic SpanishName (ends with comma) matches multiple variants: Autodenom={Autodenom}, SpanishName={SpanishName}, Matches={Count}. Skipping to avoid misattribution.", 
                                item.Autodenom, spanishName, prefixMatches.Count);
                            continue;
                        }
                        
                        // Multiple prefix matches - try to find the best one
                        // After the appendix prefix, catalog typically has space or comma+space
                        var bestMatch = prefixMatches.FirstOrDefault(v => 
                            v.Name.Length > spanishName.Length && 
                            (v.Name[spanishName.Length] == ' ' || v.Name[spanishName.Length] == ','));
                        
                        if (bestMatch != null)
                        {
                            variantId = bestMatch.Id;
                        }
                        else
                        {
                            _logger.LogWarning("Ambiguous prefix match for autodenomination: Autodenom={Autodenom}, SpanishName={SpanishName}, Matches={Count}. Skipping to avoid misattribution.", 
                                item.Autodenom, spanishName, prefixMatches.Count);
                            continue;
                        }
                    }
                    else
                    {
                        // No prefix match - try suffix matching (variant name is prefix of appendix name)
                        var suffixMatch = groupVariants.FirstOrDefault(v => 
                            spanishName.StartsWith(v.Name, StringComparison.OrdinalIgnoreCase) &&
                            (spanishName.Length == v.Name.Length || spanishName[v.Name.Length] == ',' || spanishName[v.Name.Length] == ' '));
                        
                        if (suffixMatch != null)
                        {
                            variantId = suffixMatch.Id;
                        }
                        else
                        {
                            _logger.LogWarning("No variant match for autodenomination: Autodenom={Autodenom}, SpanishName={SpanishName}, GroupVariants={Count}. Skipping to avoid misattribution.", 
                                item.Autodenom, spanishName, groupVariants.Count);
                            continue;
                        }
                    }
                }
            }
            
            if (variantId == null && groupVariants.Count == 1)
            {
                // Only one variant in this group - use it
                variantId = groupVariants[0].Id;
            }
            else if (variantId == null)
            {
                // Multiple variants in group but no clear match - skip to avoid misattribution
                _logger.LogWarning("Cannot uniquely match autodenomination to variant: Autodenom={Autodenom}, SpanishName={SpanishName}, Agrupacion={Agrupacion}, GroupVariants={Count}", 
                    item.Autodenom, item.SpanishName, item.Agrupacion, groupVariants.Count);
                continue;
            }
            
            if (variantId == Guid.Empty)
            {
                _logger.LogWarning("No variant available for autodenomination: {Autodenom}", item.Autodenom);
                continue;
            }
            
            // Create unique key for DB unique constraint: (LanguageVariantId, Autodenomination)
            // Use exact strings to match DB collation behavior
            var autodenomKey = $"{variantId}|{item.Autodenom.Trim()}";
            
            // Special debug for 'dizé' and 'dizë'
            if (item.Autodenom.Trim() == "dizé" || item.Autodenom.Trim() == "dizë")
            {
                _logger.LogDebug("DIZ DEBUG: variantId={VariantId}, autodenomKey={Key}, addedPairs.Contains={Contains}, SpanishName={SpanishName}", variantId, autodenomKey, addedPairs.Contains(autodenomKey), item.SpanishName);
            }
            
            if (addedPairs.Contains(autodenomKey))
            {
                _logger.LogDebug("Skipping duplicate autodenomination in batch for variant {VariantId}: '{Autodenom}' (Key: '{Key}')", variantId, item.Autodenom, autodenomKey);
                continue;
            }
            
            // Also check DB for existing entry (in case of re-runs)
            var existsInDb = await _context.LanguageVariantAutodenominations
                .AnyAsync(a => a.LanguageVariantId == variantId && a.Autodenomination == item.Autodenom.Trim(), cancellationToken);
            
            // Also check change tracker for uncommitted entities
            var existsInTracker = _context.ChangeTracker.Entries<LanguageVariantAutodenomination>()
                .Any(e => e.Entity.LanguageVariantId == variantId && e.Entity.Autodenomination == item.Autodenom.Trim() && e.State != EntityState.Detached);
            
            if (existsInDb || existsInTracker)
            {
                _logger.LogDebug("Skipping existing autodenomination (DB or tracker) for variant {VariantId}: '{Autodenom}'", variantId, item.Autodenom);
                addedPairs.Add(autodenomKey);
                continue;
            }
            
            addedPairs.Add(autodenomKey);
            
            _logger.LogDebug("Adding autodenomination: VariantId={VariantId}, Autodenom='{Autodenom}', SpanishName={SpanishName}, Agrupacion={Agrupacion}, Page={Page}, Key='{Key}'", variantId, item.Autodenom, item.SpanishName, item.Agrupacion, item.Page, autodenomKey);
            
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
        
        await _context.SaveChangesAsync(cancellationToken);
        return count;
    }

    private void ValidateBaselineCounts(ImportResult result)
    {
        var errors = new List<string>();
        
        if (result.FamiliesImported != EXPECTED_FAMILIES)
            errors.Add($"Families: expected {EXPECTED_FAMILIES}, got {result.FamiliesImported}");
        
        if (result.GroupsImported != EXPECTED_GROUPS)
            errors.Add($"Groups: expected {EXPECTED_GROUPS}, got {result.GroupsImported}");
        
        if (result.VariantsImported != EXPECTED_VARIANTS)
            errors.Add($"Variants: expected {EXPECTED_VARIANTS}, got {result.VariantsImported}");
        
        if (result.AutodenominationsImported != EXPECTED_AUTODENOMINATIONS)
            errors.Add($"Autodenominations: expected {EXPECTED_AUTODENOMINATIONS}, got {result.AutodenominationsImported}");
        
        if (errors.Any())
        {
            result.ValidationErrors = errors;
            throw new InvalidOperationException($"BASELINE VALIDATION FAILED:\n{string.Join("\n", errors)}");
        }
        
        _logger.LogInformation("Baseline validation PASSED: {Families}/{Groups}/{Variants}/{Autodenoms}", 
            result.FamiliesImported, result.GroupsImported, result.VariantsImported, result.AutodenominationsImported);
    }

    private async Task CreateCatalogRecordsAsync(
        Guid catalogVersionId,
        Dictionary<string, LanguageFamily> familyMap,
        Dictionary<string, LanguageGroup> groupMap,
        Dictionary<string, LanguageVariant> variantMap,
        Guid sourceDocumentId,
        CancellationToken cancellationToken)
    {
        var records = new List<CatalogRecord>();
        
        // Family records
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
                sourcePage: 30,
                sourceSection: "Index",
                extractionDate: DateTime.UtcNow,
                parserVersion: PARSER_VERSION
            ));
        }
        
        // Group records
        foreach (var kvp in groupMap)
        {
            records.Add(new CatalogRecord(
                catalogVersionId: catalogVersionId,
                entityType: "LanguageGroup",
                identityKey: $"group|{kvp.Key}",
                inaliCode: "",
                iso639_3Code: "",
                name: kvp.Key.Split('|')[1],
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
        
        // Variant records
        foreach (var kvp in variantMap)
        {
            records.Add(new CatalogRecord(
                catalogVersionId: catalogVersionId,
                entityType: "LanguageVariant",
                identityKey: $"variant|{kvp.Key}",
                inaliCode: "",
                iso639_3Code: "",
                name: kvp.Value.Name,
                autodenomination: kvp.Value.Autodenomination,
                description: null,
                geoReference: null,
                sourceDocumentId: sourceDocumentId,
                sourceHash: CATALOG_SHA256,
                sourceUrl: CATALOG_URL,
                sourcePage: 30,
                sourceSection: "Catalog",
                extractionDate: DateTime.UtcNow,
                parserVersion: PARSER_VERSION
            ));
        }
        
        _context.CatalogRecords.AddRange(records);
        await _context.SaveChangesAsync(cancellationToken);
    }

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