using AtlasBuho.Domain.AiReview;

namespace AtlasBuho.Application.Translation;

/// <summary>
/// Extracts V2 candidates from eligible AI reviews (B10 §13).
/// Candidate extraction requires a meaningful proposed correction and an eligible review outcome.
/// </summary>
public interface ICandidateExtractor
{
    /// <summary>
    /// Extracts a V2Candidate from an AI review if eligible.
    /// Returns null if the review does not warrant a candidate.
    /// </summary>
    /// <param name="review">The AI translation review</param>
    /// <param name="request">Original translation request</param>
    /// <param name="correlationId">Correlation ID for tracing</param>
    /// <returns>V2Candidate in PENDING state, or null if not eligible</returns>
    V2Candidate? Extract(
        AiTranslationReview review,
        TranslationRequest request,
        Guid correlationId);
}

/// <summary>
/// Default implementation of candidate extraction logic.
/// Creates V2Candidate with IsPromoted = false (B10 §8, §13).
/// </summary>
public sealed class CandidateExtractor : ICandidateExtractor
{
    public V2Candidate? Extract(
        AiTranslationReview review,
        TranslationRequest request,
        Guid correlationId)
    {
        // Only extract if there's a meaningful proposed correction
        if (string.IsNullOrWhiteSpace(review.ProposedCorrection))
            return null;

        // Only from eligible review statuses
        if (review.ReviewStatus is not (AiReviewStatus.PotentialIssue or AiReviewStatus.LikelyIncorrect))
            return null;

        // Create candidate in PENDING state (never auto-promoted)
        return new V2Candidate
        {
            AiReviewId = review.Id,
            CanonicalForm = request.Text,
            SpanishMeaning = review.AtlasBuhoV1Result, // V1 result as baseline
            TargetLanguage = request.TargetLanguage,
            TargetText = review.ProposedCorrection,
            SourceVariant = request.SourceLanguage,
            OriginalConfidence = review.DetectedLanguageConfidence,
            CreatedBy = "ai-review-orchestrator",
            IsPromoted = false // NEVER auto-promote (B10 §8)
        };
    }
}