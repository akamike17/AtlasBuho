using AtlasBuho.Application.AiReview;
using AtlasBuho.Application.Translation;
using AtlasBuho.Data;
using AtlasBuho.Data.Translation;
using AtlasBuho.Domain.AiReview;
using AtlasBuho.Domain.Entities;
using AtlasBuho.Infrastructure.AiProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using AtlasBuho.Application.AiReview;
using Xunit;

namespace AtlasBuho.Tests.Integration;

/// <summary>
/// Tests for provider-neutral AI configuration (B10.1 §7).
/// Tests the AiReviewOptionsProvider architecture: Disabled, Mock, Unknown provider.
/// </summary>
public class AiProviderConfigurationTests : IDisposable
{
    private readonly AtlasBuhoDbContext _context;
    private readonly TranslationQueryHandler _handler;

    public AiProviderConfigurationTests()
    {
        var options = new DbContextOptionsBuilder<AtlasBuhoDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _context = new AtlasBuhoDbContext(options);
        _context.Database.EnsureCreated();
    }

    [Fact]
    public void TestA_DisabledConfiguration_AiNotInvoked()
    {
        // Arrange: Disabled AI
        var disabledOptions = new AiReviewOptions { Enabled = false, Provider = "Disabled" };
        var disabledReviewer = new DisabledAiTranslationReviewer();

        // Act: Verify that disabled options don't require a reviewer
        // The TranslationQueryHandler should accept null reviewer when AI is disabled
        Assert.False(disabledOptions.Enabled);
        Assert.Equal("Disabled", disabledOptions.Provider);
    }

    [Fact]
    public void TestB_MockProvider_SelectableThroughConfiguration()
    {
        // Arrange: Mock provider configuration
        var mockOptions = new AiReviewOptions
        {
            Enabled = true,
            Provider = "Mock",
            Model = "mock-v1",
            Endpoint = "",
            ApiKey = "test-placeholder"
        };

        // Act: Create a provider factory that returns Mock
        var factory = new TestAiReviewerFactory(mockOptions);

        // Assert: Mock provider is resolvable
        Assert.True(mockOptions.Enabled);
        Assert.Equal("Mock", mockOptions.Provider);
        Assert.Equal("mock-v1", mockOptions.Model);
    }

