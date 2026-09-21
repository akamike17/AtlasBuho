namespace AtlasBuho.Domain.Entities;

public class GrammarRule
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid LanguageVariantId { get; private set; }
    public string Category { get; private set; } = string.Empty; // Morphology, Syntax, Phonology, etc.
    public string Subcategory { get; private set; } = string.Empty; // Conjugation, Word Order, Particles, etc.
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? Pattern { get; private set; }
    public string? Examples { get; private set; }
    public string? Notes { get; private set; }
    public string? Source { get; private set; }
    public VerificationStatus VerificationStatus { get; private set; } = VerificationStatus.Unknown;
    public double Confidence { get; private set; } = 0.0;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private readonly List<Evidence> _evidence = new();
    public IReadOnlyCollection<Evidence> Evidence => _evidence.AsReadOnly();

    private GrammarRule() { }

    public GrammarRule(
        Guid languageVariantId,
        string category,
        string subcategory,
        string name,
        string description,
        string? pattern,
        string? examples,
        string? notes,
        string? source,
        VerificationStatus verificationStatus = VerificationStatus.Unknown,
        double confidence = 0.0)
    {
        LanguageVariantId = languageVariantId;
        Category = category ?? throw new ArgumentNullException(nameof(category));
        Subcategory = subcategory ?? throw new ArgumentNullException(nameof(subcategory));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Pattern = pattern;
        Examples = examples;
        Notes = notes;
        Source = source;
        VerificationStatus = verificationStatus;
        Confidence = confidence;
    }

    public void Update(
        string category,
        string subcategory,
        string name,
        string description,
        string? pattern,
        string? examples,
        string? notes,
        string? source,
        VerificationStatus verificationStatus,
        double confidence)
    {
        Category = category ?? throw new ArgumentNullException(nameof(category));
        Subcategory = subcategory ?? throw new ArgumentNullException(nameof(subcategory));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Pattern = pattern;
        Examples = examples;
        Notes = notes;
        Source = source;
        VerificationStatus = verificationStatus;
        Confidence = confidence;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddEvidence(Evidence evidence)
    {
        if (evidence == null) throw new ArgumentNullException(nameof(evidence));
        _evidence.Add(evidence);
    }
}