namespace AtlasBuho.Domain.Entities;

public class ImportQuarantine
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid CatalogVersionId { get; private set; }
    public Guid SourceDocumentId { get; private set; }
    public string EntityType { get; private set; } = string.Empty; // "LanguageFamily", "LanguageGroup", "LanguageVariant", "LanguageVariantAutodenomination"
    public string RawDataJson { get; private set; } = string.Empty; // Original source row as JSON
    public string? ComparisonKey { get; private set; } // Normalized key used for matching
    public int SourcePage { get; private set; }
    public string SourceSection { get; private set; } = string.Empty;
    public string Reason { get; private set; } = string.Empty; // Human-readable reason
    public string ResolutionMethod { get; private set; } = string.Empty; // How it was resolved or why not
    public QuarantineStatus Status { get; private set; } = QuarantineStatus.Open;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; private set; }
    public string? ResolvedBy { get; private set; }
    public string? ResolutionNotes { get; private set; }

    // Relationships
    public CatalogVersion? CatalogVersion { get; private set; }
    public SourceDocument? SourceDocument { get; private set; }

    private ImportQuarantine() { }

    public ImportQuarantine(
        Guid catalogVersionId,
        Guid sourceDocumentId,
        string entityType,
        string rawDataJson,
        string? comparisonKey,
        int sourcePage,
        string sourceSection,
        string reason,
        string resolutionMethod)
    {
        CatalogVersionId = catalogVersionId;
        SourceDocumentId = sourceDocumentId;
        EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
        RawDataJson = rawDataJson ?? throw new ArgumentNullException(nameof(rawDataJson));
        ComparisonKey = comparisonKey;
        SourcePage = sourcePage;
        SourceSection = sourceSection ?? string.Empty;
        Reason = reason ?? string.Empty;
        ResolutionMethod = resolutionMethod ?? string.Empty;
        Status = QuarantineStatus.Open;
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkResolved(string resolvedBy, string resolutionNotes)
    {
        Status = QuarantineStatus.Resolved;
        ResolvedAt = DateTime.UtcNow;
        ResolvedBy = resolvedBy;
        ResolutionNotes = resolutionNotes;
    }

    public void MarkAcceptedAsArtifact(string resolvedBy, string resolutionNotes)
    {
        Status = QuarantineStatus.AcceptedAsArtifact;
        ResolvedAt = DateTime.UtcNow;
        ResolvedBy = resolvedBy;
        ResolutionNotes = resolutionNotes;
    }

    public void MarkRejected(string resolvedBy, string resolutionNotes)
    {
        Status = QuarantineStatus.Rejected;
        ResolvedAt = DateTime.UtcNow;
        ResolvedBy = resolvedBy;
        ResolutionNotes = resolutionNotes;
    }
}

public enum QuarantineStatus
{
    Open = 0,
    Resolved = 1,
    AcceptedAsArtifact = 2,
    Rejected = 3
}