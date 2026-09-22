namespace AtlasBuho.Domain.Entities;

public class SourcePage
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid SourceDocumentId { get; private set; }
    public int PageNumber { get; private set; }
    public string? SectionReference { get; private set; }
    public string? RawTextHash { get; private set; }
    public DateTime ExtractedAt { get; private set; } = DateTime.UtcNow;

    public SourceDocument? SourceDocument { get; private set; }

    private SourcePage() { }

    public SourcePage(
        Guid sourceDocumentId,
        int pageNumber,
        string? sectionReference,
        string? rawTextHash)
    {
        SourceDocumentId = sourceDocumentId;
        PageNumber = pageNumber;
        SectionReference = sectionReference;
        RawTextHash = rawTextHash;
    }
}