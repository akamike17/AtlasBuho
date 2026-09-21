namespace AtlasBuho.Domain.Entities;

public class Community
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid LanguageVariantId { get; private set; }
    public Guid? RegionId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Autodenomination { get; private set; }
    public string? State { get; private set; }
    public string? Municipality { get; private set; }
    public string? Locality { get; private set; }
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }
    public int? Population { get; private set; }
    public int? SpeakerCount { get; private set; }
    public string? Source { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private Community() { }

    public Community(
        Guid languageVariantId,
        string name,
        string? autodenomination,
        string? state,
        string? municipality,
        string? locality,
        double? latitude,
        double? longitude,
        int? population,
        int? speakerCount,
        string? source,
        Guid? regionId = null)
    {
        LanguageVariantId = languageVariantId;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Autodenomination = autodenomination;
        State = state;
        Municipality = municipality;
        Locality = locality;
        Latitude = latitude;
        Longitude = longitude;
        Population = population;
        SpeakerCount = speakerCount;
        Source = source;
        RegionId = regionId;
    }

    public void Update(
        string name,
        string? autodenomination,
        string? state,
        string? municipality,
        string? locality,
        double? latitude,
        double? longitude,
        int? population,
        int? speakerCount,
        string? source,
        Guid? regionId = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Autodenomination = autodenomination;
        State = state;
        Municipality = municipality;
        Locality = locality;
        Latitude = latitude;
        Longitude = longitude;
        Population = population;
        SpeakerCount = speakerCount;
        Source = source;
        RegionId = regionId;
        UpdatedAt = DateTime.UtcNow;
    }
}