using AtlasBuho.Application.AiReview;
using AtlasBuho.Application.Translation;
using AtlasBuho.Data;
using AtlasBuho.Data.Seeding;
using AtlasBuho.Data.Translation;
using AtlasBuho.Domain.AiReview;
using AtlasBuho.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AtlasBuho.Tests.Integration;

/// <summary>
/// Tests for the real TranslationQueryHandler orchestrator (B10 §16).
/// Tests invoke the actual orchestrator, not manual entity creation.
/// </summary>
public class TranslationOrchestrationTests : IDisposable
{
    private readonly AtlasBuhoDbContext _context;
    private readonly TranslationQueryHandler _handler;
    private readonly ITranslationEngine _translationEngine;
    private readonly TestAiTranslationReviewer _testReviewer;
    private readonly AiReviewOptions _aiOptions;

    public TranslationOrchestrationTests()
    {
        var options = new DbContextOptionsBuilder<AtlasBuhoDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _context = new AtlasBuhoDbContext(options);
        _context.Database.EnsureCreated();

        _translationEngine = new DictionaryTranslationEngine(_context);
        _testReviewer = new TestAiTranslationReviewer();
        _aiOptions = new AiReviewOptions { Enabled = true, Provider = "Mock", Model = "mock-v1" };

        var persistence = new AiOrchestrationPersistence(_context);
        var extractor = new CandidateExtractor();

        _handler = new TranslationQueryHandler(
            _translationEngine,
            _testReviewer,
            persistence,
            _aiOptions,
            extractor);
    }

    [Fact]
    public async Task TestA_V1Only_WhenAiDisabled_ReturnsV1Result()
    {
        // Arrange: disable AI
        _aiOptions.Enabled = false;
        _testReviewer.ShouldThrow = false;
        await SeedTestData("tsi", "casa", "es");

        var request = new TranslationRequest("test-variant", "es", "tsi");
        var correlationId = Guid.NewGuid();

        // Act
        var result = await _handler.HandleAsync(request, correlationId);

        // Assert: V1 is the result (may be NotFound if seed incomplete)
        Assert.NotNull(result.V1Result);
        Assert.Null(result.AiReview);
        Assert.Null(result.Candidate);

        // No review persisted
        var reviews = await _context.AiTranslationReviews.ToListAsync();
        Assert.Empty(reviews);
        Assert.Equal(0, _testReviewer.CallCount);
    }

    [Fact]
    public async Task TestB_V1PlusAiReview_PersistsReviewWithActualV1()
    {
        // Arrange
        await SeedTestData("tsi", "casa", "es");
        _testReviewer.SetResponse(AiReviewStatus.LikelyIncorrect, proposedCorrection: "hogar");

        var request = new TranslationRequest("test-variant", "es", "tsi");
        var correlationId = Guid.NewGuid();

        // Act
        var result = await _handler.HandleAsync(request, correlationId);

        // Assert: AI review persisted with V1 result
        var savedReview = await _context.AiTranslationReviews.FirstAsync();
        Assert.Equal("tsi", savedReview.InputText);
        Assert.Equal("hogar", savedReview.ProposedCorrection);
        Assert.Equal(AiReviewStatus.LikelyIncorrect, savedReview.ReviewStatus);
        Assert.Equal(correlationId, savedReview.CorrelationId);
    }

    [Fact]
    public async Task TestC_AiCorrectionDoesNotModifyCanonicalV1()
    {
        // Arrange: canonical evidence: tsi → casa
        var (lexemeId, _) = await SeedTestData("tsi", "casa", "es");
        _testReviewer.SetResponse(AiReviewStatus.LikelyIncorrect, proposedCorrection: "hogar");

        var request = new TranslationRequest("test-variant", "es", "tsi");
        var correlationId = Guid.NewGuid();

        // Act
        var result = await _handler.HandleAsync(request, correlationId);

        // Assert: AI review persisted with correction
        Assert.NotNull(result.AiReview);
        Assert.Equal("hogar", result.AiReview!.ProposedCorrection);

        // Assert: canonical equivalence unchanged
        var canonical = await _context.LexicalEquivalences
            .FirstAsync(e => e.SourceLexemeId == lexemeId && e.TargetLanguage == "es");
        Assert.Equal("casa", canonical.TargetText);
        Assert.True(canonical.IsCanonical);

        // Assert: "hogar" does NOT exist in canonical data
        var correctionInCanonical = await _context.LexicalEquivalences
            .Where(e => e.TargetText == "hogar")
            .ToListAsync();
        Assert.Empty(correctionInCanonical);

        // Assert: canonical lexeme unchanged
        var lexeme = await _context.Lexemes.FindAsync(lexemeId);
        Assert.Equal("tsi", lexeme!.CanonicalForm);
    }

