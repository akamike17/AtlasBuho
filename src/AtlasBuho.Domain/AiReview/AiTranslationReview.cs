namespace AtlasBuho.Domain.AiReview;

/// <summary>
/// Status of an AI translation review. AI is never authoritative — it only reviews V1 evidence.
/// Per B10 §7: no state implies AI alone established linguistic truth.
/// </summary>
public enum AiReviewStatus
{
    /// <summary>Review not yet performed</summary>
    NotReviewed = 0,

    /// <summary>AI agrees with V1 translation (documented evidence confirmed)</summary>
    Supported = 1,

    /// <summary>AI detected a potential issue (requires human review)</summary>
    PotentialIssue = 2,

    /// <summary>AI believes V1 translation is likely incorrect (requires evidence review)</summary>
    LikelyIncorrect = 3,

    /// <summary>AI cannot establish a correction (insufficient context/evidence)</summary>
    InsufficientEvidence = 4,

    /// <summary>Provider error occurred (timeout, rate limit, unavailable)</summary>
    ProviderError = 5
}

/// <summary>
/// Result of AI-based language identification. NOT canonical — DOCUMENTED variants are authoritative.
/// </summary>
public sealed record AiLanguageDetection(
    string DetectedLanguage,
    double Confidence,
    string Provider,
    string Model,
    DateTime DetectedAt);

/// <summary>
/// Structured AI translation review response (B10 §7).
/// Captures the complete review context for auditability and V2 candidate extraction.
/// </summary>
public sealed class AiTranslationReview
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Original user input text</summary>
    public string InputText { get; set; } = string.Empty;

    /// <summary>Requested source language</summary>
    public string SourceLanguageRequested { get; set; } = string.Empty;

    /// <summary>Requested target language</summary>
    public string TargetLanguageRequested { get; set; } = string.Empty;

    /// <summary>AI-detected language (may differ from requested)</summary>
    public string? DetectedLanguage { get; set; }

    /// <summary>Confidence of language detection (0.0 - 1.0)</summary>
    public double? DetectedLanguageConfidence { get; set; }

    /// <summary>V1 translation result (from DictionaryTranslationEngine)</summary>
    public string? AtlasBuhoV1Result { get; set; }

    /// <summary>AI assessment of the V1 translation</summary>
    public AiReviewStatus ReviewStatus { get; set; } = AiReviewStatus.NotReviewed;

    /// <summary>Proposed correction (null if insufficient evidence or supported)</summary>
    public string? ProposedCorrection { get; set; }

    /// <summary>AI explanation of the assessment</summary>
    public string? Explanation { get; set; }

    /// <summary>Evidence notes provided by AI (references to context used)</summary>
    public string? EvidenceNotes { get; set; }

    /// <summary>Provider name (e.g., "OpenAI", "NVIDIA", "Mock")</summary>
    public string ProviderName { get; set; } = string.Empty;

    /// <summary>Model name (e.g., "gpt-4", "llama3-70b")</summary>
    public string ModelName { get; set; } = string.Empty;

    /// <summary>Prompt template version (e.g., "translation-review-v1")</summary>
    public string PromptVersion { get; set; } = string.Empty;

    /// <summary>Response schema version (e.g., "v1")</summary>
    public string ResponseSchemaVersion { get; set; } = "v1";

    /// <summary>AI provider latency in milliseconds (if available)</summary>
    public int? LatencyMs { get; set; }

    /// <summary>Request success</summary>
    public bool Success { get; set; }

    /// <summary>Error code if provider failed</summary>
    public string? ErrorCode { get; set; }

    /// <summary>Correlation ID for distributed tracing</summary>
    public Guid? CorrelationId { get; set; }

    /// <summary>Candidate lifecycle state (B10 §15)</summary>
    public AiCandidateLifecycle Lifecycle { get; set; } = AiCandidateLifecycle.Pending;

    /// <summary>Human review timestamp (when reviewed)</summary>
    public DateTime? ReviewedAt { get; set; }

    /// <summary>Human reviewer notes</summary>
    public string? HumanReviewNotes { get; set; }

    /// <summary>Promotion timestamp (when promoted to V2 candidate)</summary>
    public DateTime? PromotedAt { get; set; }

    /// <summary>Associated V2 candidate entity ID (if promoted)</summary>
    public Guid? V2CandidateId { get; set; }
}

/// <summary>
/// Lifecycle state for AI-proposed V2 candidates (B10 §15).
/// Enforces: AI cannot promote directly to PROMOTED without human review.
/// </summary>
public enum AiCandidateLifecycle
{
    /// <summary>Created, awaiting review</summary>
    Pending = 0,

    /// <summary>Under human/evidence review</summary>
    UnderReview = 1,

    /// <summary>Rejected after review (remains logged for diagnostics)</summary>
    Rejected = 2,

    /// <summary>Accepted after review</summary>
    Accepted = 3,

    /// <summary>Explicitly marked as V2 candidate</summary>
    V2Candidate = 4,

    /// <summary>Promoted to canonical (only via explicit evidence process)</summary>
    Promoted = 5
}

/// <summary>
/// V2 candidate extracted from accepted AI reviews (B10 §14).
/// Not canonical — requires explicit promotion workflow.
/// </summary>
public sealed class V2Candidate
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Source AI review ID</summary>
    public Guid AiReviewId { get; set; }
    public AiTranslationReview AiReview { get; set; } = null!;

    /// <summary>Canonical form (proposed)</summary>
    public string CanonicalForm { get; set; } = string.Empty;

    /// <summary>Spanish meaning (proposed)</summary>
    public string? SpanishMeaning { get; set; }

    /// <summary>Target language for equivalence (proposed)</summary>
    public string? TargetLanguage { get; set; }

    /// <summary>Target text (proposed)</summary>
    public string? TargetText { get; set; }

    /// <summary>Source language variant (proposed)</summary>
    public string? SourceVariant { get; set; }

    /// <summary>Confidence score from original AI review</summary>
    public double? OriginalConfidence { get; set; }

    /// <summary>Human operator who created the candidate</summary>
    public string? CreatedBy { get; set; }

    /// <summary>Promotion evidence reference (source ID)</summary>
    public Guid? PromotionEvidenceId { get; set; }

    /// <summary>Is this candidate promoted to canonical?</summary>
    public bool IsPromoted { get; set; }

    /// <summary>Promotion timestamp</summary>
    public DateTime? PromotedAt { get; set; }
}
