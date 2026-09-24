using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Application.Translation;

/// 6B.md §21: translation result separates status, match type, verification status and
/// provenance; one field is never overloaded to represent all states.
public sealed record TranslationRequest(string SourceLanguage, string TargetLanguage, string Text);

public sealed record TranslationAlternative(string Text, string MatchType, string VerificationStatus);

public sealed record TranslationResult(
    TranslationStatus Status,
    string SourceLanguage,
    string TargetLanguage,
    string Input,
    string? Translation,
    string? MatchType,
    string? VerificationStatus,
    string DatasetVersion,
    string EngineVersion,
    IReadOnlyList<TranslationAlternative> Alternatives)
{
    public static TranslationResult InvalidInput(string source, string target)
        => new(TranslationStatus.InvalidInput, source, target, string.Empty, null, null, null,
               string.Empty, string.Empty, Array.Empty<TranslationAlternative>());

    public static TranslationResult UnsupportedLanguage(string source, string target, string input,
        string datasetVersion, string engineVersion)
        => new(TranslationStatus.UnsupportedLanguage, source, target, input, null, null, null,
               datasetVersion, engineVersion, Array.Empty<TranslationAlternative>());

    public static TranslationResult NotFound(string source, string target, string input,
        string datasetVersion, string engineVersion)
        => new(TranslationStatus.NotFound, source, target, input, null, null, null,
               datasetVersion, engineVersion, Array.Empty<TranslationAlternative>());
}

public enum TranslationStatus
{
    Translated,
    NotFound,
    PendingVerification,
    UnsupportedLanguage,
    InvalidInput
}
