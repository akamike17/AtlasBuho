namespace AtlasBuho.Domain.Entities;

public class LanguageFamily
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public string? NameEnglish { get; private set; }
    public string? Description { get; private set; }
    public string? Source { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private readonly List<LanguageGroup> _languageGroups = new();
    public IReadOnlyCollection<LanguageGroup> LanguageGroups => _languageGroups.AsReadOnly();

    private LanguageFamily() { }

    public LanguageFamily(string name, string? nameEnglish, string? description, string? source)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        NameEnglish = nameEnglish;
        Description = description;
        Source = source;
    }

    public void Update(string name, string? nameEnglish, string? description, string? source)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        NameEnglish = nameEnglish;
        Description = description;
        Source = source;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddLanguageGroup(LanguageGroup group)
    {
        if (group == null) throw new ArgumentNullException(nameof(group));
        if (!_languageGroups.Any(g => g.Id == group.Id))
        {
            _languageGroups.Add(group);
        }
    }
}