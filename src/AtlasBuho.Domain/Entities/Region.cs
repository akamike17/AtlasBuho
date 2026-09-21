namespace AtlasBuho.Domain.Entities;

public class Region
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Country { get; private set; } = "México";
    public string? Source { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private readonly List<Community> _communities = new();
    public IReadOnlyCollection<Community> Communities => _communities.AsReadOnly();

    private Region() { }

    public Region(string name, string? description, string? country, string? source)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        Country = country;
        Source = source;
    }

    public void Update(string name, string? description, string? country, string? source)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        Country = country;
        Source = source;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddCommunity(Community community)
    {
        if (community == null) throw new ArgumentNullException(nameof(community));
        if (!_communities.Any(c => c.Id == community.Id))
        {
            _communities.Add(community);
        }
    }
}