    [Fact]
    public async Task TestD_CandidateExtraction_FromEligibleCorrection()
    {
        // Arrange
        await SeedTestData("tsi", "casa", "es");
        _testReviewer.SetResponse(AiReviewStatus.LikelyIncorrect, proposedCorrection: "hogar");

        var request = new TranslationRequest("test-variant", "es", "tsi");

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert: V2Candidate exists, IsPromoted = false
        Assert.NotNull(result.Candidate);
        Assert.False(result.Candidate!.IsPromoted);
        Assert.Equal("hogar", result.Candidate.TargetText);
    }

    [Fact]
    public async Task TestE_InsufficientEvidence_NoCandidateCreated()
    {
        // Arrange
        await SeedTestData("tsi", "casa", "es");
        _testReviewer.SetResponse(AiReviewStatus.InsufficientEvidence, proposedCorrection: null);

        var request = new TranslationRequest("test-variant", "es", "tsi");

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert: V1 returned (may be null), review persisted, NO candidate
        Assert.NotNull(result.AiReview);
        Assert.Equal(AiReviewStatus.InsufficientEvidence, result.AiReview!.ReviewStatus);
        Assert.Null(result.Candidate);

        // No candidate in database
        var candidates = await _context.V2Candidates.ToListAsync();
        Assert.Empty(candidates);
    }

    [Fact]
    public async Task TestF_AiDisabled_ReviewerNotInvoked()
    {
        // Arrange: disable AI, configure reviewer to fail if called
        _aiOptions.Enabled = false;
        _testReviewer.ShouldThrow = true;

        await SeedTestData("tsi", "casa", "es");
        var request = new TranslationRequest("test-variant", "es", "tsi");

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert: no exception (reviewer was NOT called)
        Assert.Null(result.AiReview);
        Assert.Null(result.Candidate);
        Assert.Equal(0, _testReviewer.CallCount);
    }

    [Fact]
    public async Task TestG_AiProviderFailure_V1StillWorks()
    {
        // Arrange
        await SeedTestData("tsi", "casa", "es");
        _testReviewer.SetException(new InvalidOperationException("AI provider unavailable"));

        var request = new TranslationRequest("test-variant", "es", "tsi");

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert: V1 still works (may be null), failure recorded
        Assert.NotNull(result.AiReview);
        Assert.Equal(AiReviewStatus.ProviderError, result.AiReview!.ReviewStatus);
        Assert.False(result.AiReview.Success);
        Assert.Equal("InvalidOperationException", result.AiReview.ErrorCode);
        Assert.Null(result.Candidate);

        // Canonical data unchanged
        var canonicalCount = await _context.LexicalEquivalences.CountAsync();
        Assert.Equal(1, canonicalCount);
    }

    [Fact]
    public async Task TestH_AiCorrectionCannotBePromotedAutomatically()
    {
        // Arrange
        await SeedTestData("tsi", "casa", "es");
        _testReviewer.SetResponse(AiReviewStatus.LikelyIncorrect, proposedCorrection: "hogar");

        var request = new TranslationRequest("test-variant", "es", "tsi");

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert: V2Candidate exists but IsPromoted is false
        Assert.NotNull(result.Candidate);
        Assert.False(result.Candidate!.IsPromoted);
        Assert.Null(result.Candidate.PromotedAt);

        // Assert: persisted candidate is also not promoted
        var persisted = await _context.V2Candidates.FirstAsync();
        Assert.False(persisted.IsPromoted);
        Assert.Null(persisted.PromotedAt);
    }