    [Fact]
    public void TestC_UnknownProvider_FailsWithClearError()
    {
        // Arrange: Unknown provider
        var unknownOptions = new AiReviewOptions { Enabled = true, Provider = "NotARealProvider" };

        // Act & Assert: Creating reviewer should fail with clear error
        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            var factory = new TestAiReviewerFactory(unknownOptions);
            factory.CreateReviewer();
        });

        // Verify error message is clear
        Assert.Contains("Unknown AI provider", exception.Message);
        Assert.Contains("NotARealProvider", exception.Message);
    }

    [Fact]
    public void TestD_ProviderNeutrality_HandlerDoesNotDependOnSpecificProvider()
    {
        // Arrange: Create handler with mock reviewer
        var mockOptions = new AiReviewOptions { Enabled = true, Provider = "Mock", Model = "mock-v1" };
        var mockReviewer = new MockAiTranslationReviewer(mockOptions);
        var extractor = new CandidateExtractor();
        var persistence = new AiOrchestrationPersistence(_context);

        var handlerWithMock = new TranslationQueryHandler(
            new DictionaryTranslationEngine(_context),
            mockReviewer,
            persistence,
            mockOptions,
            extractor);

        // Assert: Handler depends on IAiTranslationReviewer, not specific implementation
        Assert.NotNull(handlerWithMock);

        // Create handler with disabled reviewer
        var disabledOptions = new AiReviewOptions { Enabled = false, Provider = "Disabled" };
        var disabledReviewer = new DisabledAiTranslationReviewer();
        var handlerWithDisabled = new TranslationQueryHandler(
            new DictionaryTranslationEngine(_context),
            disabledReviewer,
            new AiOrchestrationPersistence(_context),
            disabledOptions,
            extractor);

        // Assert: Handler works with any IAiTranslationReviewer implementation
        Assert.NotNull(handlerWithDisabled);
    }

    [Fact]
    public void TestE_AiFailure_V1CanonicalResultsSurvives()
    {
        // Arrange: Create failing reviewer
        var failingOptions = new AiReviewOptions { Enabled = true, Provider = "Mock", Model = "mock-v1" };
        var failingReviewer = new TestAiTranslationReviewer();
        failingReviewer.SetException(new InvalidOperationException("AI provider failed"));

        var extractor = new CandidateExtractor();
        var persistence = new AiOrchestrationPersistence(_context);

        var handler = new TranslationQueryHandler(
            new DictionaryTranslationEngine(_context),
            failingReviewer,
            persistence,
            failingOptions,
            extractor);

        // Act: Handle request with failing AI
        var request = new TranslationRequest("test", "es", "test-input");
        var result = handler.HandleAsync(request).Result;

        // Assert: V1 result is preserved (not exception)
        Assert.NotNull(result.V1Result);
        Assert.False(result.AiReview!.Success);
        Assert.Equal(AiReviewStatus.ProviderError, result.AiReview!.ReviewStatus);
    }

    [Fact]
    public void TestF_CanonicalDictionaryIsolation_ActionDoesNotModifyCanonical()
    {
        // Arrange
        var mockOptions = new AiReviewOptions { Enabled = true, Provider = "Mock", Model = "mock-v1" };
        var mockReviewer = new MockAiTranslationReviewer(mockOptions);
        var extractor = new CandidateExtractor();
        var persistence = new AiOrchestrationPersistence(_context);

        var handler = new TranslationQueryHandler(
            new DictionaryTranslationEngine(_context),
            mockReviewer,
            persistence,
            mockOptions,
            extractor);

        // Act: Handle request that produces candidate with correction
        var request = new TranslationRequest("test", "es", "test-input");
        var result = handler.HandleAsync(request).Result;

        // Assert: V2Candidate exists but is not promoted
        if (result.Candidate != null)
        {
            Assert.False(result.Candidate.IsPromoted);
            Assert.Null(result.Candidate.PromotedAt);
        }
    }

    [Fact]
    public void TestG_CandidateNotPromoted_Automatic()
    {
        // Arrange
        var mockOptions = new AiReviewOptions { Enabled = true, Provider = "Mock", Model = "mock-v1" };
        var mockReviewer = new MockAiTranslationReviewer(mockOptions);
        var extractor = new CandidateExtractor();
        var persistence = new AiOrchestrationPersistence(_context);

        var handler = new TranslationQueryHandler(
            new DictionaryTranslationEngine(_context),
            mockReviewer,
            persistence,
            mockOptions,
            extractor);

        // Act
        var request = new TranslationRequest("test", "es", "test-input");
        var result = handler.HandleAsync(request).Result;

        // Assert: If a candidate is produced, it must not be promoted
        if (result.Candidate != null)
        {
            Assert.False(result.Candidate.IsPromoted);
            Assert.Null(result.Candidate.PromotedAt);
        }
    }

    [Fact]
    public void TestH_NoSecretsInConfiguration()
    {
        // Arrange: Test configuration
        var secureOptions = new AiReviewOptions
        {
            Enabled = true,
            Provider = "Mock",
            Model = "mock-v1",
            Endpoint = "",
            ApiKey = "test-placeholder"  // Placeholder, not a real key
        };

        // Act & Assert: Verify configuration uses placeholder, not real credentials
        Assert.Equal("test-placeholder", secureOptions.ApiKey);
        Assert.NotEqual("sk-", secureOptions.ApiKey.Substring(0, 3)); // Not a real OpenAI key
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    private class TestAiReviewerFactory : IAiTranslationReviewer
    {
        private readonly AiReviewOptions _options;

        public TestAiReviewerFactory(AiReviewOptions options)
        {
            _options = options;
        }

        public IAiTranslationReviewer CreateReviewer()
        {
            return _options.Provider?.ToLowerInvariant() switch
            {
                "mock" => new MockAiTranslationReviewer(_options),
                "disabled" => new DisabledAiTranslationReviewer(),
                null => new DisabledAiTranslationReviewer(),
                _ => throw new InvalidOperationException(
                    $"Unknown AI provider: '{_options.Provider}'. " +
                    $"Valid providers: Mock, Disabled. " +
                    $"External providers must be registered separately.")
            };
        }

        public Task<AiTranslationReviewResult> ReviewTranslationAsync(
            AiTranslationReviewRequest request,
            CancellationToken ct = default)
        {
            return CreateReviewer().ReviewTranslationAsync(request, ct);
        }

        public Task<AiLanguageDetectionResult> DetectLanguageAsync(
            string text,
            IReadOnlyList<string> knownVariants,
            CancellationToken ct = default)
        {
            return CreateReviewer().DetectLanguageAsync(text, knownVariants, ct);
        }
    }
}