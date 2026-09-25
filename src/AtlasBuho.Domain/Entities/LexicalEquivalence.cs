using System.Security.Cryptography;
using System.Text;

namespace AtlasBuho.Domain.Entities;

/// <summary>
/// ADR 0001 — A single piece of lexical-translation EVIDENCE.
///
/// One row = one verified piece of evidence that a source lexeme (in exactly one
/// LanguageVariant, see ADR 0001 I4/P0) has an attested translation in a target language.
/// Nothing about the reverse direction is inferred — the Spanish→Indigenous direction
/// would need its own explicitly evidenced rows (I2).
///
/// Canonical-marking invariant (I1): at most ONE row may be canonical for the same
/// (SourceLexemeId, TargetLanguage, CatalogVersionId). Two canonical rows are an
/// integrity violation, never disambiguated by ordering or text.
///
/// Versioning invariant (I3): Completed CatalogVersions are immutable. Evidence belongs
/// to its CatalogVersionId and is never mutated in place; new evidence means a new
/// catalog/corpus version, never a silent overwrite of a completed one.
/// </summary>
public class LexicalEquivalence
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid SourceLexemeId { get; private set; }
    public string TargetLanguage { get; private set; } = string.Empty; // "es" | "en" (ADR 0001 §2)
    public string TargetText { get; private set; } = string.Empty;
    public bool IsCanonical { get; private set; }
    public VerificationStatus VerificationStatus { get; private set; } = VerificationStatus.Unknown;
    public Guid SourceId { get; private set; }
    public Guid CatalogVersionId { get; private set; }
    public string RowHash { get; private set; } = string.Empty; // SHA256 over the evidence fields (I3 integrity)
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public Lexeme? SourceLexeme { get; private set; }
    public Source? EvidenceSource { get; private set; }
    public CatalogVersion? CatalogVersion { get; private set; }

    private LexicalEquivalence() { }

    public LexicalEquivalence(
        Guid sourceLexemeId,
        string targetLanguage,
        string targetText,
        bool isCanonical,
        VerificationStatus verificationStatus,
        Guid sourceId,
        Guid catalogVersionId)
    {
        if (string.IsNullOrWhiteSpace(targetLanguage))
            throw new ArgumentNullException(nameof(targetLanguage));
        if (string.IsNullOrWhiteSpace(targetText))
            throw new ArgumentNullException(nameof(targetText));

        SourceLexemeId = sourceLexemeId;
        TargetLanguage = targetLanguage.Trim().ToLowerInvariant();
        TargetText = targetText.Trim();
        IsCanonical = isCanonical;
        VerificationStatus = verificationStatus;
        SourceId = sourceId;
        CatalogVersionId = catalogVersionId;
    }

    /// Deterministic row integrity hash. Used to detect any silent mutation of evidence.
    public string ComputeRowHash()
    {
        var canonical = $"{SourceLexemeId}|{TargetLanguage}|{TargetText}|{IsCanonical}|{(int)VerificationStatus}|{SourceId}|{CatalogVersionId}";
        using var sha256 = SHA256.Create();
        return Convert.ToHexString(sha256.ComputeHash(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
    }
}
