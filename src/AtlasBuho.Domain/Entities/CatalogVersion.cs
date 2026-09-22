namespace AtlasBuho.Domain.Entities;

public class CatalogVersion
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid CatalogSourceId { get; private set; }
    public string VersionNumber { get; private set; } = string.Empty;
    public string SourceDocumentHash { get; private set; } = string.Empty;
    public DateTime RetrievedAt { get; private set; }
    public DateTime ImportedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; private set; }
    public int FamilyCount { get; private set; }
    public int GroupCount { get; private set; }
    public int VariantCount { get; private set; }
    public int AutodenominationCount { get; private set; }
    public string ParserVersion { get; private set; } = "1.0.0";
    public string ImportStatus { get; private set; } = "Pending";
    public string? ErrorMessage { get; private set; }
    public string? ValidationErrors { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public CatalogSource? CatalogSource { get; private set; }
    private readonly List<CatalogRecord> _records = new();
    public IReadOnlyCollection<CatalogRecord> Records => _records.AsReadOnly();

    private CatalogVersion() { }

    public CatalogVersion(
        Guid catalogSourceId,
        string versionNumber,
        string sourceDocumentHash,
        DateTime retrievedAt,
        int familyCount,
        int groupCount,
        int variantCount,
        int autodenominationCount,
        string parserVersion,
        string importStatus = "Pending")
    {
        CatalogSourceId = catalogSourceId;
        VersionNumber = versionNumber ?? throw new ArgumentNullException(nameof(versionNumber));
        SourceDocumentHash = sourceDocumentHash ?? throw new ArgumentNullException(nameof(sourceDocumentHash));
        RetrievedAt = retrievedAt;
        FamilyCount = familyCount;
        GroupCount = groupCount;
        VariantCount = variantCount;
        AutodenominationCount = autodenominationCount;
        ParserVersion = parserVersion ?? "1.0.0";
        ImportStatus = importStatus ?? "Pending";
    }

    public void AddRecord(CatalogRecord record)
    {
        if (record == null) throw new ArgumentNullException(nameof(record));
        _records.Add(record);
    }

    public void UpdateStatus(string status, string? errorMessage = null)
    {
        ImportStatus = status ?? "Unknown";
        ErrorMessage = errorMessage;
        if (status == "Completed" || status == "Failed")
        {
            CompletedAt = DateTime.UtcNow;
        }
    }

    public void UpdateCounts(int familyCount, int groupCount, int variantCount, int autodenominationCount)
    {
        FamilyCount = familyCount;
        GroupCount = groupCount;
        VariantCount = variantCount;
        AutodenominationCount = autodenominationCount;
    }

    public void SetValidationErrors(string errors)
    {
        ValidationErrors = errors;
    }
}