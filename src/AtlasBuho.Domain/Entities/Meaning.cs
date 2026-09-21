namespace AtlasBuho.Domain.Entities;

public class Meaning
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid LexemeId { get; private set; }
    public string SpanishMeaning { get; private set; } = string.Empty;
    public string? EnglishMeaning { get; private set; }
    public string? PartOfSpeech { get; private set; }
    public string? SemanticDomain { get; private set; }
    public string? Register { get; private set; }
    public string? RegionalNotes { get; private set; }
    public string? Source { get; private set; }
    public VerificationStatus VerificationStatus { get; private set; } = VerificationStatus.Unknown;
    public double Confidence { get; private set; } = 0.0;
    public int Order { get; private set; } = 0;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private readonly List<Evidence> _evidence = new();
    public IReadOnlyCollection<Evidence> Evidence => _evidence.AsReadOnly();

    private Meaning() { }

    public Meaning(
        Guid lexemeId,
        string spanishMeaning,
        string? englishMeaning,
        string? partOfSpeech,
        string? semanticDomain,
        string? register,
        string? regionalNotes,
        string? source,
        VerificationStatus verificationStatus = VerificationStatus.Unknown,
        double confidence = 0.0,
        int order = 0)
    {
        LexemeId = lexemeId;
        SpanishMeaning = spanishMeaning ?? throw new ArgumentNullException(nameof(spanishMeaning));
        EnglishMeaning = englishMeaning;
        PartOfSpeech = partOfSpeech;
        SemanticDomain = semanticDomain;
        Register = register;
        RegionalNotes = regionalNotes;
        Source = source;
        VerificationStatus = verificationStatus;
        Confidence = confidence;
        Order = order;
    }

    public void Update(
        string spanishMeaning,
        string? englishMeaning,
        string? partOfSpeech,
        string? semanticDomain,
        string? register,
        string? regionalNotes,
        string? source,
        VerificationStatus verificationStatus,
        double confidence,
        int order)
    {
        SpanishMeaning = spanishMeaning ?? throw new ArgumentNullException(nameof(spanishMeaning));
        EnglishMeaning = englishMeaning;
        PartOfSpeech = partOfSpeech;
        SemanticDomain = semanticDomain;
        Register = register;
        RegionalNotes = regionalNotes;
        Source = source;
        VerificationStatus = verificationStatus;
        Confidence = confidence;
        Order = order;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddEvidence(Evidence evidence)
    {
        if (evidence == null) throw new ArgumentNullException(nameof(evidence));
        _evidence.Add(evidence);
    }
}