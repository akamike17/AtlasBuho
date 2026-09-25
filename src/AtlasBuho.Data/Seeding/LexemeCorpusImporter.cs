using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AtlasBuho.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AtlasBuho.Data.Seeding;

/// <summary>
/// Phase 2 — Lexeme / Corpus Ingestion.
/// Controlled ingestion of lexical corpus into Lexemes, Meanings and LexicalEquivalence evidence.
/// Respects 6B invariants: I1 canonical, I2 directionality, I3 provenance, I4 variant isolation,
/// I5 dataset reproducibility, I6 completed version immutability.
/// </summary>
public interface ILexemeCorpusImporter
{
    Task<LexemeImportResult> ImportCorpusAsync(LexemeCorpus corpus, CancellationToken cancellationToken = default);
}

public class LexemeCorpus
{
    public required string VersionNumber { get; init; }
    public required string SourceName { get; init; }
    public required string SourceLevel { get; init; } // SourceLevel enum name
    public string? SourceDescription { get; init; }
    public string? SourceInstitution { get; init; }
    public string? SourceUrl { get; init; }
    public required string LanguageVariantName { get; init; }
    public required List<LexemeCorpusRecord> Records { get; init; }
}

public class LexemeCorpusRecord
{
    public required string CanonicalForm { get; init; }
    public string? SpanishMeaning { get; init; }
    public string? EnglishMeaning { get; init; }
    public string? PartOfSpeech { get; init; }
    public string? SemanticDomain { get; init; }
    public string? Register { get; init; }
    public string? RegionalNotes { get; init; }
    public string? Etymology { get; init; }
    public string? SourceReference { get; init; }
    public required string VerificationStatus { get; init; } // VerificationStatus enum name
    public double Confidence { get; init; }
    public bool IsCanonical { get; init; }
    public string? TargetTextEs { get; init; } // explicit Spanish translation evidence
    public string? TargetTextEn { get; init; } // explicit English translation evidence
}

public class LexemeImportResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int LexemesCreated { get; set; }
    public int LexemesExisting { get; set; }
    public int MeaningsCreated { get; set; }
    public int MeaningsExisting { get; set; }
    public int EquivalencesCreated { get; set; }
    public int EquivalencesExisting { get; set; }
    public int RecordsQuarantined { get; set; }
    public List<string> ValidationErrors { get; set; } = new();
    public Dictionary<string, object> Provenance { get; set; } = new();
    public List<LexemeImportRecordResult> RecordResults { get; set; } = new();
}

public class LexemeImportRecordResult
{
    public required string CanonicalForm { get; set; }
    public required LexemeImportOutcome Outcome { get; set; }
    public string? Reason { get; set; }
    public Guid? LexemeId { get; set; }
    public bool LexemeWasCreated { get; set; }
    public Guid? MeaningId { get; set; }
    public bool MeaningWasCreated { get; set; }
    public List<Guid> EquivalenceIds { get; set; } = new();
}

public enum LexemeImportOutcome
{
    Accepted,
    Quarantined,
    Rejected
}

public class LexemeCorpusImporter : ILexemeCorpusImporter
{
    private readonly AtlasBuhoDbContext _context;
    private readonly ILogger<LexemeCorpusImporter> _logger;

