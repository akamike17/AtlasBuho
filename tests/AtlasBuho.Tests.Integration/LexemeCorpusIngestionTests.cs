using AtlasBuho.Data;
using AtlasBuho.Data.Seeding;
using AtlasBuho.Data.Translation;
using AtlasBuho.Application.Translation;
using AtlasBuho.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Xunit;

namespace AtlasBuho.Tests.Integration;

/// <summary>
/// Phase 2 — Lexeme/Corpus Ingestion integration tests.
/// Verifies controlled ingestion against real MySQL with real migration-created schema.
/// </summary>
public class LexemeCorpusIngestionTests : IAsyncLifetime
{
    private readonly string _dbName = $"atlasbuho_test_lexeme_{Guid.NewGuid():N}";
    private AtlasBuhoDbContext _context = null!;
    private LexemeCorpusImporter _importer = null!;

    public async Task InitializeAsync()
    {
        var password = Environment.GetEnvironmentVariable("ATLASBUHO_TEST_DB_PASSWORD")
            ?? throw new InvalidOperationException("ATLASBUHO_TEST_DB_PASSWORD environment variable not set");

        var connectionString = $"Server=127.0.0.1;Port=3306;Database={_dbName};User=Admin;Password={password};AllowLoadLocalInfile=true";

        var options = new DbContextOptionsBuilder<AtlasBuhoDbContext>()
            .UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 46)))
            .Options;

        _context = new AtlasBuhoDbContext(options);
        await _context.Database.EnsureDeletedAsync();
        await _context.Database.MigrateAsync();

        _importer = new LexemeCorpusImporter(_context, new LoggerFactory().CreateLogger<LexemeCorpusImporter>());
    }

    public async Task DisposeAsync()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
    }

    private async Task<(LanguageVariant variant, CatalogSource catalogSource)> SeedPhase1DataAsync()
    {
        var familyId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var variantId = Guid.NewGuid();

        _context.Database.ExecuteSqlRaw(
            @"INSERT INTO LanguageFamilies (Id, Name, NameEnglish, CreatedAt, UpdatedAt)
              VALUES (@p0, @p1, 'Test Family EN', UTC_TIMESTAMP(6), UTC_TIMESTAMP(6))",
            familyId.ToString(), $"Test Family {Guid.NewGuid():N}");

        _context.Database.ExecuteSqlRaw(
            @"INSERT INTO LanguageGroups (Id, LanguageFamilyId, Name, CreatedAt, UpdatedAt)
              VALUES (@p0, @p1, @p2, UTC_TIMESTAMP(6), UTC_TIMESTAMP(6))",
            groupId.ToString(), familyId.ToString(), $"Test Group {Guid.NewGuid():N}");

        _context.Database.ExecuteSqlRaw(
            @"INSERT INTO LanguageVariants (Id, LanguageGroupId, Name, CreatedAt, UpdatedAt)
              VALUES (@p0, @p1, @p2, UTC_TIMESTAMP(6), UTC_TIMESTAMP(6))",
            variantId.ToString(), groupId.ToString(), $"Test Variant {Guid.NewGuid():N}");

        var catalogSourceId = Guid.NewGuid();
        _context.Database.ExecuteSqlRaw(
            @"INSERT INTO CatalogSources (Id, Name, Description, BaseUrl, IsActive, CreatedAt, UpdatedAt)
              VALUES (@p0, @p1, 'Instituto Nacional de Lenguas Indígenas', 'https://www.inali.gob.mx', 1, UTC_TIMESTAMP(6), UTC_TIMESTAMP(6))",
            catalogSourceId.ToString(), $"INALI-{Guid.NewGuid():N}");

        var variant = await _context.LanguageVariants.FindAsync(variantId);
        var catalogSource = await _context.CatalogSources.FindAsync(catalogSourceId);
        return (variant!, catalogSource!);
    }

    [Fact]
    public async Task ValidRecord_IsAcceptedAndCreatesLexemeMeaningEquivalence()
    {
        // Arrange
        var (variant, _) = await SeedPhase1DataAsync();
        var importer = _importer;

        var corpus = new LexemeCorpus
        {
            VersionNumber = $"v1.0-test-{Guid.NewGuid():N}",
            SourceName = "Test Dictionary",
            SourceLevel = "AcademicMaterial",
            LanguageVariantName = variant.Name,
            Records = new List<LexemeCorpusRecord>
            {
                new()
                {
                    CanonicalForm = "bene xono",
                    SpanishMeaning = "zorro",
                    TargetTextEs = "zorro",
                    VerificationStatus = "Verified",
                    IsCanonical = true,
                    Confidence = 1.0
                }
            }
        };

        // Act
        var result = await importer.ImportCorpusAsync(corpus);

        // Assert
        Assert.True(result.Success, result.ErrorMessage);
        Assert.Equal(1, result.LexemesCreated);
        Assert.Equal(1, result.MeaningsCreated);
        Assert.Equal(1, result.EquivalencesCreated);
        Assert.Equal(0, result.RecordsQuarantined);

        // Verify in DB
        var lexeme = await _context.Lexemes.FirstOrDefaultAsync(l => l.CanonicalForm == "bene xono");
        Assert.NotNull(lexeme);
        Assert.Equal(variant.Id, lexeme!.LanguageVariantId);

        var meaning = await _context.Meanings.FirstOrDefaultAsync(m => m.LexemeId == lexeme.Id);
        Assert.NotNull(meaning);
        Assert.Equal("zorro", meaning!.SpanishMeaning);

        var equivalence = await _context.LexicalEquivalences.FirstOrDefaultAsync(e => e.SourceLexemeId == lexeme.Id);
        Assert.NotNull(equivalence);
        Assert.Equal("es", equivalence!.TargetLanguage);
        Assert.True(equivalence.IsCanonical);
        Assert.Equal(VerificationStatus.Verified, equivalence.VerificationStatus);
    }

    [Fact]
    public async Task SameFormDifferentVariants_AreIsolated()
    {
        // Arrange: create second variant with same form
        var (variantA, _) = await SeedPhase1DataAsync();
        var groupBId = Guid.NewGuid();
        var variantBId = Guid.NewGuid();
        var variantBName = $"Test Variant B {Guid.NewGuid():N}";

        // Get the family from variantA's group
        var groupA = await _context.LanguageGroups.FindAsync(variantA.LanguageGroupId);
        var familyAId = groupA!.LanguageFamilyId;

        _context.Database.ExecuteSqlRaw(
            @"INSERT INTO LanguageGroups (Id, LanguageFamilyId, Name, CreatedAt, UpdatedAt)
              VALUES (@p0, @p1, @p2, UTC_TIMESTAMP(6), UTC_TIMESTAMP(6))",
            groupBId.ToString(), familyAId.ToString(), $"Test Group B {Guid.NewGuid():N}");

        _context.Database.ExecuteSqlRaw(
            @"INSERT INTO LanguageVariants (Id, LanguageGroupId, Name, CreatedAt, UpdatedAt)
              VALUES (@p0, @p1, @p2, UTC_TIMESTAMP(6), UTC_TIMESTAMP(6))",
            variantBId.ToString(), groupBId.ToString(), variantBName);

        var importer = _importer;

        // Act: same canonical form in two variants
        var corpusA = new LexemeCorpus
        {
            VersionNumber = $"v1.0-A-{Guid.NewGuid():N}",
            SourceName = "Dict A",
            SourceLevel = "AcademicMaterial",
            LanguageVariantName = variantA.Name,
            Records = new List<LexemeCorpusRecord>
            {
                new() { CanonicalForm = "tsi", SpanishMeaning = "casa", TargetTextEs = "casa", VerificationStatus = "Verified", IsCanonical = true }
            }
        };

        var corpusB = new LexemeCorpus
        {
            VersionNumber = $"v1.0-B-{Guid.NewGuid():N}",
            SourceName = "Dict B",
            SourceLevel = "AcademicMaterial",
            LanguageVariantName = variantBName,
            Records = new List<LexemeCorpusRecord>
            {
                new() { CanonicalForm = "tsi", SpanishMeaning = "perro", TargetTextEs = "perro", VerificationStatus = "Verified", IsCanonical = true }
            }
        };

        var resultA = await importer.ImportCorpusAsync(corpusA);
        var resultB = await importer.ImportCorpusAsync(corpusB);

        // Assert: two lexemes, isolated by variant
        Assert.True(resultA.Success);
        Assert.True(resultB.Success);
        Assert.Equal(1, resultA.LexemesCreated);
        Assert.Equal(1, resultB.LexemesCreated);

        var lexemes = await _context.Lexemes.Where(l => l.CanonicalForm == "tsi").ToListAsync();
        Assert.Equal(2, lexemes.Count);
        Assert.Contains(lexemes, l => l.LanguageVariantId == variantA.Id);
        Assert.Contains(lexemes, l => l.LanguageVariantId == variantBId);
    }

    [Fact]
    public async Task SameTargetDifferentSources_ProduceBothEvidenceRecords()
    {
        // Arrange
        var (variant, _) = await SeedPhase1DataAsync();
        var importer = _importer;

        var corpus = new LexemeCorpus
        {
            VersionNumber = $"v1.0-prov-{Guid.NewGuid():N}",
            SourceName = "Dict X",
            SourceLevel = "AcademicMaterial",
            LanguageVariantName = variant.Name,
            Records = new List<LexemeCorpusRecord>
            {
                new() { CanonicalForm = "tsi", SpanishMeaning = "casa", TargetTextEs = "casa", VerificationStatus = "Verified", IsCanonical = true },
                new() { CanonicalForm = "tsi", SpanishMeaning = "hogar", TargetTextEs = "hogar", VerificationStatus = "Documented", IsCanonical = false }
            }
        };

        // Act
        var result = await importer.ImportCorpusAsync(corpus);

        // Assert: both evidence records survive (I3)
        Assert.True(result.Success);
        Assert.Equal(1, result.LexemesCreated); // same lexeme
        Assert.Equal(2, result.MeaningsCreated); // two meanings
        Assert.Equal(2, result.EquivalencesCreated); // two equivalence rows

        var equivalences = await _context.LexicalEquivalences
            .Where(e => e.SourceLexeme!.CanonicalForm == "tsi")
            .ToListAsync();
        Assert.Equal(2, equivalences.Count);
        Assert.Contains(equivalences, e => e.TargetText == "casa" && e.IsCanonical);
        Assert.Contains(equivalences, e => e.TargetText == "hogar" && !e.IsCanonical);
    }

    [Fact]
    public async Task Directionality_ForwardDoesNotGenerateReverse()
    {
        // Arrange
        var (variant, _) = await SeedPhase1DataAsync();
        var importer = _importer;

        var corpus = new LexemeCorpus
        {
            VersionNumber = $"v1.0-dir-{Guid.NewGuid():N}",
            SourceName = "Dict Dir",
            SourceLevel = "AcademicMaterial",
            LanguageVariantName = variant.Name,
            Records = new List<LexemeCorpusRecord>
            {
                new() { CanonicalForm = "bene xono", SpanishMeaning = "zorro", TargetTextEs = "zorro", VerificationStatus = "Verified", IsCanonical = true }
            }
        };

        await importer.ImportCorpusAsync(corpus);

        // Verify forward evidence exists
        var forwardEquivalences = await _context.LexicalEquivalences
            .Where(e => e.TargetLanguage == "es" && e.TargetText == "zorro")
            .ToListAsync();
        Assert.Single(forwardEquivalences);

        // Act: query reverse direction with a term that does NOT exist as TargetText
        var engine = new DictionaryTranslationEngine(_context);
        var reverseResult = await engine.TranslateAsync(new TranslationRequest(
            SourceLanguage: "español",
            TargetLanguage: variant.Name,
            Text: "zorro_inexistente"));

        // Assert: no reverse auto-generation (I2)
        Assert.Equal(TranslationStatus.NotFound, reverseResult.Status);

        // No reverse evidence should exist in the DB (the import only created forward)
        var reverseEquivalencesInDb = await _context.LexicalEquivalences
            .Where(e => e.TargetLanguage != "es" && e.TargetText == "bene xono")
            .ToListAsync();
        Assert.Empty(reverseEquivalencesInDb);
    }

    [Fact]
    public async Task Idempotency_SecondImportDoesNotDuplicate()
    {
        // Arrange
        var (variant, _) = await SeedPhase1DataAsync();
        var importer = _importer;

        var corpus = new LexemeCorpus
        {
            VersionNumber = $"v1.0-idem-{Guid.NewGuid():N}",
            SourceName = "Dict Idem",
            SourceLevel = "AcademicMaterial",
            LanguageVariantName = variant.Name,
            Records = new List<LexemeCorpusRecord>
            {
                new() { CanonicalForm = "tsi", SpanishMeaning = "casa", TargetTextEs = "casa", VerificationStatus = "Verified", IsCanonical = true }
            }
        };

        // Act: import twice
        var result1 = await importer.ImportCorpusAsync(corpus);
        var result2 = await importer.ImportCorpusAsync(corpus);

        // Assert: second import finds existing records, no duplicates
        Assert.True(result1.Success, $"Import 1 failed: {result1.ErrorMessage}. ValidationErrors: {string.Join("; ", result1.ValidationErrors)}");
        Assert.True(result2.Success, $"Import 2 failed: {result2.ErrorMessage}. ValidationErrors: {string.Join("; ", result2.ValidationErrors)}");

        // Debug: check versions in DB
        var versions = await _context.CatalogVersions.ToListAsync();
        Assert.True(versions.Count == 1, $"Expected 1 version, found {versions.Count}: {string.Join(", ", versions.Select(v => $"{v.VersionNumber}={v.ImportStatus}"))}");

        var allEquivalences = await _context.LexicalEquivalences.ToListAsync();
        Assert.True(allEquivalences.Count == 1, $"Expected 1 equivalence, found {allEquivalences.Count}: {string.Join(", ", allEquivalences.Select(e => $"{e.TargetText}@{e.CatalogVersionId}"))}");

        Assert.Equal(1, result1.LexemesCreated);
        Assert.Equal(0, result2.LexemesCreated);
        Assert.Equal(1, result1.MeaningsCreated);
        Assert.Equal(0, result2.MeaningsCreated);
        Assert.Equal(1, result1.EquivalencesCreated);
        Assert.Equal(0, result2.EquivalencesCreated);

        var lexemes = await _context.Lexemes.Where(l => l.CanonicalForm == "tsi").ToListAsync();
        Assert.Single(lexemes);

        var equivalences = await _context.LexicalEquivalences
            .Where(e => e.SourceLexemeId == lexemes[0].Id)
            .ToListAsync();
        Assert.Single(equivalences);
    }

    [Fact]
    public async Task CompletedVersion_IsImmutable()
    {
        // Arrange
        var (variant, _) = await SeedPhase1DataAsync();
        var importer = _importer;

        var corpus = new LexemeCorpus
        {
            VersionNumber = $"v1.0-immut-{Guid.NewGuid():N}",
            SourceName = "Dict Immut",
            SourceLevel = "AcademicMaterial",
            LanguageVariantName = variant.Name,
            Records = new List<LexemeCorpusRecord>
            {
                new() { CanonicalForm = "tsi", SpanishMeaning = "casa", TargetTextEs = "casa", VerificationStatus = "Verified", IsCanonical = true }
            }
        };

        await importer.ImportCorpusAsync(corpus);

        // Get the completed version
        var completedVersion = await _context.CatalogVersions
            .FirstAsync(v => v.ImportStatus == "Completed");

        // Act & Assert: UPDATE on completed version must be rejected by trigger
        var ex = await Assert.ThrowsAsync<MySqlException>(async () =>
            await _context.Database.ExecuteSqlRawAsync(
                "UPDATE CatalogVersions SET ImportStatus = 'Archived' WHERE Id = @p0",
                completedVersion.Id.ToString()));

        Assert.Equal("45000", ex.SqlState);
    }

    [Fact]
    public async Task InvalidRecord_IsQuarantined()
    {
        // Arrange
        var (variant, _) = await SeedPhase1DataAsync();
        var importer = _importer;

        var corpus = new LexemeCorpus
        {
            VersionNumber = $"v1.0-quar-{Guid.NewGuid():N}",
            SourceName = "Dict Quar",
            SourceLevel = "AcademicMaterial",
            LanguageVariantName = variant.Name,
            Records = new List<LexemeCorpusRecord>
            {
                new() { CanonicalForm = "", SpanishMeaning = "invalid", VerificationStatus = "Unknown" } // empty canonical form
            }
        };

        // Act
        var result = await importer.ImportCorpusAsync(corpus);

        // Assert
        Assert.True(result.Success); // corpus-level success despite record quarantine
        Assert.Equal(1, result.RecordsQuarantined);
        Assert.Equal(0, result.LexemesCreated);
        Assert.Equal("CanonicalForm is empty", result.RecordResults[0].Reason);
    }

    [Fact]
    public async Task Reproducibility_SameInputSameOutput()
    {
        // Arrange
        var (variant, _) = await SeedPhase1DataAsync();
        var importer = _importer;

        var versionNumber = $"v1.0-repro-{Guid.NewGuid():N}";
        var corpus = new LexemeCorpus
        {
            VersionNumber = versionNumber,
            SourceName = "Dict Repro",
            SourceLevel = "AcademicMaterial",
            LanguageVariantName = variant.Name,
            Records = new List<LexemeCorpusRecord>
            {
                new() { CanonicalForm = "tsi", SpanishMeaning = "casa", TargetTextEs = "casa", VerificationStatus = "Verified", IsCanonical = true }
            }
        };

        // Act: run twice on same version
        var result1 = await importer.ImportCorpusAsync(corpus);
        var result2 = await importer.ImportCorpusAsync(corpus);

        // Assert: deterministic and idempotent — second import finds existing, creates nothing new
        Assert.True(result1.Success);
        Assert.True(result2.Success);
        Assert.Equal(1, result1.LexemesCreated);
        Assert.Equal(0, result2.LexemesCreated); // existing
        Assert.Equal(1, result1.MeaningsCreated);
        Assert.Equal(0, result2.MeaningsCreated); // existing
        Assert.Equal(1, result1.EquivalencesCreated);
        Assert.Equal(0, result2.EquivalencesCreated); // existing
        Assert.Equal(result1.RecordsQuarantined, result2.RecordsQuarantined);
    }
}