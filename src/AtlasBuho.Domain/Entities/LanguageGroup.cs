namespace AtlasBuho.Domain.Entities;

public class LanguageGroup
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid LanguageFamilyId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? NameEnglish { get; private set; }
    public string? Description { get; private set; }
    public string? Source { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private readonly List<LanguageVariant> _languageVariants = new();
    public IReadOnlyCollection<LanguageVariant> LanguageVariants => _languageVariants.AsReadOnly();

    private LanguageGroup() { }

    public LanguageGroup(Guid languageFamilyId, string name, string? nameEnglish, string? description, string? source)
    {
        LanguageFamilyId = languageFamilyId;
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

    public void AddLanguageVariant(LanguageVariant variant)
    {
        if (variant == null) throw new ArgumentNullException(nameof(variant));
        if (!_languageVariants.Any(v => v.Id == variant.Id))
        {
            _languageVariants.Add(variant);
        }
    }
}