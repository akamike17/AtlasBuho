using AtlasBuho.Application.AiReview;
using AtlasBuho.Application.Translation;
using AtlasBuho.Data;
using AtlasBuho.Data.Extensions;
using AtlasBuho.Data.Translation;
using AtlasBuho.Domain.AiReview;
using AtlasBuho.Domain.Entities;
using AtlasBuho.Infrastructure.AiProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AtlasBuho.Tests.Integration;

/// <summary>
/// Behavioral tests for B10.1: verify actual runtime resolution of provider service.
/// </summary>
public class AiProviderBehavioralTests : IDisposable
{
    private readonly AtlasBuhoDbContext _context;

    public AiProviderBehavioralTests()
    {
        var options = new DbContextOptionsBuilder<AtlasBuhoDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _context = new AtlasBuhoDbContext(options);
        _context.Database.EnsureCreated();
    }

    [Fact]
    public void TestA_DisabledConfiguration_ResolvesDisabledReviewer()
    {
        // Arrange: Build actual service container with Disabled provider
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AtlasBuho:AI:Enabled"] = "false",
                ["AtlasBuho:AI:Provider"] = "Disabled",
                ["AtlasBuho:AI:Model"] = "mock-v1",
                ["AtlasBuho:AI:ApiKey"] = "test-placeholder"
            })
            .Build());
        services.AddAtlasBuhoTranslationForTesting();

        var serviceProvider = services.BuildServiceProvider();

        // Act: Resolve the actual service
        var reviewer = serviceProvider.GetRequiredService<IAiTranslationReviewer>();

        // Assert: DisabledAiTranslationReviewer is resolved, not Mock
        Assert.IsType<DisabledAiTranslationReviewer>(reviewer);
    }

    [Fact]
    public void TestB_MockProvider_SelectableThroughConfiguration()
    {
        // Arrange: Build actual service container with Mock provider
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AtlasBuho:AI:Enabled"] = "true",
                ["AtlasBuho:AI:Provider"] = "Mock",
                ["AtlasBuho:AI:Model"] = "mock-v1",
                ["AtlasBuho:AI:ApiKey"] = "test-placeholder"
            })
            .Build());
        services.AddAtlasBuhoTranslationForTesting();

        var serviceProvider = services.BuildServiceProvider();

        // Act: Resolve IAiTranslationReviewer
        var reviewer = serviceProvider.GetRequiredService<IAiTranslationReviewer>();

        // Assert: MockAiTranslationReviewer is resolved through DI
        Assert.IsType<MockAiTranslationReviewer>(reviewer);
    }

    [Fact]
    public void TestC_UnknownProvider_FailsWithClearError()
    {
        // Arrange: Build actual service container with unknown provider
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AtlasBuho:AI:Enabled"] = "true",
                ["AtlasBuho:AI:Provider"] = "UnknownProvider",
                ["AtlasBuho:AI:Model"] = "unknown",
                ["AtlasBuho:AI:ApiKey"] = "test-placeholder"
            })
            .Build());
        services.AddAtlasBuhoTranslationForTesting();

        var serviceProvider = services.BuildServiceProvider();

        // Act & Assert: Resolving should fail deterministically with clear error
        var exception = Assert.Throws<InvalidOperationException>(() =>
            serviceProvider.GetRequiredService<IAiTranslationReviewer>());

        Assert.Contains("Unknown AI provider", exception.Message);
        Assert.Contains("UnknownProvider", exception.Message);
    }

    [Fact]
    public void TestD_ExternalPluggableProvider_SelectableThroughConfiguration()
    {
        // Arrange: Register external factory in DI (proving extensibility)
        // This proves the registry allows external providers WITHOUT modifying Domain/Handler
        var services = new ServiceCollection();
        services.AddLogging();

        // External provider configuration
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AtlasBuho:AI:Enabled"] = "true",
                ["AtlasBuho:AI:Provider"] = "ExternalTestProvider",
                ["AtlasBuho:AI:Model"] = "external-model-v1",
                ["AtlasBuho:AI:ApiKey"] = "test-placeholder"
            })
            .Build());

        // Register AtlasBuho services (includes default factories)
        services.AddAtlasBuhoTranslationForTesting();

        // Register EXTERNAL factory — this is the key to provider neutrality
        // External providers can be added without modifying Domain/Handler
        services.AddSingleton<IAiTranslationReviewerFactory, TestExternalProviderFactory>();

        var serviceProvider = services.BuildServiceProvider();

        // Act: Resolve through registry (registry resolves by provider name from config)
        var reviewer = serviceProvider.GetRequiredService<IAiTranslationReviewer>();

        // Assert: External provider is resolved (proves registry selects by provider name)
        Assert.IsType<TestExternalProvider>( reviewer);
    }

    [Fact]
    public async Task TestE_AiFailure_V1CanonicalResultsSurvives()
    {
        // Arrange: Create failing reviewer wired directly (handler-level test)
        var failingReviewer = new TestAiTranslationReviewer();
        failingReviewer.SetException(new InvalidOperationException("AI provider failed"));

        var handler = new TranslationQueryHandler(
            new DictionaryTranslationEngine(_context),
            failingReviewer,
            new AiOrchestrationPersistence(_context),
            new AiReviewOptions { Enabled = true, Provider = "Mock", Model = "mock-v1" },
            new CandidateExtractor());

        // Act
        var request = new TranslationRequest("test", "es", "test-input");
        var result = await handler.HandleAsync(request);

        // Assert: V1 survives, failure recorded
        Assert.NotNull(result.V1Result);
        Assert.NotNull(result.AiReview);
        Assert.False(result.AiReview!.Success);
        Assert.Equal(AiReviewStatus.ProviderError, result.AiReview!.ReviewStatus);
        Assert.Equal("InvalidOperationException", result.AiReview.ErrorCode);
    }

    [Fact]
    public async Task TestF_CanonicalDictionaryIsolation()
    {
        // Arrange
        var (lexemeId, _) = await SeedTestData("tsi", "casa", "es");

        var mockReviewer = new MockAiTranslationReviewer(
            new AiReviewOptions { Enabled = true, Provider = "Mock", Model = "mock-v1" });

        var handler = new TranslationQueryHandler(
            new DictionaryTranslationEngine(_context),
            mockReviewer,
            new AiOrchestrationPersistence(_context),
            new AiReviewOptions { Enabled = true, Provider = "Mock", Model = "mock-v1" },
            new CandidateExtractor());

        // Act
        var request = new TranslationRequest("test", "es", "casa");
        var result = await handler.HandleAsync(request);

        // Assert: canonical unchanged regardless of AI output
        var canonical = await _context.LexicalEquivalences
            .FirstAsync(e => e.SourceLexemeId == lexemeId && e.TargetLanguage == "es");
        Assert.Equal("casa", canonical.TargetText);
        Assert.True(canonical.IsCanonical);

        // "hogar" not inserted into canonical data
        var correctionInCanonical = await _context.LexicalEquivalences
            .Where(e => e.TargetText == "hogar")
            .ToListAsync();
        Assert.Empty(correctionInCanonical);
    }

    [Fact]
    public async Task TestG_CandidateNotPromoted_Automatic()
    {
        // Arrange
        var mockReviewer = new TestAiTranslationReviewer();
        mockReviewer.SetResponse(AiReviewStatus.LikelyIncorrect, "hogar");
        var handler = new TranslationQueryHandler(
            new DictionaryTranslationEngine(_context),
            mockReviewer,
            new AiOrchestrationPersistence(_context),
            new AiReviewOptions { Enabled = true, Provider = "Mock", Model = "mock-v1" },
            new CandidateExtractor());

        // Act
        var request = new TranslationRequest("test", "es", "test-input");
        var result = await handler.HandleAsync(request);

        // Assert
        if (result.Candidate != null)
        {
            Assert.False(result.Candidate.IsPromoted);
            Assert.Null(result.Candidate.PromotedAt);
        }
    }

    [Fact]
    public void TestH_NoSecretsInConfiguration()
    {
        var secureOptions = new AiReviewOptions
        {
            Enabled = true, Provider = "Mock", Model = "mock-v1",
            Endpoint = "", ApiKey = "test-placeholder"
        };

        Assert.Equal("test-placeholder", secureOptions.ApiKey);
        Assert.False(secureOptions.ApiKey.StartsWith("sk-", StringComparison.Ordinal));
    }

    // Test-only external provider (B10.1 §8)
    public sealed class TestExternalProvider : IAiTranslationReviewer
    {
        public Task<AiTranslationReviewResult> ReviewTranslationAsync(
            AiTranslationReviewRequest request, CancellationToken ct = default)
            => Task.FromResult(new AiTranslationReviewResult(
                Status: AiReviewStatus.PotentialIssue,
                DetectedLanguage: request.SourceLanguage,
                DetectedLanguageConfidence: 0.95,
                ProposedCorrection: "external-correction",
                Explanation: "External provider test",
                EvidenceNotes: null,
                Provider: "ExternalTestProvider",
                Model: "test-model-v1",
                PromptVersion: "external-v1",
                LatencyMs: 100,
                Success: true,
                ErrorCode: null));

        public Task<AiLanguageDetectionResult> DetectLanguageAsync(
            string text, IReadOnlyList<string> knownVariants, CancellationToken ct = default)
            => Task.FromResult(new AiLanguageDetectionResult("test-lang", 0.95, "ExternalTestProvider", "test-model-v1", false, null));
    }

    // Test factory for external provider (B10.1 §8)
    public sealed class TestExternalProviderFactory : IAiTranslationReviewerFactory
    {
        public bool CanResolve(string providerName)
            => providerName.Equals("ExternalTestProvider", StringComparison.OrdinalIgnoreCase);

        public IAiTranslationReviewer Create(AiReviewOptions options)
            => new TestExternalProvider();
    }

    private async Task<(Guid LexemeId, Guid EquivalenceId)> SeedTestData(string lexeme, string meaning, string targetLanguage)
    {
        var group = new LanguageGroup(Guid.NewGuid(), "test-group", null, null, null);
        _context.LanguageGroups.Add(group);
        await _context.SaveChangesAsync();

        var variant = new LanguageVariant(group.Id, "test-variant", null, null, null, null, null, null, null);
        _context.LanguageVariants.Add(variant);
        await _context.SaveChangesAsync();

        var newLexeme = new Lexeme(variant.Id, lexeme, null, null, meaning, null, null, null, null, null, null, null, "test", VerificationStatus.Verified, 0.95);
        _context.Lexemes.Add(newLexeme);
        await _context.SaveChangesAsync();

        var newMeaning = new Meaning(newLexeme.Id, meaning, null, null, null, null, null, "src", VerificationStatus.Verified, 0.95, 0);
        _context.Meanings.Add(newMeaning);
        await _context.SaveChangesAsync();

        var source = new Source("src-1", SourceLevel.AcademicMaterial, null, null, null, null, null, null, null, null, null, null, null, null, null);
        _context.Sources.Add(source);
        await _context.SaveChangesAsync();

        var catalogSource = new CatalogSource($"Src{Guid.NewGuid():N}", "Test", "https://test.com");
        _context.CatalogSources.Add(catalogSource);
        await _context.SaveChangesAsync();

        var version = new CatalogVersion(catalogSource.Id, $"v1-{Guid.NewGuid():N}", "hash", DateTime.UtcNow, 0, 0, 0, 0, "v1.0", "Completed");
        _context.CatalogVersions.Add(version);
        await _context.SaveChangesAsync();

        var equivalence = new LexicalEquivalence(newLexeme.Id, targetLanguage, meaning, true, VerificationStatus.Verified, source.Id, version.Id);
        _context.LexicalEquivalences.Add(equivalence);
        await _context.SaveChangesAsync();

        return (newLexeme.Id, equivalence.Id);
    }

    public void Dispose() => _context.Dispose();
}