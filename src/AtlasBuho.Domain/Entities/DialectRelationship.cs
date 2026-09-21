namespace AtlasBuho.Domain.Entities;

public enum DialectRelationshipType
{
    MutualIntelligibility = 1,
    PartialIntelligibility = 2,
    NoIntelligibility = 3,
    HistoricalRelation = 4,
    GeographicProximity = 5,
    SharedFeatures = 6
}

public class DialectRelationship
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid SourceLanguageVariantId { get; private set; }
    public Guid TargetLanguageVariantId { get; private set; }
    public DialectRelationshipType RelationshipType { get; private set; }
    public double? IntelligibilityScore { get; private set; } // 0.0 to 1.0
    public string? Description { get; private set; }
    public string? Source { get; private set; }
    public VerificationStatus VerificationStatus { get; private set; } = VerificationStatus.Unknown;
    public double Confidence { get; private set; } = 0.0;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private readonly List<Evidence> _evidence = new();
    public IReadOnlyCollection<Evidence> Evidence => _evidence.AsReadOnly();

    private DialectRelationship() { }

    public DialectRelationship(
        Guid sourceLanguageVariantId,
        Guid targetLanguageVariantId,
        DialectRelationshipType relationshipType,
        double? intelligibilityScore,
        string? description,
        string? source,
        VerificationStatus verificationStatus = VerificationStatus.Unknown,
        double confidence = 0.0)
    {
        SourceLanguageVariantId = sourceLanguageVariantId;
        TargetLanguageVariantId = targetLanguageVariantId;
        RelationshipType = relationshipType;
        IntelligibilityScore = intelligibilityScore;
        Description = description;
        Source = source;
        VerificationStatus = verificationStatus;
        Confidence = confidence;
    }

    public void Update(
        DialectRelationshipType relationshipType,
        double? intelligibilityScore,
        string? description,
        string? source,
        VerificationStatus verificationStatus,
        double confidence)
    {
        RelationshipType = relationshipType;
        IntelligibilityScore = intelligibilityScore;
        Description = description;
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