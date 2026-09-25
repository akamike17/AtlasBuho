using AtlasBuho.Domain.AiReview;

namespace AtlasBuho.Application.AiReview;

/// <summary>
/// Abstraction for AI translation review providers (B10 §5).
/// AI is an external reviewer and improvement signal — never authoritative.
/// Implementations must live in Infrastructure, not Domain.
/// </summary>
public interface IAiTranslationReviewer
{
    /// <summary>
    /// Reviews a V1 translation result using AI analysis.
    /// </summary>
    /// <param name="request">Review context with V1 result and available evidence</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Structured AI review with status, correction proposal, and metadata</returns>
    Task<AiTranslationReviewResult> ReviewTranslationAsync(
        AiTranslationReviewRequest request,
        CancellationToken ct = default);

    /// <summary>
    /// Detects the language of input text.
    /// Result is NOT canonical — documented variants are authoritative.
    /// </summary>
    /// <param name="text">Input text to analyze</param>
    /// <param name="knownVariants">Known documented variants for context</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>AI language detection result with confidence</returns>
    Task<AiLanguageDetectionResult> DetectLanguageAsync(
        string text,
        IReadOnlyList<string> knownVariants,
        CancellationToken ct = default);
}

/// <summary>
/// Request context for AI translation review (B10 §6).
/// Only relevant evidence is provided — no database dumps.
/// </summary>
public sealed record AiTranslationReviewRequest(
    /// <summary>User's original input text</summary>
    string InputText,

    /// <summary>Requested source language</summary>
    string SourceLanguage,

    /// <summary>Requested target language</summary>
    string TargetLanguage,

    /// <summary>V1 translation result (from DictionaryTranslationEngine)</summary>
    string? V1Translation,

    /// <summary>Known language variant (documented)</summary>
    string? KnownVariant,

    /// <summary>Known lexeme from canonical evidence</summary>
    string? KnownLexeme,

    /// <summary>Known meaning from canonical evidence</summary>
    string? KnownMeaning,

    /// <summary>Documented translation from evidence</summary>
    string? DocumentedTranslation,

    /// <summary>Source/provenance reference</summary>
    string? SourceReference,

    /// <summary>Provider name to use (null = default)</summary>
    string? ProviderName = null,

    /// <summary>Correlation ID for tracing</summary>
    Guid? CorrelationId = null);

/// <summary>
/// Structured AI translation review result (B10 §7).
/// </summary>
public sealed record AiTranslationReviewResult(
    /// <summary>Review status</summary>
    AiReviewStatus Status,

    /// <summary>AI-detected language (if requested)</summary>
    string? DetectedLanguage,

    /// <summary>Language detection confidence</summary>
    double? DetectedLanguageConfidence,

    /// <summary>Proposed correction (null if supported/insufficient)</summary>
    string? ProposedCorrection,

    /// <summary>AI explanation</summary>
    string? Explanation,

    /// <summary>Evidence notes</summary>
    string? EvidenceNotes,

    /// <summary>Provider name</summary>
    string Provider,

    /// <summary>Model name</summary>
    string Model,

    /// <summary>Prompt version</summary>
    string PromptVersion,

    /// <summary>Latency in milliseconds</summary>
    int? LatencyMs,

    /// <summary>Request success</summary>
    bool Success,

    /// <summary>Error code if failed</summary>
    string? ErrorCode);

/// <summary>
/// AI language detection result (B10 §8).
/// NOT canonical — conflicts with documented variants are preserved.
/// </summary>
public sealed record AiLanguageDetectionResult(
    string DetectedLanguage,
    double Confidence,
    string Provider,
    string Model,
    bool ConflictsWithDocumented,
    string? DocumentedVariant);

/// <summary>
/// Configuration for AI review behavior.
/// </summary>
public sealed class AiReviewOptions
{
    /// <summary>Enable/disable AI review (default: false)</summary>
    public bool Enabled { get; set; }

    /// <summary>Provider name (e.g., "OpenAI", "Mock")</summary>
    public string Provider { get; set; } = "Mock";

    /// <summary>Model name (e.g., "gpt-4")</summary>
    public string Model { get; set; } = "mock-model";

    /// <summary>API endpoint</summary>
    public string? Endpoint { get; set; }

    /// <summary>Timeout in seconds</summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>Maximum retries</summary>
    public int MaxRetries { get; set; } = 2;

    /// <summary>Prompt version</summary>
    public string PromptVersion { get; set; } = "translation-review-v1";

    // NOTE: API keys must come from environment variables — never stored in code/config files
    // Set via: ATLASBUHO_AI_API_KEY environment variable
}
