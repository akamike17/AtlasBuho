namespace AtlasBuho.Domain.Entities;

public class CatalogRecord
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid CatalogVersionId { get; private set; }
    public CatalogRecordStatus Status { get; private set; } = CatalogRecordStatus.Current;
    
    // Identity fields - what makes this record unique
    public string EntityType { get; private set; } = string.Empty; // "LanguageFamily", "LanguageGroup", "LanguageVariant", "Autodenomination"
    public string IdentityKey { get; private set; } = string.Empty; // Composite key: e.g., "Family:Maya|Group:Q'anjob'al|Variant:<Q'anjob'al>"
    public string InaliCode { get; private set; } = string.Empty;
    public string Iso639_3Code { get; private set; } = string.Empty;
    
    // Data fields
    public string Name { get; private set; } = string.Empty;
    public string? Autodenomination { get; private set; }
    public string? Description { get; private set; }
    public string? GeoReference { get; private set; }
    
    // Provenance fields
    public Guid SourceDocumentId { get; private set; }
    public string SourceHash { get; private set; } = string.Empty;
    public string SourceUrl { get; private set; } = string.Empty;
    public int SourcePage { get; private set; }
    public string SourceSection { get; private set; } = string.Empty;
    public DateTime ExtractionDate { get; private set; }
    public string ParserVersion { get; private set; } = "1.0.0";
    
    // Relationships
    public Guid? ParentRecordId { get; private set; } // For hierarchical: Family -> Group -> Variant -> Autodenomination
    public CatalogVersion? CatalogVersion { get; private set; }
    public CatalogRecord? ParentRecord { get; private set; }
    
    private CatalogRecord() { }

    public CatalogRecord(
        Guid catalogVersionId,
        string entityType,
        string identityKey,
        string inaliCode,
        string iso639_3Code,
        string name,
        string? autodenomination,
        string? description,
        string? geoReference,
        Guid sourceDocumentId,
        string sourceHash,
        string sourceUrl,
        int sourcePage,
        string sourceSection,
        DateTime extractionDate,
        string parserVersion,
        Guid? parentRecordId = null)
    {
        CatalogVersionId = catalogVersionId;
        EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
        IdentityKey = identityKey ?? throw new ArgumentNullException(nameof(identityKey));
        InaliCode = inaliCode ?? string.Empty;
        Iso639_3Code = iso639_3Code ?? string.Empty;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Autodenomination = autodenomination;
        Description = description;
        GeoReference = geoReference;
        SourceDocumentId = sourceDocumentId;
        SourceHash = sourceHash ?? throw new ArgumentNullException(nameof(sourceHash));
        SourceUrl = sourceUrl ?? throw new ArgumentNullException(nameof(sourceUrl));
        SourcePage = sourcePage;
        SourceSection = sourceSection ?? string.Empty;
        ExtractionDate = extractionDate;
        ParserVersion = parserVersion ?? "1.0.0";
        ParentRecordId = parentRecordId;
    }

    public void UpdateStatus(CatalogRecordStatus status)
    {
        Status = status;
    }
}