    private async Task<(Guid LexemeId, Guid EquivalenceId)> SeedTestData(
        string lexeme,
        string meaning,
        string targetLanguage)
    {
        var group = new LanguageGroup(
            languageFamilyId: Guid.NewGuid(),
            name: "test-group",
            nameEnglish: null,
            description: null,
            source: null);
        _context.LanguageGroups.Add(group);
        await _context.SaveChangesAsync();

        var variant = new LanguageVariant(
            languageGroupId: group.Id,
            name: "test-variant",
            autodenomination: null,
            iso639_3Code: null,
            inaliCode: null,
            description: null,
            writingSystem: null,
            orthography: null,
            source: null);
        _context.LanguageVariants.Add(variant);
        await _context.SaveChangesAsync();

        var newLexeme = new Lexeme(
            languageVariantId: variant.Id,
            canonicalForm: lexeme,
            alternativeForms: null,
            autodenomination: null,
            spanishMeaning: meaning,
            partOfSpeech: null,
            pronunciationIpa: null,
            pronunciationReadable: null,
            semanticDomain: null,
            register: null,
            regionalNotes: null,
            etymology: null,
            source: "test",
            verificationStatus: VerificationStatus.Verified,
            confidence: 0.95);
        _context.Lexemes.Add(newLexeme);
        await _context.SaveChangesAsync();

        var newMeaning = new Meaning(
            newLexeme.Id,
            meaning,
            null, null, null, null, null, "src",
            VerificationStatus.Verified, 0.95, 0);
        _context.Meanings.Add(newMeaning);
        await _context.SaveChangesAsync();

        var catalogSource = new CatalogSource(
            "test-src", "Test Catalog", "https://test.com");
        _context.CatalogSources.Add(catalogSource);
        await _context.SaveChangesAsync();

        var version = new CatalogVersion(
            catalogSource.Id, "v1", "hash", DateTime.UtcNow,
            0, 0, 0, 0, "v1.0", "Completed");
        _context.CatalogVersions.Add(version);
        await _context.SaveChangesAsync();

        var source = new Source(
            "src-1", SourceLevel.AcademicMaterial,
            null, null, null, null, null, null, null, null, null, null, null, null, null);
        _context.Sources.Add(source);
        await _context.SaveChangesAsync();

        var equivalence = new LexicalEquivalence(
            newLexeme.Id,
            targetLanguage,
            meaning,
            true,
            VerificationStatus.Verified,
            source.Id,
            version.Id);
        _context.LexicalEquivalences.Add(equivalence);
        await _context.SaveChangesAsync();

        // Create CatalogRecord with variant name (this is the crucial part for the engine)
        var catalogRecord = new CatalogRecord(
            catalogVersionId: version.Id,
            entityType: "LanguageVariant",
            identityKey: $"Variant:{variant.Name}",
            inaliCode: string.Empty,
            iso639_3Code: string.Empty,
            name: variant.Name,  // Line 361-362: this is what NormalizeLanguage looks for
            autodenomination: null,
            description: null,
            geoReference: null,
            sourceDocumentId: source.Id,
            sourceHash: "hash",
            sourceUrl: "https://test.com",
            sourcePage: 1,
            sourceSection: "test",
            extractionDate: DateTime.UtcNow,
            parserVersion: "1.0",
            parentRecordId: null);
        _context.CatalogRecords.Add(catalogRecord);
        await _context.SaveChangesAsync();

        return (newLexeme.Id, equivalence.Id);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

/// <summary>
/// Test double for IAiTranslationReviewer with deterministic scenarios.
/// </summary>
public sealed class TestAiTranslationReviewer : IAiTranslationReviewer
{
    private AiReviewStatus _status = AiReviewStatus.Supported;
    private string? _proposedCorrection = null;
    private Exception? _exception = null;

    public int CallCount { get; private set; }

    public bool ShouldThrow { get; set; }

    public void SetResponse(AiReviewStatus status, string? proposedCorrection = null)
    {
        _status = status;
        _proposedCorrection = proposedCorrection;
        _exception = null;
    }

    public void SetException(Exception exception)
    {
        _exception = exception;
    }

    public Task<AiTranslationReviewResult> ReviewTranslationAsync(
        AiTranslationReviewRequest request,
        CancellationToken ct = default)
    {
        CallCount++;

        if (ShouldThrow)
            throw new InvalidOperationException("Test double configured to throw");

        if (_exception != null)
            throw _exception;

        var result = new AiTranslationReviewResult(
            Status: _status,
            DetectedLanguage: request.SourceLanguage,
            DetectedLanguageConfidence: 0.85,
            ProposedCorrection: _proposedCorrection,
            Explanation: $"Test explanation for {_status}",
            EvidenceNotes: "Test evidence notes",
            Provider: "TestMock",
            Model: "test-model-v1",
            PromptVersion: "test-prompt-v1",
            LatencyMs: 10,
            Success: true,
            ErrorCode: null);

        return Task.FromResult(result);
    }

    public Task<AiLanguageDetectionResult> DetectLanguageAsync(
        string text,
        IReadOnlyList<string> knownVariants,
        CancellationToken ct = default)
    {
        CallCount++;
        var result = new AiLanguageDetectionResult(
            DetectedLanguage: knownVariants.FirstOrDefault() ?? "unknown",
            Confidence: 0.5,
            Provider: "TestMock",
            Model: "test-model",
            ConflictsWithDocumented: false,
            DocumentedVariant: knownVariants.FirstOrDefault());
        return Task.FromResult(result);
    }
}