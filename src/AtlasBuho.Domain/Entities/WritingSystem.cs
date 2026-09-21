namespace AtlasBuho.Domain.Entities;

public class WritingSystem
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid LanguageVariantId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Script { get; private set; } // Latin, Cyrillic, etc.
    public string? Description { get; private set; }
    public string? OrthographyRules { get; private set; }
    public string? Source { get; private set; }
    public bool IsOfficial { get; private set; } = false;
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private WritingSystem() { }

    public WritingSystem(
        Guid languageVariantId,
        string name,
        string? script,
        string? description,
        string? orthographyRules,
        string? source,
        bool isOfficial,
        bool isActive)
    {
        LanguageVariantId = languageVariantId;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Script = script;
        Description = description;
        OrthographyRules = orthographyRules;
        Source = source;
        IsOfficial = isOfficial;
        IsActive = isActive;
    }

    public void Update(
        string name,
        string? script,
        string? description,
        string? orthographyRules,
        string? source,
        bool isOfficial,
        bool isActive)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Script = script;
        Description = description;
        OrthographyRules = orthographyRules;
        Source = source;
        IsOfficial = isOfficial;
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }
}