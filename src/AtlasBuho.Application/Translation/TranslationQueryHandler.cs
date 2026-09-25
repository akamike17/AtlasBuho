using AtlasBuho.Application.AiReview;
using AtlasBuho.Domain.AiReview;

namespace AtlasBuho.Application.Translation;

/// <summary>
/// Application-level orchestrator for translation queries with optional AI review (B10 §3-§5).
/// 
/// <para><b>Flow:</b></para>
/// <para>1. V1 translation via ITranslationEngine (canonical, evidence-based)</para>
/// <para>2. If AI enabled: review via IAiTranslationReviewer (non-canonical)</para>
/// <para>3. Persist AiTranslationReview (audit trail)</para>
/// <para>4. If eligible correction: create V2Candidate (PENDING, never auto-promoted)</para>
/// <para>5. Return V1 result (AI never overrides canonical translation)</para>
/// 
/// <para><b>Critical invariant:</b> AI cannot mutate canonical V1 data.
/// All canonical entities (Lexeme, Meaning, LexicalEquivalence, CatalogVersion, Source)
/// remain unchanged regardless of AI review outcome.</para>
/// </summary>
public sealed class TranslationQueryHandler
{
    private readonly ITranslationEngine _translationEngine;
    private readonly IAiTranslationReviewer? _aiReviewer;
    private readonly IAiOrchestrationPersistence _persistence;
    private readonly AiReviewOptions _aiOptions;
    private readonly ICandidateExtractor _candidateExtractor;

    /// <summary>
    /// Creates a translation query orchestrator.
    /// </summary>
    /// <param name="translationEngine">Canonical V1 translation engine (required)</param>
    /// <param name="aiReviewer">AI translation reviewer (optional, may be null if AI disabled)</param>
    /// <param name="dbContext">Database context for persistence</param>
    /// <param name="aiOptions">AI configuration options</param>
    /// <param name="candidateExtractor">V2 candidate extraction logic</param>
    public TranslationQueryHandler(
        ITranslationEngine translationEngine,
        IAiTranslationReviewer? aiReviewer,
        IAiOrchestrationPersistence persistence,
        AiReviewOptions aiOptions,
        ICandidateExtractor candidateExtractor)
    {
        _translationEngine = translationEngine ?? throw new ArgumentNullException(nameof(translationEngine));
        _aiReviewer = aiReviewer;
        _persistence = persistence ?? throw new ArgumentNullException(nameof(persistence));
        _aiOptions = aiOptions ?? new AiReviewOptions();
        _candidateExtractor = candidateExtractor ?? throw new ArgumentNullException(nameof(candidateExtractor));
    }

    /// <summary>
    /// Handles a translation query with optional AI review.
    /// </summary>
    /// <remarks>
    /// This is the real executable path that B10 requires:
    /// 
    /// <code>
    /// Input
    ///   → ITranslationEngine.TranslateAsync (V1 canonical)
    ///   → IF AI enabled: IAiTranslationReviewer.ReviewTranslationAsync
    ///   → Persist AiTranslationReview
    ///   → IF eligible: CandidateExtractor.Extract
    ///   → Persist V2Candidate (PENDING, IsPromoted = false)
    ///   → Return V1 result (never AI's correction)
    /// </code>
    /// 
    /// Critical invariant: AI cannot mutate canonical V1 data.
    /// </remarks>
    /// <param name="request">Translation request</param>
    /// <param name="correlationId">Optional correlation ID for tracing</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Translation result with optional AI review metadata</returns>
    public async Task<TranslationOrchestrationResult> HandleAsync(
        TranslationRequest request,
        Guid? correlationId = null,
        CancellationToken ct = default)
    {
        correlationId ??= Guid.NewGuid();

        // Step 1: V1 canonical translation (always executed, always returned)
        var v1Result = await _translationEngine.TranslateAsync(request, ct);

        // Step 2: If AI disabled or no reviewer configured, return V1 only
        if (!_aiOptions.Enabled || _aiReviewer == null)
        {
            return TranslationOrchestrationResult.FromV1Only(v1Result, correlationId.Value);
        }

        // Step 3: AI review (non-destructive)
        AiTranslationReview? aiReview = null;
        try
        {
            aiReview = await PerformAiReviewAsync(request, v1Result, correlationId.Value, ct);
        }
        catch (Exception ex)
        {
            // AI failure must not affect V1 (B10 §11)
            aiReview = CreateProviderErrorReview(request, v1Result, correlationId.Value, ex);
        }

        // Step 4: Persist AI review (audit trail)
        await _persistence.AddAiReviewAsync(aiReview, ct);
        await _persistence.SaveChangesAsync(ct);

        // Step 5: Extract V2 candidate if eligible (never auto-promoted)
        V2Candidate? candidate = null;
        if (ShouldExtractCandidate(aiReview))
        {
            candidate = _candidateExtractor.Extract(aiReview, request, correlationId.Value);
            if (candidate != null)
            {
                await _persistence.AddV2CandidateAsync(candidate, ct);
                await _persistence.SaveChangesAsync(ct);
            }
        }

        // Step 6: Return V1 + review metadata (V1 is always the canonical result)
        return TranslationOrchestrationResult.FromV1WithAiReview(
            v1Result,
            aiReview,
            candidate,
            correlationId.Value);
    }

