using AtlasBuho.Domain.AiReview;

namespace AtlasBuho.Application.AiReview;

/// <summary>
/// Factory for AI translation reviewers (B10.1 §4).
/// Allows external providers to be registered without modifying Domain/Application core.
/// </summary>
public interface IAiTranslationReviewerFactory
{
    /// <summary>
    /// Returns true if this factory can create a reviewer for the given provider name.
    /// </summary>
    bool CanResolve(string providerName);

    /// <summary>
    /// Creates the AI translation reviewer from configuration.
    /// </summary>
    IAiTranslationReviewer Create(AiReviewOptions options);
}

/// <summary>
/// Registry of AI translation reviewer factories (B10.1 §4).
/// Resolves provider by name through registered factories.
/// </summary>
public interface IAiTranslationReviewerRegistry
{
    /// <summary>
    /// Resolves an AI translation reviewer for the given provider name.
    /// </summary>
    IAiTranslationReviewer Resolve(string providerName, AiReviewOptions options);
}

/// <summary>
/// Default implementation of the provider registry.
/// External providers register additional factories in DI.
/// </summary>
public sealed class AiTranslationReviewerRegistry : IAiTranslationReviewerRegistry
{
    private readonly IEnumerable<IAiTranslationReviewerFactory> _factories;

    public AiTranslationReviewerRegistry(IEnumerable<IAiTranslationReviewerFactory> factories)
    {
        _factories = factories ?? throw new ArgumentNullException(nameof(factories));
    }

    public IAiTranslationReviewer Resolve(string providerName, AiReviewOptions options)
    {
        var providerKey = string.IsNullOrWhiteSpace(providerName) ? "Disabled" : providerName;
        var factory = _factories.FirstOrDefault(f => f.CanResolve(providerKey));

        if (factory == null)
        {
            var available = string.Join(", ",
                _factories.Select(f => f.GetType().Name.Replace("Factory", "")));
            throw new InvalidOperationException(
                $"Unknown AI provider: '{providerName}'. " +
                $"Registered providers: {available}. " +
                $"External providers must be registered via IAiTranslationReviewerFactory.");
        }

        return factory.Create(options);
    }
}