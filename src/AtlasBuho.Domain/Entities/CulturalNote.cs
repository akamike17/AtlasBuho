namespace AtlasBuho.Domain.Entities;

public enum CulturalNoteType
{
    Linguistic = 1,
    Historical = 2,
    AcademicInterpretation = 3,
    CommunityTradition = 4,
    ArtisticMaterial = 5
}

public class CulturalNote
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid LanguageVariantId { get; private set; }
    public Guid? CommunityId { get; private set; }
    public CulturalNoteType Type { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public string? Source { get; private set; }
    public string? Author { get; private set; }
    public DateTime? DateRecorded { get; private set; }
    public VerificationStatus VerificationStatus { get; private set; } = VerificationStatus.Unknown;
    public double Confidence { get; private set; } = 0.0;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private readonly List<Evidence> _evidence = new();
    public IReadOnlyCollection<Evidence> Evidence => _evidence.AsReadOnly();

    private readonly List<AudioRecording> _audioRecordings = new();
    public IReadOnlyCollection<AudioRecording> AudioRecordings => _audioRecordings.AsReadOnly();

    private readonly List<VideoRecording> _videoRecordings = new();
    public IReadOnlyCollection<VideoRecording> VideoRecordings => _videoRecordings.AsReadOnly();

    private CulturalNote() { }

    public CulturalNote(
        Guid languageVariantId,
        CulturalNoteType type,
        string title,
        string content,
        string? source,
        string? author,
        DateTime? dateRecorded,
        VerificationStatus verificationStatus = VerificationStatus.Unknown,
        double confidence = 0.0,
        Guid? communityId = null)
    {
        LanguageVariantId = languageVariantId;
        Type = type;
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Content = content ?? throw new ArgumentNullException(nameof(content));
        Source = source;
        Author = author;
        DateRecorded = dateRecorded;
        VerificationStatus = verificationStatus;
        Confidence = confidence;
        CommunityId = communityId;
    }

    public void Update(
        CulturalNoteType type,
        string title,
        string content,
        string? source,
        string? author,
        DateTime? dateRecorded,
        VerificationStatus verificationStatus,
        double confidence,
        Guid? communityId = null)
    {
        Type = type;
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Content = content ?? throw new ArgumentNullException(nameof(content));
        Source = source;
        Author = author;
        DateRecorded = dateRecorded;
        VerificationStatus = verificationStatus;
        Confidence = confidence;
        CommunityId = communityId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddEvidence(Evidence evidence)
    {
        if (evidence == null) throw new ArgumentNullException(nameof(evidence));
        _evidence.Add(evidence);
    }

    public void AddAudioRecording(AudioRecording recording)
    {
        if (recording == null) throw new ArgumentNullException(nameof(recording));
        if (!_audioRecordings.Any(r => r.Id == recording.Id))
        {
            _audioRecordings.Add(recording);
        }
    }

    public void AddVideoRecording(VideoRecording recording)
    {
        if (recording == null) throw new ArgumentNullException(nameof(recording));
        if (!_videoRecordings.Any(r => r.Id == recording.Id))
        {
            _videoRecordings.Add(recording);
        }
    }
}