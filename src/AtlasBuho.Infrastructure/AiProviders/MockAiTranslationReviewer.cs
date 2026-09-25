using AtlasBuho.Application.AiReview;
using AtlasBuho.Domain.AiReview;

namespace AtlasBuho.Infrastructure.AiProviders;

/// <summary>
/// Mock AI translation reviewer for testing (B10 §22).
/// Does not call external services — returns deterministic responses based on input hash.
/// </summary>
public sealed class MockAiTranslationReviewer : IAiTranslationReviewer
{
    private readonly AiReviewOptions _options;

    public MockAiTranslationReviewer(AiReviewOptions? options = null)
    {
        _options = options ?? new AiReviewOptions();
    }

    public Task<AiTranslationReviewResult> ReviewTranslationAsync(
        AiTranslationReviewRequest request,
        CancellationToken ct = default)
    {
        // Deterministic response based on input text hash for reproducibility
        var hash = request.InputText.GetHashCode();

        // Simulate different review outcomes based on hash
        var status = (Math.Abs(hash) % 5) switch
        {
            0 => AiReviewStatus.Supported,
            1 => AiReviewStatus.PotentialIssue,
            2 => AiReviewStatus.LikelyIncorrect,
            3 => AiReviewStatus.InsufficientEvidence,
            _ => AiReviewStatus.Supported
        };

        var proposedCorrection = status >= AiReviewStatus.PotentialIssue && status <= AiReviewStatus.LikelyIncorrect
            ? GenerateMockCorrection(request.V1Translation, request.InputText)
            : null;

        var result = new AiTranslationReviewResult(
            Status: status,
            DetectedLanguage: request.SourceLanguage,
            DetectedLanguageConfidence: 0.85,
            ProposedCorrection: proposedCorrection,
            Explanation: GenerateMockExplanation(status, request),
            EvidenceNotes: request.KnownVariant != null
                ? $"Using documented variant: {request.KnownVariant}"
                : "No documented variant available",
            Provider: "Mock",
            Model: _options.Model,
            PromptVersion: _options.PromptVersion,
            LatencyMs: 5,
            Success: true,
            ErrorCode: null);

        return Task.FromResult(result);
    }

    public Task<AiLanguageDetectionResult> DetectLanguageAsync(
        string text,
        IReadOnlyList<string> knownVariants,
        CancellationToken ct = default)
    {
        // Mock detection: return first known variant with high confidence
        // or "unknown" with low confidence if no variants provided
        var detected = knownVariants.Count > 0 ? knownVariants[0] : "unknown";
        var confidence = knownVariants.Count > 0 ? 0.92 : 0.15;
        var conflicts = knownVariants.Count > 0 && detected != knownVariants[0];

        var result = new AiLanguageDetectionResult(
            DetectedLanguage: detected,
            Confidence: confidence,
            Provider: "Mock",
            Model: _options.Model,
            ConflictsWithDocumented: conflicts,
            DocumentedVariant: knownVariants.Count > 0 ? knownVariants[0] : null);

        return Task.FromResult(result);
    }

    private static string GenerateMockCorrection(string? v1Translation, string inputText)
    {
        if (string.IsNullOrEmpty(v1Translation))
            return $"correction_for_{inputText.GetHashCode():X}";

        return $"alt_{v1Translation.GetHashCode():X}";
    }

    private static string GenerateMockExplanation(AiReviewStatus status, AiTranslationReviewRequest request)
    {
        return status switch
        {
            AiReviewStatus.Supported => "V1 translation is consistent with documented evidence.",
            AiReviewStatus.PotentialIssue => "Potential ambiguity detected; verify with additional sources.",
            AiReviewStatus.LikelyIncorrect => "V1 result appears inconsistent with contextual analysis.",
            AiReviewStatus.InsufficientEvidence => "Insufficient context to establish a correction.",
            AiReviewStatus.ProviderError => "Simulated provider error (none in mock).",
            _ => "No review performed."
        };
    }
}

/// <summary>
/// AI review service that orchestrates between V1 translation and AI review (B10 §19).
/// AI is optional and downstream from evidence-based V1.
/// </summary>
public sealed class AiTranslationReviewService
{
    private readonly IAiTranslationReviewer _reviewer;
    private readonly AiReviewOptions _options;

    public AiTranslationReviewService(
        IAiTranslationReviewer reviewer,
        AiReviewOptions? options = null)
    {
        _reviewer = reviewer;
        _options = options ?? new AiReviewOptions();
    }

    /// <summary>
    /// Reviews a V1 translation with AI (if enabled).
    /// Returns null if AI is disabled or provider fails.
    /// </summary>
    public async Task<AiTranslationReview?> ReviewIfEnabledAsync(
        AiTranslationReviewRequest request,
        CancellationToken ct = default)
    {
        if (!_options.Enabled)
            return null;

        try
        {
            var result = await _reviewer.ReviewTranslationAsync(request, ct);

            return new AiTranslationReview
            {
                InputText = request.InputText,
                SourceLanguageRequested = request.SourceLanguage,
                TargetLanguageRequested = request.TargetLanguage,
                DetectedLanguage = result.DetectedLanguage,
                DetectedLanguageConfidence = result.DetectedLanguageConfidence,
                AtlasBuhoV1Result = request.V1Translation,
                ReviewStatus = result.Status,
                ProposedCorrection = result.ProposedCorrection,
                Explanation = result.Explanation,
                EvidenceNotes = result.EvidenceNotes,
                ProviderName = result.Provider,
                ModelName = result.Model,
                PromptVersion = result.PromptVersion,
                LatencyMs = result.LatencyMs,
                Success = result.Success,
                ErrorCode = result.ErrorCode,
                CorrelationId = request.CorrelationId
            };
        }
        catch (Exception ex)
        {
            // Log and return null — V1 must continue working (B10 §19)
            return new AiTranslationReview
            {
                InputText = request.InputText,
                SourceLanguageRequested = request.SourceLanguage,
                TargetLanguageRequested = request.TargetLanguage,
                AtlasBuhoV1Result = request.V1Translation,
                ReviewStatus = AiReviewStatus.ProviderError,
                ProviderName = _options.Provider,
                ModelName = _options.Model,
                PromptVersion = _options.PromptVersion,
                Success = false,
                ErrorCode = ex.GetType().Name,
                CorrelationId = request.CorrelationId
            };
        }
    }
}