    private async Task<AiTranslationReview> PerformAiReviewAsync(
        TranslationRequest request,
        TranslationResult v1Result,
        Guid correlationId,
        CancellationToken ct)
    {
        var reviewRequest = new AiTranslationReviewRequest(
            InputText: request.Text,
            SourceLanguage: request.SourceLanguage ?? string.Empty,
            TargetLanguage: request.TargetLanguage ?? string.Empty,
            V1Translation: v1Result.Translation,
            KnownVariant: request.SourceLanguage,
            KnownLexeme: null,
            KnownMeaning: v1Result.Translation,
            DocumentedTranslation: v1Result.Translation,
            SourceReference: null,
            ProviderName: _aiOptions.Provider,
            CorrelationId: correlationId);

        var reviewResult = await _aiReviewer!.ReviewTranslationAsync(reviewRequest, ct);

        return new AiTranslationReview
        {
            InputText = request.Text,
            SourceLanguageRequested = request.SourceLanguage ?? string.Empty,
            TargetLanguageRequested = request.TargetLanguage ?? string.Empty,
            DetectedLanguage = reviewResult.DetectedLanguage,
            DetectedLanguageConfidence = reviewResult.DetectedLanguageConfidence,
            AtlasBuhoV1Result = v1Result.Translation,
            ReviewStatus = reviewResult.Status,
            ProposedCorrection = reviewResult.ProposedCorrection,
            Explanation = reviewResult.Explanation,
            EvidenceNotes = reviewResult.EvidenceNotes,
            ProviderName = reviewResult.Provider,
            ModelName = reviewResult.Model,
            PromptVersion = reviewResult.PromptVersion,
            LatencyMs = reviewResult.LatencyMs,
            Success = reviewResult.Success,
            ErrorCode = reviewResult.ErrorCode,
            CorrelationId = correlationId
        };
    }

    private AiTranslationReview CreateProviderErrorReview(
        TranslationRequest request,
        TranslationResult v1Result,
        Guid correlationId,
        Exception ex)
    {
        return new AiTranslationReview
        {
            InputText = request.Text,
            SourceLanguageRequested = request.SourceLanguage ?? string.Empty,
            TargetLanguageRequested = request.TargetLanguage ?? string.Empty,
            AtlasBuhoV1Result = v1Result.Translation,
            ReviewStatus = AiReviewStatus.ProviderError,
            ProviderName = _aiOptions.Provider,
            ModelName = _aiOptions.Model,
            PromptVersion = _aiOptions.PromptVersion,
            Success = false,
            ErrorCode = ex.GetType().Name,
            CorrelationId = correlationId
        };
    }

    private static bool ShouldExtractCandidate(AiTranslationReview review)
    {
        // INSUFFICIENT_EVIDENCE: persist review but no candidate (B10 §12)
        if (review.ReviewStatus == AiReviewStatus.InsufficientEvidence)
            return false;

        // Provider error: no candidate
        if (review.ReviewStatus == AiReviewStatus.ProviderError)
            return false;

        // No correction proposed: no candidate
        if (string.IsNullOrWhiteSpace(review.ProposedCorrection))
            return false;

        // Only extract from statuses that indicate a correction was proposed
        return review.ReviewStatus is AiReviewStatus.PotentialIssue or AiReviewStatus.LikelyIncorrect;
    }
}

/// <summary>
/// Result of the full orchestration: V1 + optional AI review + optional V2 candidate.
/// </summary>
public sealed record TranslationOrchestrationResult(
    TranslationResult V1Result,
    AiTranslationReview? AiReview,
    V2Candidate? Candidate,
    Guid CorrelationId)
{
    /// <summary>
    /// Creates result when AI is disabled or not configured.
    /// </summary>
    public static TranslationOrchestrationResult FromV1Only(TranslationResult v1Result, Guid correlationId)
        => new(v1Result, null, null, correlationId);

    /// <summary>
    /// Creates result with AI review and optional candidate.
    /// </summary>
    public static TranslationOrchestrationResult FromV1WithAiReview(
        TranslationResult v1Result,
        AiTranslationReview? aiReview,
        V2Candidate? candidate,
        Guid correlationId)
        => new(v1Result, aiReview, candidate, correlationId);
}