    public LexemeCorpusImporter(AtlasBuhoDbContext context, ILogger<LexemeCorpusImporter> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<LexemeImportResult> ImportCorpusAsync(LexemeCorpus corpus, CancellationToken cancellationToken = default)
    {
        var result = new LexemeImportResult();
        var strategy = _context.Database.CreateExecutionStrategy();

        try
        {
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    // Step 1: Validate corpus structure
                    ValidateCorpus(corpus, result);
                    if (result.ValidationErrors.Count > 0)
                    {
                        result.Success = false;
                        return result;
                    }

                    // Step 2: Resolve CatalogVersion (Pending or create new)
                    var catalogVersion = await GetOrCreateCatalogVersionAsync(corpus.VersionNumber, cancellationToken);
                    var isExistingCompleted = catalogVersion.ImportStatus == "Completed";
                    result.Provenance["CatalogVersionId"] = catalogVersion.Id;
                    result.Provenance["CatalogVersionNumber"] = catalogVersion.VersionNumber;

                    // Step 3: Resolve LanguageVariant (must exist from Phase 1)
                    var variant = await ResolveLanguageVariantAsync(corpus.LanguageVariantName, cancellationToken);
                    if (variant == null)
                    {
                        result.Success = false;
                        result.ErrorMessage = $"LanguageVariant '{corpus.LanguageVariantName}' not found. Import Phase 1 catalog first.";
                        return result;
                    }
                    result.Provenance["LanguageVariantId"] = variant.Id;
                    result.Provenance["LanguageVariantName"] = variant.Name;

                    // Step 4: Create or get Source
                    var source = await GetOrCreateSourceAsync(corpus, cancellationToken);
                    result.Provenance["SourceId"] = source.Id;

                    // Step 5: Process each record through controlled pipeline
                    foreach (var record in corpus.Records)
                    {
                        var recordResult = await ProcessRecordAsync(record, variant, source, catalogVersion, cancellationToken);
                        result.RecordResults.Add(recordResult);

                        switch (recordResult.Outcome)
                        {
                            case LexemeImportOutcome.Accepted:
                                if (recordResult.LexemeWasCreated) result.LexemesCreated++;
                                if (recordResult.MeaningWasCreated) result.MeaningsCreated++;
                                result.EquivalencesCreated += recordResult.EquivalenceIds.Count;
                                break;
                            case LexemeImportOutcome.Quarantined:
                                result.RecordsQuarantined++;
                                break;
                            case LexemeImportOutcome.Rejected:
                                result.ValidationErrors.Add($"Record '{record.CanonicalForm}': {recordResult.Reason}");
                                break;
                        }
                    }

                    // Step 6: Validate canonical invariant (I1)
                    await ValidateCanonicalInvariantAsync(catalogVersion.Id, cancellationToken);

                    // Step 7: Mark version as Completed ONLY if not already completed (I6)
                    if (!isExistingCompleted)
                    {
                        await MarkCompletedAsync(catalogVersion, cancellationToken);
                    }
                    await transaction.CommitAsync(cancellationToken);

                    result.Success = true;
                    _logger.LogInformation(
                        "Lexeme corpus ingestion completed: {Lexemes} lexemes, {Meanings} meanings, {Equivalences} equivalences, {Quarantined} quarantined",
                        result.LexemesCreated, result.MeaningsCreated, result.EquivalencesCreated, result.RecordsQuarantined);

                    return result;
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lexeme corpus ingestion failed");
            result.Success = false;
            result.ErrorMessage = $"{ex.Message} | Inner: {ex.InnerException?.Message}";
            return result;
        }
    }

    private void ValidateCorpus(LexemeCorpus corpus, LexemeImportResult result)
    {
        if (string.IsNullOrWhiteSpace(corpus.VersionNumber))
            result.ValidationErrors.Add("VersionNumber is required");
        if (string.IsNullOrWhiteSpace(corpus.SourceName))
            result.ValidationErrors.Add("SourceName is required");
        if (string.IsNullOrWhiteSpace(corpus.LanguageVariantName))
            result.ValidationErrors.Add("LanguageVariantName is required");
        if (corpus.Records == null || corpus.Records.Count == 0)
            result.ValidationErrors.Add("At least one record is required");
    }

    private async Task<CatalogVersion> GetOrCreateCatalogVersionAsync(string versionNumber, CancellationToken cancellationToken)
    {
        // Look for existing Pending version first
        var existing = await _context.CatalogVersions
            .FirstOrDefaultAsync(v => v.VersionNumber == versionNumber && v.ImportStatus == "Pending", cancellationToken);

        if (existing != null) return existing;

        // Also check if already Completed (idempotency for same version)
        var completed = await _context.CatalogVersions
            .FirstOrDefaultAsync(v => v.VersionNumber == versionNumber && v.ImportStatus == "Completed", cancellationToken);

        if (completed != null)
        {
            // Detach to prevent EF from tracking modifications to immutable version
            _context.Entry(completed).State = EntityState.Detached;
            return completed;
        }

        // Create new CatalogVersion with Pending status (will be marked Completed after validation)
        var catalogSource = await _context.CatalogSources
            .FirstOrDefaultAsync(s => s.Name.StartsWith("INALI"), cancellationToken)
            ?? await CreateDefaultCatalogSourceAsync(cancellationToken);

        var version = new CatalogVersion(
            catalogSourceId: catalogSource.Id,
            versionNumber: versionNumber,
            sourceDocumentHash: ComputeHash(versionNumber + DateTime.UtcNow.ToString("O")),
            retrievedAt: DateTime.UtcNow,
            familyCount: 0,
            groupCount: 0,
            variantCount: 0,
            autodenominationCount: 0,
            parserVersion: "2026.09.25-phase2-lexeme-ingestion",
            importStatus: "Pending");

        _context.CatalogVersions.Add(version);
        await _context.SaveChangesAsync(cancellationToken);
        return version;
    }

    /// <summary>
    /// Marks the CatalogVersion as Completed IFF all validations passed.
    /// Must be called before any trigger-protected mutations would be rejected.
    /// </summary>
    private async Task<CatalogVersion> MarkCompletedAsync(CatalogVersion version, CancellationToken cancellationToken)
    {
        version.UpdateStatus("Completed");
        await _context.SaveChangesAsync(cancellationToken);
        return version;
    }

    private async Task<CatalogSource> CreateDefaultCatalogSourceAsync(CancellationToken cancellationToken)
    {
        var source = new CatalogSource(
            name: "INALI",
            description: "Instituto Nacional de Lenguas Indígenas",
            baseUrl: "https://www.inali.gob.mx");
        _context.CatalogSources.Add(source);
        await _context.SaveChangesAsync(cancellationToken);
        return source;
    }

    private async Task<LanguageVariant?> ResolveLanguageVariantAsync(string variantName, CancellationToken cancellationToken)
    {
        // I4: exact name match only, never partial or first-candidate
        return await _context.LanguageVariants
            .FirstOrDefaultAsync(v => v.Name == variantName, cancellationToken);
    }

    private async Task<Source> GetOrCreateSourceAsync(LexemeCorpus corpus, CancellationToken cancellationToken)
    {
        var level = Enum.Parse<SourceLevel>(corpus.SourceLevel);
        var existing = await _context.Sources
            .FirstOrDefaultAsync(s => s.Name == corpus.SourceName && s.Level == level, cancellationToken);

        if (existing != null) return existing;

        var source = new Source(
            name: corpus.SourceName,
            level: level,
            description: corpus.SourceDescription,
            institution: corpus.SourceInstitution,
            url: corpus.SourceUrl,
            doi: null,
            isbn: null,
            issn: null,
            publicationDate: null,
            authors: null,
            editors: null,
            publisher: null,
            location: null,
            language: null,
            notes: null);

        _context.Sources.Add(source);
        await _context.SaveChangesAsync(cancellationToken);
        return source;
    }

    private async Task<LexemeImportRecordResult> ProcessRecordAsync(
        LexemeCorpusRecord record,
        LanguageVariant variant,
        Source source,
        CatalogVersion catalogVersion,
        CancellationToken cancellationToken)
    {
        var result = new LexemeImportRecordResult
        {
            CanonicalForm = record.CanonicalForm,
            Outcome = LexemeImportOutcome.Rejected
        };

        try
        {
            // Validate record
            if (string.IsNullOrWhiteSpace(record.CanonicalForm))
            {
                result.Outcome = LexemeImportOutcome.Quarantined;
                result.Reason = "CanonicalForm is empty";
                return result;
            }

            // I4: variant isolation — resolve or create lexeme within this specific variant
            var lexeme = await _context.Lexemes
                .FirstOrDefaultAsync(l => l.LanguageVariantId == variant.Id && l.CanonicalForm == record.CanonicalForm, cancellationToken);

            if (lexeme == null)
            {
                lexeme = new Lexeme(
                    languageVariantId: variant.Id,
                    canonicalForm: record.CanonicalForm,
                    alternativeForms: null,
                    autodenomination: null,
                    spanishMeaning: record.SpanishMeaning,
                    partOfSpeech: record.PartOfSpeech,
                    pronunciationIpa: null,
                    pronunciationReadable: null,
                    semanticDomain: record.SemanticDomain,
                    register: record.Register,
                    regionalNotes: record.RegionalNotes,
                    etymology: record.Etymology,
                    source: record.SourceReference,
                    verificationStatus: ParseVerificationStatus(record.VerificationStatus),
                    confidence: record.Confidence);

                _context.Lexemes.Add(lexeme);
                await _context.SaveChangesAsync(cancellationToken);
                result.LexemeId = lexeme.Id;
                result.LexemeWasCreated = true;
            }
            else
            {
                result.LexemeId = lexeme.Id;
                result.LexemeWasCreated = false;
            }

            // Create Meaning if Spanish/English meaning provided
            if (!string.IsNullOrWhiteSpace(record.SpanishMeaning) || !string.IsNullOrWhiteSpace(record.EnglishMeaning))
            {
                var existingMeaning = await _context.Meanings
                    .FirstOrDefaultAsync(m => m.LexemeId == lexeme.Id &&
                        m.SpanishMeaning == record.SpanishMeaning &&
                        m.EnglishMeaning == record.EnglishMeaning, cancellationToken);

                if (existingMeaning == null)
                {
                    var meaning = new Meaning(
                        lexemeId: lexeme.Id,
                        spanishMeaning: record.SpanishMeaning ?? string.Empty,
                        englishMeaning: record.EnglishMeaning,
                        partOfSpeech: record.PartOfSpeech,
                        semanticDomain: record.SemanticDomain,
                        register: record.Register,
                        regionalNotes: record.RegionalNotes,
                        source: record.SourceReference,
                        verificationStatus: ParseVerificationStatus(record.VerificationStatus),
                        confidence: record.Confidence,
                        order: 0);

                    _context.Meanings.Add(meaning);
                    await _context.SaveChangesAsync(cancellationToken);
                    result.MeaningId = meaning.Id;
                    result.MeaningWasCreated = true;
                }
                else
                {
                    result.MeaningId = existingMeaning.Id;
                    result.MeaningWasCreated = false;
                }
            }

            // Create LexicalEquivalence evidence for explicit translations
            if (!string.IsNullOrWhiteSpace(record.TargetTextEs))
            {
                var eqId = await CreateLexicalEquivalenceAsync(
                    lexeme.Id, "es", record.TargetTextEs, record.IsCanonical,
                    record.VerificationStatus, source.Id, catalogVersion.Id, cancellationToken);
                if (eqId.HasValue) result.EquivalenceIds.Add(eqId.Value);
            }

            if (!string.IsNullOrWhiteSpace(record.TargetTextEn))
            {
                var eqId = await CreateLexicalEquivalenceAsync(
                    lexeme.Id, "en", record.TargetTextEn, record.IsCanonical,
                    record.VerificationStatus, source.Id, catalogVersion.Id, cancellationToken);
                if (eqId.HasValue) result.EquivalenceIds.Add(eqId.Value);
            }

            result.Outcome = LexemeImportOutcome.Accepted;
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to process record {CanonicalForm}", record.CanonicalForm);
            result.Outcome = LexemeImportOutcome.Quarantined;
            result.Reason = ex.Message;
            return result;
        }
    }

    private async Task<Guid?> CreateLexicalEquivalenceAsync(
        Guid lexemeId, string targetLanguage, string targetText, bool isCanonical,
        string verificationStatus, Guid sourceId, Guid catalogVersionId,
        CancellationToken cancellationToken)
    {
        // Check for existing equivalence (I5: idempotency)
        var existing = await _context.LexicalEquivalences
            .FirstOrDefaultAsync(e => e.SourceLexemeId == lexemeId &&
                e.TargetLanguage == targetLanguage &&
                e.TargetText == targetText &&
                e.CatalogVersionId == catalogVersionId, cancellationToken);

        if (existing != null) return null; // existing, not new

        var equivalence = new LexicalEquivalence(
            sourceLexemeId: lexemeId,
            targetLanguage: targetLanguage,
            targetText: targetText,
            isCanonical: isCanonical,
            verificationStatus: ParseVerificationStatus(verificationStatus),
            sourceId: sourceId,
            catalogVersionId: catalogVersionId);

        _context.LexicalEquivalences.Add(equivalence);
        await _context.SaveChangesAsync(cancellationToken);
        return equivalence.Id;
    }

    private async Task ValidateCanonicalInvariantAsync(Guid catalogVersionId, CancellationToken cancellationToken)
    {
        // I1: at most one canonical per (SourceLexemeId, TargetLanguage, CatalogVersionId)
        var violations = await _context.LexicalEquivalences
            .Where(e => e.CatalogVersionId == catalogVersionId && e.IsCanonical)
            .GroupBy(e => new { e.SourceLexemeId, e.TargetLanguage })
            .Where(g => g.Count() > 1)
            .Select(g => new { g.Key.SourceLexemeId, g.Key.TargetLanguage, Count = g.Count() })
            .ToListAsync(cancellationToken);

        if (violations.Count > 0)
        {
            throw new InvalidOperationException(
                $"I1 canonical invariant violated: {violations.Count} lexemes have multiple canonical equivalences in this version. " +
                $"First violation: Lexeme {violations[0].SourceLexemeId}, Language {violations[0].TargetLanguage}, Count {violations[0].Count}");
        }
    }

    private static VerificationStatus ParseVerificationStatus(string status)
    {
        return Enum.TryParse<VerificationStatus>(status, true, out var parsed)
            ? parsed
            : VerificationStatus.Unknown;
    }

    private static string ComputeHash(string input)
    {
        using var sha256 = SHA256.Create();
        return Convert.ToHexString(sha256.ComputeHash(Encoding.UTF8.GetBytes(input))).ToLowerInvariant();
    }
}
