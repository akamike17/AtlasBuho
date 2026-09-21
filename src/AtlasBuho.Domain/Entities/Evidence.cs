namespace AtlasBuho.Domain.Entities;

public class Evidence
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid SourceId { get; private set; }
    public Source? Source { get; private set; }
    public string? EntityType { get; private set; } // Lexeme, Meaning, Phrase, GrammarRule, etc.
    public Guid? EntityId { get; private set; }
    public string? Quote { get; private set; }
    public string? PageReference { get; private set; }
    public string? SectionReference { get; private set; }
    public string? Url { get; private set; }
    public string? Notes { get; private set; }
    public double Confidence { get; private set; } = 0.0;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private Evidence() { }

    public Evidence(
        Guid sourceId,
        string? entityType,
        Guid? entityId,
        string? quote,
        string? pageReference,
        string? sectionReference,
        string? url,
        string? notes,
        double confidence)
    {
        SourceId = sourceId;
        EntityType = entityType;
        EntityId = entityId;
        Quote = quote;
        PageReference = pageReference;
        SectionReference = sectionReference;
        Url = url;
        Notes = notes;
        Confidence = confidence;
    }

    public void Update(
        Guid sourceId,
        string? entityType,
        Guid? entityId,
        string? quote,
        string? pageReference,
        string? sectionReference,
        string? url,
        string? notes,
        double confidence)
    {
        SourceId = sourceId;
        EntityType = entityType;
        EntityId = entityId;
        Quote = quote;
        PageReference = pageReference;
        SectionReference = sectionReference;
        Url = url;
        Notes = notes;
        Confidence = confidence;
        UpdatedAt = DateTime.UtcNow;
    }
}