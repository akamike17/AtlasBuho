using AtlasBuho.Data;
using AtlasBuho.Data.Seeding;
using AtlasBuho.Domain.AiReview;
using AtlasBuho.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit;

namespace AtlasBuho.Tests.Integration;

/// <summary>
/// B10 AI Separation Tests — verifies AI cannot create canonical evidence (Test 1).
/// </summary>
public class AiSeparationTests : IAsyncLifetime
{
    private readonly string _dbName = $"atlasbuho_test_ai_sep_{Guid.NewGuid():N}";
    private AtlasBuhoDbContext _context = null!;

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
    }

    public async Task DisposeAsync()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task Test1_AiDoesNotCreateCanonicalEvidence()
    {
        // Arrange: create AI review with correction proposal
        var review = new AiTranslationReview
        {
            InputText = "test input",
            SourceLanguageRequested = "variant-a",
            TargetLanguageRequested = "es",
            AtlasBuhoV1Result = "casa",
            ReviewStatus = AiReviewStatus.LikelyIncorrect,
            ProposedCorrection = "hogar",
            Explanation = "AI proposes alternative",
            ProviderName = "Mock",
            ModelName = "mock-v1",
            PromptVersion = "translation-review-v1",
            Success = true
        };

        var lexemeCountBefore = await _context.Lexemes.CountAsync();
        var meaningCountBefore = await _context.Meanings.CountAsync();
        var equivalenceCountBefore = await _context.LexicalEquivalences.CountAsync();

        // Act: save AI review
        _context.AiTranslationReviews.Add(review);
        await _context.SaveChangesAsync();

        // Assert: canonical evidence unchanged, only AI review created
        var lexemeCountAfter = await _context.Lexemes.CountAsync();
        var meaningCountAfter = await _context.Meanings.CountAsync();
        var equivalenceCountAfter = await _context.LexicalEquivalences.CountAsync();

        Assert.Equal(lexemeCountBefore, lexemeCountAfter);
        Assert.Equal(meaningCountBefore, meaningCountAfter);
        Assert.Equal(equivalenceCountBefore, equivalenceCountAfter);
        Assert.Equal(1, await _context.AiTranslationReviews.CountAsync());
    }

    [Fact]
    public async Task Test2_AiCorrectionIsPersistedAsReview()
    {
        // Arrange & Act
        var review = new AiTranslationReview
        {
            InputText = "bene xono",
            SourceLanguageRequested = "chontal",
            TargetLanguageRequested = "es",
            AtlasBuhoV1Result = "zorro",
            ReviewStatus = AiReviewStatus.PotentialIssue,
            ProposedCorrection = "zorro alternative",
            Explanation = "Possible dialectal variant",
            EvidenceNotes = "Low confidence in current evidence",
            ProviderName = "OpenAI",
            ModelName = "gpt-4",
            PromptVersion = "translation-review-v1",
            LatencyMs = 1250,
            Success = true,
            CorrelationId = Guid.NewGuid()
        };

        _context.AiTranslationReviews.Add(review);
        await _context.SaveChangesAsync();

        // Assert
        var saved = await _context.AiTranslationReviews.FirstAsync();
        Assert.Equal("bene xono", saved.InputText);
        Assert.Equal("zorro", saved.AtlasBuhoV1Result);
        Assert.Equal("zorro alternative", saved.ProposedCorrection);
        Assert.Equal("OpenAI", saved.ProviderName);
        Assert.Equal("gpt-4", saved.ModelName);
        Assert.Equal("translation-review-v1", saved.PromptVersion);
        Assert.Equal(1250, saved.LatencyMs);
        Assert.True(saved.Success);
    }

    [Fact]
    public async Task Test3_AiLanguageDetectionDoesNotOverwriteVariant()
    {
        // Arrange: create documented variant
        var familyId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var variantId = Guid.NewGuid();

        _context.Database.ExecuteSqlRaw(
            @"INSERT INTO LanguageFamilies (Id, Name, NameEnglish, CreatedAt, UpdatedAt)
              VALUES (@p0, 'Otomanguean', 'Otomanguean', UTC_TIMESTAMP(6), UTC_TIMESTAMP(6))",
            familyId.ToString());

        _context.Database.ExecuteSqlRaw(
            @"INSERT INTO LanguageGroups (Id, LanguageFamilyId, Name, CreatedAt, UpdatedAt)
              VALUES (@p0, @p1, 'Popolocan', UTC_TIMESTAMP(6), UTC_TIMESTAMP(6))",
            groupId.ToString(), familyId.ToString());

        _context.Database.ExecuteSqlRaw(
            @"INSERT INTO LanguageVariants (Id, LanguageGroupId, Name, CreatedAt, UpdatedAt)
              VALUES (@p0, @p1, 'Chontal de Oaxaca', UTC_TIMESTAMP(6), UTC_TIMESTAMP(6))",
            variantId.ToString(), groupId.ToString());

        // Act: AI detects different variant
        var review = new AiTranslationReview
        {
            InputText = "test",
            SourceLanguageRequested = "Chontal de Oaxaca",
            TargetLanguageRequested = "es",
            DetectedLanguage = "Chontal de Tabasco", // AI detects different variant
            DetectedLanguageConfidence = 0.45,
            ReviewStatus = AiReviewStatus.PotentialIssue,
            ProviderName = "Mock",
            ModelName = "mock-v1",
            PromptVersion = "translation-review-v1",
            Success = true
        };

        _context.AiTranslationReviews.Add(review);
        await _context.SaveChangesAsync();

        // Assert: documented variant unchanged
        var variant = await _context.LanguageVariants.FindAsync(variantId);
        Assert.Equal("Chontal de Oaxaca", variant!.Name);

        // AI detection stored separately
        var savedReview = await _context.AiTranslationReviews.FirstAsync();
        Assert.Equal("Chontal de Tabasco", savedReview.DetectedLanguage);
        Assert.NotEqual(variant.Name, savedReview.DetectedLanguage);
    }

    [Fact]
    public async Task Test4_InsufficientEvidenceDoesNotFabricateTranslation()
    {
        // Arrange & Act
        var review = new AiTranslationReview
        {
            InputText = "unknown term",
            SourceLanguageRequested = "variant-x",
            TargetLanguageRequested = "es",
            AtlasBuhoV1Result = null,
            ReviewStatus = AiReviewStatus.InsufficientEvidence,
            ProposedCorrection = null, // No correction proposed
            Explanation = "AI cannot establish correction without evidence",
            ProviderName = "Mock",
            ModelName = "mock-v1",
            PromptVersion = "translation-review-v1",
            Success = true
        };

        _context.AiTranslationReviews.Add(review);
        await _context.SaveChangesAsync();

        // Assert
        var saved = await _context.AiTranslationReviews.FirstAsync();
        Assert.Equal(AiReviewStatus.InsufficientEvidence, saved.ReviewStatus);
        Assert.Null(saved.ProposedCorrection);
    }

    [Fact]
    public async Task Test5_ProviderFailureDoesNotAffectV1()
    {
        // Arrange: create canonical evidence first
        var familyId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var variantId = Guid.NewGuid();

        _context.Database.ExecuteSqlRaw(
            @"INSERT INTO LanguageFamilies (Id, Name, NameEnglish, CreatedAt, UpdatedAt)
              VALUES (@p0, 'Otomanguean', 'Otomanguean', UTC_TIMESTAMP(6), UTC_TIMESTAMP(6))",
            familyId.ToString());

        _context.Database.ExecuteSqlRaw(
            @"INSERT INTO LanguageGroups (Id, LanguageFamilyId, Name, CreatedAt, UpdatedAt)
              VALUES (@p0, @p1, 'Popolocan', UTC_TIMESTAMP(6), UTC_TIMESTAMP(6))",
            groupId.ToString(), familyId.ToString());

        _context.Database.ExecuteSqlRaw(
            @"INSERT INTO LanguageVariants (Id, LanguageGroupId, Name, CreatedAt, UpdatedAt)
              VALUES (@p0, @p1, 'Chontal de Oaxaca', UTC_TIMESTAMP(6), UTC_TIMESTAMP(6))",
            variantId.ToString(), groupId.ToString());

        // Act: AI provider fails
        var review = new AiTranslationReview
        {
            InputText = "test",
            SourceLanguageRequested = "Chontal de Oaxaca",
            TargetLanguageRequested = "es",
            ReviewStatus = AiReviewStatus.ProviderError,
            ProviderName = "OpenAI",
            ModelName = "gpt-4",
            PromptVersion = "translation-review-v1",
            Success = false,
            ErrorCode = "TIMEOUT"
        };

        _context.AiTranslationReviews.Add(review);
        await _context.SaveChangesAsync();

        // Assert: failure logged, V1 unchanged
        var saved = await _context.AiTranslationReviews.FirstAsync();
        Assert.False(saved.Success);
        Assert.Equal("TIMEOUT", saved.ErrorCode);
        Assert.Equal(AiReviewStatus.ProviderError, saved.ReviewStatus);

        // Canonical variant still exists
        var variant = await _context.LanguageVariants.FindAsync(variantId);
        Assert.NotNull(variant);
    }

    [Fact]
    public async Task Test6_ProviderIndependenceThroughAbstraction()
    {
        // This test verifies the abstraction is testable without real AI
        // MockAiTranslationReviewer is used in unit tests — integration tests verify persistence layer

        var review = new AiTranslationReview
        {
            InputText = "abstraction test",
            SourceLanguageRequested = "variant",
            TargetLanguageRequested = "es",
            ProviderName = "MockProvider", // Different provider name
            ModelName = "mock-model-v2",
            PromptVersion = "translation-review-v2",
            Success = true
        };

        _context.AiTranslationReviews.Add(review);
        await _context.SaveChangesAsync();

        var saved = await _context.AiTranslationReviews.FirstAsync();
        Assert.Equal("MockProvider", saved.ProviderName);
        Assert.Equal("translation-review-v2", saved.PromptVersion);
    }

    [Fact]
    public async Task Test7_PromptVersionIsPersisted()
    {
        // Arrange & Act
        var review = new AiTranslationReview
        {
            InputText = "version test",
            SourceLanguageRequested = "variant",
            TargetLanguageRequested = "es",
            ProviderName = "Mock",
            ModelName = "mock-v1",
            PromptVersion = "translation-review-v2", // Non-default version
            Success = true
        };

        _context.AiTranslationReviews.Add(review);
        await _context.SaveChangesAsync();

        // Assert
        var saved = await _context.AiTranslationReviews.FirstAsync();
        Assert.Equal("translation-review-v2", saved.PromptVersion);
        Assert.Equal("v1", saved.ResponseSchemaVersion);
    }

    [Fact]
    public async Task Test8_NoSecretsInAiReviewRecords()
    {
        // Arrange & Act: create reviews with sensitive-looking data in wrong fields
        var review = new AiTranslationReview
        {
            InputText = "test input", // Normal user text
            SourceLanguageRequested = "variant",
            TargetLanguageRequested = "es",
            ProviderName = "Mock",
            ModelName = "mock-v1",
            PromptVersion = "translation-review-v1",
            Success = true
        };

        _context.AiTranslationReviews.Add(review);
        await _context.SaveChangesAsync();

        // Assert: no API keys, tokens, or secrets in metadata fields
        var saved = await _context.AiTranslationReviews.FirstAsync();

        // Check metadata fields that should never contain secrets
        var metadataFields = new[]
        {
            saved.ProviderName, saved.ModelName, saved.PromptVersion,
            saved.DetectedLanguage, saved.ProposedCorrection,
            saved.Explanation, saved.EvidenceNotes, saved.ErrorCode
        };

        foreach (var field in metadataFields)
        {
            if (field != null)
            {
                Assert.DoesNotContain("apikey", field, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("api_key", field, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("bearer", field, StringComparison.OrdinalIgnoreCase);
            }
        }

        // InputText is user data — we don't scan it for secrets as it's not metadata
        Assert.Equal("test input", saved.InputText);
    }
}
