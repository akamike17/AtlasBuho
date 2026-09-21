namespace AtlasBuho.Domain.Entities;

public class Orthography
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid LanguageVariantId { get; private set; }
    public Guid? WritingSystemId { get; private set; }
    public string Grapheme { get; private set; } = string.Empty;
    public string? IpaEquivalent { get; private set; }
    public string? Description { get; private set; }
    public string? PositionalRules { get; private set; }
    public string? Allophones { get; private set; }
    public string? Source { get; private set; }
    public VerificationStatus VerificationStatus { get; private set; } = VerificationStatus.Unknown;
    public double Confidence { get; private set; } = 0.0;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private Orthography() { }

    public Orthography(
        Guid languageVariantId,
        string grapheme,
        string? ipaEquivalent,
        string? description,
        string? positionalRules,
        string? allophones,
        string? source,
        VerificationStatus verificationStatus = VerificationStatus.Unknown,
        double confidence = 0.0,
        Guid? writingSystemId = null)
    {
        LanguageVariantId = languageVariantId;
        Grapheme = grapheme ?? throw new ArgumentNullException(nameof(grapheme));
        IpaEquivalent = ipaEquivalent;
        Description = description;
        PositionalRules = positionalRules;
        Allophones = allophones;
        Source = source;
        VerificationStatus = verificationStatus;
        Confidence = confidence;
        WritingSystemId = writingSystemId;
    }

    public void Update(
        string grapheme,
        string? ipaEquivalent,
        string? description,
        string? positionalRules,
        string? allophones,
        string? source,
        VerificationStatus verificationStatus,
        double confidence,
        Guid? writingSystemId = null)
    {
        Grapheme = grapheme ?? throw new ArgumentNullException(nameof(grapheme));
        IpaEquivalent = ipaEquivalent;
        Description = description;
        PositionalRules = positionalRules;
        Allophones = allophones;
        Source = source;
        VerificationStatus = verificationStatus;
        Confidence = confidence;
        WritingSystemId = writingSystemId;
        UpdatedAt = DateTime.UtcNow;
    }
}