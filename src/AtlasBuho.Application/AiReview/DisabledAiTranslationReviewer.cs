using AtlasBuho.Application.AiReview;
using AtlasBuho.Application.Translation;

namespace AtlasBuho.Application.AiReview;

/// <summary>
/// Disabled AI translation reviewer (B10.1 §5).
/// Implements IAiTranslationReviewer but throws if invoked — caller must check AiReviewOptions.Enabled.
/// </summary>
public sealed class DisabledAiTranslationReviewer : IAiTranslationReviewer
{
    public Task<AiTranslationReviewResult> ReviewTranslationAsync(
        AiTranslationReviewRequest request,
        CancellationToken ct = default)
    {
        // This should never be called when AI is disabled
        // The handler checks AiReviewOptions.Enabled before invoking the reviewer
        throw new InvalidOperationException(
            "AI review is disabled. Check AiReviewOptions.Enabled before calling ReviewTranslationAsync.");
    }

    public Task<AiLanguageDetectionResult> DetectLanguageAsync(
        string text,
        IReadOnlyList<string> knownVariants,
        CancellationToken ct = default)
    {
        // This should never be called when AI is disabled
        throw new InvalidOperationException(
            "AI language detection is disabled. Check AiReviewOptions.Enabled before calling DetectLanguageAsync.");
    }
}

/// <summary>
/// Configuration validator for AI options (B10.1 §6).
/// Validates provider selection and required configuration fields.
/// </summary>
public sealed class AiReviewOptionsValidator
{
    public static void Validate(AiReviewOptions options)
    {
        if (options == null)
            throw new ArgumentNullException(nameof(options));

        // Enabled/disabled is a boolean choice — no provider required when disabled
        if (!options.Enabled)
        {
            if (options.Provider != null && options.Provider != "Disabled")
            {
                throw new InvalidOperationException(
                    $"AI is disabled but Provider is set to '{options.Provider}'. " +
                    "Set Provider = 'Disabled' or leave it null.");
            }
            return;
        }

        // Provider is required when enabled
        if (string.IsNullOrWhiteSpace(options.Provider))
        {
            throw new InvalidOperationException(
                "AI is enabled but Provider is not specified. " +
                "Set Provider to 'Mock' or a valid provider name.");
        }

        // Validation of provider name is delegated to the factory registry.
        // This validator only checks that a provider name is present and non-empty.
        // External providers must be registered via IAiTranslationReviewerFactory.
    }
}