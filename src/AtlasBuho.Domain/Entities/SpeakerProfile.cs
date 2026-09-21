namespace AtlasBuho.Domain.Entities;

public class SpeakerProfile
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid LanguageVariantId { get; private set; }
    public Guid? CommunityId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Pseudonym { get; private set; }
    public DateTime? BirthDate { get; private set; }
    public string? Gender { get; private set; }
    public string? EducationLevel { get; private set; }
    public string? Occupation { get; private set; }
    public string? Source { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private readonly List<AudioRecording> _recordings = new();
    public IReadOnlyCollection<AudioRecording> Recordings => _recordings.AsReadOnly();

    private readonly List<ConsentRecord> _consents = new();
    public IReadOnlyCollection<ConsentRecord> Consents => _consents.AsReadOnly();

    private SpeakerProfile() { }

    public SpeakerProfile(
        Guid languageVariantId,
        string name,
        string? pseudonym,
        DateTime? birthDate,
        string? gender,
        string? educationLevel,
        string? occupation,
        string? source,
        Guid? communityId = null)
    {
        LanguageVariantId = languageVariantId;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Pseudonym = pseudonym;
        BirthDate = birthDate;
        Gender = gender;
        EducationLevel = educationLevel;
        Occupation = occupation;
        Source = source;
        CommunityId = communityId;
    }

    public void Update(
        string name,
        string? pseudonym,
        DateTime? birthDate,
        string? gender,
        string? educationLevel,
        string? occupation,
        string? source,
        Guid? communityId = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Pseudonym = pseudonym;
        BirthDate = birthDate;
        Gender = gender;
        EducationLevel = educationLevel;
        Occupation = occupation;
        Source = source;
        CommunityId = communityId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddRecording(AudioRecording recording)
    {
        if (recording == null) throw new ArgumentNullException(nameof(recording));
        if (!_recordings.Any(r => r.Id == recording.Id))
        {
            _recordings.Add(recording);
        }
    }

    public void AddConsent(ConsentRecord consent)
    {
        if (consent == null) throw new ArgumentNullException(nameof(consent));
        _consents.Add(consent);
    }
}