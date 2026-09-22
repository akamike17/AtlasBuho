namespace AtlasBuho.Domain.Entities;

public enum CatalogRecordStatus
{
    Current = 1,
    Historical = 2,
    Quarantined = 3
}

public class CatalogSource
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string BaseUrl { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private readonly List<CatalogVersion> _versions = new();
    public IReadOnlyCollection<CatalogVersion> Versions => _versions.AsReadOnly();

    private CatalogSource() { }

    public CatalogSource(string name, string description, string baseUrl)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? string.Empty;
        BaseUrl = baseUrl ?? string.Empty;
    }

    public void AddVersion(CatalogVersion version)
    {
        if (version == null) throw new ArgumentNullException(nameof(version));
        _versions.Add(version);
    }

    public void Update(string name, string description, string baseUrl, bool isActive)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? string.Empty;
        BaseUrl = baseUrl ?? string.Empty;
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }
}