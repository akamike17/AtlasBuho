using AtlasBuho.Application.AiReview;
using AtlasBuho.Infrastructure.AiProviders;

namespace AtlasBuho.Data.Extensions;

/// <summary>
/// Factory for Mock AI provider (B10.1 §5).
/// Lives in Infrastructure alongside the Mock implementation.
/// </summary>
public sealed class MockAiTranslationReviewerFactory : IAiTranslationReviewerFactory
{
    public bool CanResolve(string providerName)
        => providerName.Equals("Mock", StringComparison.OrdinalIgnoreCase);

    public IAiTranslationReviewer Create(AiReviewOptions options)
        => new MockAiTranslationReviewer(options);
}

/// <summary>
/// Factory for Disabled AI provider (B10.1 §5).
/// </summary>
public sealed class DisabledAiTranslationReviewerFactory : IAiTranslationReviewerFactory
{
    public bool CanResolve(string providerName)
        => providerName.Equals("Disabled", StringComparison.OrdinalIgnoreCase) ||
           string.IsNullOrWhiteSpace(providerName);

    public IAiTranslationReviewer Create(AiReviewOptions options)
        => new DisabledAiTranslationReviewer();
}