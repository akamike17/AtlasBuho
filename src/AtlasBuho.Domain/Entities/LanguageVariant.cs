namespace AtlasBuho.Domain.Entities;

public class LanguageVariant
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid LanguageGroupId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Autodenomination { get; private set; }
    public string? Iso639_3Code { get; private set; }
    public string? InaliCode { get; private set; }
    public string? Description { get; private set; }
    public string? WritingSystem { get; private set; }
    public string? Orthography { get; private set; }
    public string? Source { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private readonly List<Community> _communities = new();
    public IReadOnlyCollection<Community> Communities => _communities.AsReadOnly();

    private readonly List<Lexeme> _lexemes = new();
    public IReadOnlyCollection<Lexeme> Lexemes => _lexemes.AsReadOnly();

    private readonly List<SpeakerProfile> _speakers = new();
    public IReadOnlyCollection<SpeakerProfile> Speakers => _speakers.AsReadOnly();

    private readonly List<RiskAssessment> _riskAssessments = new();
    public IReadOnlyCollection<RiskAssessment> RiskAssessments => _riskAssessments.AsReadOnly();

    private readonly List<DialectRelationship> _dialectRelationships = new();
    public IReadOnlyCollection<DialectRelationship> DialectRelationships => _dialectRelationships.AsReadOnly();

    private readonly List<LanguageVariantAutodenomination> _autodenominations = new();
    public IReadOnlyCollection<LanguageVariantAutodenomination> Autodenominations => _autodenominations.AsReadOnly();

    private LanguageVariant() { }

    public LanguageVariant(
        Guid languageGroupId,
        string name,
        string? autodenomination,
        string? iso639_3Code,
        string? inaliCode,
        string? description,
        string? writingSystem,
        string? orthography,
        string? source)
    {
        LanguageGroupId = languageGroupId;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Autodenomination = autodenomination;
        Iso639_3Code = iso639_3Code;
        InaliCode = inaliCode;
        Description = description;
        WritingSystem = writingSystem;
        Orthography = orthography;
        Source = source;
    }

    public void Update(
        string name,
        string? autodenomination,
        string? iso639_3Code,
        string? inaliCode,
        string? description,
        string? writingSystem,
        string? orthography,
        string? source)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Autodenomination = autodenomination;
        Iso639_3Code = iso639_3Code;
        InaliCode = inaliCode;
        Description = description;
        WritingSystem = writingSystem;
        Orthography = orthography;
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

    public void AddLexeme(Lexeme lexeme)
    {
        if (lexeme == null) throw new ArgumentNullException(nameof(lexeme));
        if (!_lexemes.Any(l => l.Id == lexeme.Id))
        {
            _lexemes.Add(lexeme);
        }
    }

    public void AddSpeaker(SpeakerProfile speaker)
    {
        if (speaker == null) throw new ArgumentNullException(nameof(speaker));
        if (!_speakers.Any(s => s.Id == speaker.Id))
        {
            _speakers.Add(speaker);
        }
    }

    public void AddRiskAssessment(RiskAssessment assessment)
    {
        if (assessment == null) throw new ArgumentNullException(nameof(assessment));
        _riskAssessments.Add(assessment);
    }

    public void AddDialectRelationship(DialectRelationship relationship)
    {
        if (relationship == null) throw new ArgumentNullException(nameof(relationship));
        if (!_dialectRelationships.Any(d => d.Id == relationship.Id))
        {
            _dialectRelationships.Add(relationship);
        }
    }

    public void AddAutodenomination(LanguageVariantAutodenomination autodenomination)
    {
        if (autodenomination == null) throw new ArgumentNullException(nameof(autodenomination));
        if (!_autodenominations.Any(a => a.Id == autodenomination.Id))
        {
            _autodenominations.Add(autodenomination);
        }
    }
}