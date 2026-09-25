using AtlasBuho.Domain.AiReview;

namespace AtlasBuho.Application.Translation;

/// <summary>
/// Minimal persistence abstraction for AI orchestration (B10 §19).
/// Application layer uses this instead of concrete DbContext to maintain Clean Architecture.
/// </summary>
public interface IAiOrchestrationPersistence
{
    /// <summary>
    /// Adds a new AI translation review to the persistence store.
    /// </summary>
    Task AddAiReviewAsync(AiTranslationReview review, CancellationToken ct = default);

    /// <summary>
    /// Adds a new V2 candidate to the persistence store.
    /// </summary>
    Task AddV2CandidateAsync(V2Candidate candidate, CancellationToken ct = default);

    /// <summary>
    /// Saves all pending changes to the persistence store.
    /// </summary>
    Task SaveChangesAsync(CancellationToken ct = default);
}