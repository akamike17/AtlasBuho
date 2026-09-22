namespace AtlasBuho.Domain.Entities;

public class SourceDocument
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Title { get; private set; } = string.Empty;
    public string Url { get; private set; } = string.Empty;
    public string HashSha256 { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long SizeBytes { get; private set; }
    public DateTime RetrievedAt { get; private set; }
    public int TotalPages { get; private set; }
    public string CatalogRange { get; private set; } = string.Empty;
    public string ParserVersion { get; private set; } = "1.0.0";
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private readonly List<SourcePage> _pages = new();
    public IReadOnlyCollection<SourcePage> Pages => _pages.AsReadOnly();

    private SourceDocument() { }

    public SourceDocument(
        string title,
        string url,
        string hashSha256,
        string contentType,
        long sizeBytes,
        DateTime retrievedAt,
        int totalPages,
        string catalogRange,
        string parserVersion)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Url = url ?? throw new ArgumentNullException(nameof(url));
        HashSha256 = hashSha256 ?? throw new ArgumentNullException(nameof(hashSha256));
        ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
        SizeBytes = sizeBytes;
        RetrievedAt = retrievedAt;
        TotalPages = totalPages;
        CatalogRange = catalogRange ?? string.Empty;
        ParserVersion = parserVersion ?? "1.0.0";
    }

    public void AddPage(SourcePage page)
    {
        if (page == null) throw new ArgumentNullException(nameof(page));
        if (!_pages.Any(p => p.PageNumber == page.PageNumber))
        {
            _pages.Add(page);
        }
    }

    public void Update(
        string title,
        string url,
        string hashSha256,
        string contentType,
        long sizeBytes,
        DateTime retrievedAt,
        int totalPages,
        string catalogRange,
        string parserVersion)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Url = url ?? throw new ArgumentNullException(nameof(url));
        HashSha256 = hashSha256 ?? throw new ArgumentNullException(nameof(hashSha256));
        ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
        SizeBytes = sizeBytes;
        RetrievedAt = retrievedAt;
        TotalPages = totalPages;
        CatalogRange = catalogRange ?? string.Empty;
        ParserVersion = parserVersion ?? "1.0.0";
        UpdatedAt = DateTime.UtcNow;
    }
}