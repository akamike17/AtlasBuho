namespace AtlasBuho.Domain.Entities;

public class VideoRecording
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid? SpeakerId { get; private set; }
    public Guid LanguageVariantId { get; private set; }
    public Guid? CommunityId { get; private set; }
    public Guid? LexemeId { get; private set; }
    public Guid? PhraseId { get; private set; }
    public Guid? ExampleId { get; private set; }
    public Guid? CulturalNoteId { get; private set; }
    public string FilePath { get; private set; } = string.Empty;
    public string? FileName { get; private set; }
    public string? FileHash { get; private set; }
    public long? FileSizeBytes { get; private set; }
    public string? MimeType { get; private set; }
    public double? DurationSeconds { get; private set; }
    public int? Width { get; private set; }
    public int? Height { get; private set; }
    public string? FrameRate { get; private set; }
    public DateTime? RecordedAt { get; private set; }
    public string? Context { get; private set; }
    public string? Transcription { get; private set; }
    public string? Translation { get; private set; }
    public string? Annotation { get; private set; }
    public string? Source { get; private set; }
    public ConsentStatus ConsentStatus { get; private set; } = ConsentStatus.Unknown;
    public string? ConsentSource { get; private set; }
    public string? UsagePermission { get; private set; }
    public string? AttributionRequirement { get; private set; }
    public bool RemovalRequested { get; private set; } = false;
    public string? CommunityRestriction { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private VideoRecording() { }

    public VideoRecording(
        Guid languageVariantId,
        string filePath,
        string? fileName,
        string? fileHash,
        long? fileSizeBytes,
        string? mimeType,
        double? durationSeconds,
        int? width,
        int? height,
        string? frameRate,
        DateTime? recordedAt,
        string? context,
        string? transcription,
        string? translation,
        string? annotation,
        string? source,
        ConsentStatus consentStatus = ConsentStatus.Unknown,
        string? consentSource = null,
        string? usagePermission = null,
        string? attributionRequirement = null,
        string? communityRestriction = null,
        Guid? speakerId = null,
        Guid? communityId = null,
        Guid? lexemeId = null,
        Guid? phraseId = null,
        Guid? exampleId = null,
        Guid? culturalNoteId = null)
    {
        LanguageVariantId = languageVariantId;
        FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        FileName = fileName;
        FileHash = fileHash;
        FileSizeBytes = fileSizeBytes;
        MimeType = mimeType;
        DurationSeconds = durationSeconds;
        Width = width;
        Height = height;
        FrameRate = frameRate;
        RecordedAt = recordedAt;
        Context = context;
        Transcription = transcription;
        Translation = translation;
        Annotation = annotation;
        Source = source;
        ConsentStatus = consentStatus;
        ConsentSource = consentSource;
        UsagePermission = usagePermission;
        AttributionRequirement = attributionRequirement;
        CommunityRestriction = communityRestriction;
        SpeakerId = speakerId;
        CommunityId = communityId;
        LexemeId = lexemeId;
        PhraseId = phraseId;
        ExampleId = exampleId;
        CulturalNoteId = culturalNoteId;
    }

    public void Update(
        string filePath,
        string? fileName,
        string? fileHash,
        long? fileSizeBytes,
        string? mimeType,
        double? durationSeconds,
        int? width,
        int? height,
        string? frameRate,
        DateTime? recordedAt,
        string? context,
        string? transcription,
        string? translation,
        string? annotation,
        string? source,
        ConsentStatus consentStatus,
        string? consentSource,
        string? usagePermission,
        string? attributionRequirement,
        string? communityRestriction,
        Guid? speakerId = null,
        Guid? communityId = null,
        Guid? lexemeId = null,
        Guid? phraseId = null,
        Guid? exampleId = null,
        Guid? culturalNoteId = null)
    {
        FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        FileName = fileName;
        FileHash = fileHash;
        FileSizeBytes = fileSizeBytes;
        MimeType = mimeType;
        DurationSeconds = durationSeconds;
        Width = width;
        Height = height;
        FrameRate = frameRate;
        RecordedAt = recordedAt;
        Context = context;
        Transcription = transcription;
        Translation = translation;
        Annotation = annotation;
        Source = source;
        ConsentStatus = consentStatus;
        ConsentSource = consentSource;
        UsagePermission = usagePermission;
        AttributionRequirement = attributionRequirement;
        CommunityRestriction = communityRestriction;
        SpeakerId = speakerId;
        CommunityId = communityId;
        LexemeId = lexemeId;
        PhraseId = phraseId;
        ExampleId = exampleId;
        CulturalNoteId = culturalNoteId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RequestRemoval()
    {
        RemovalRequested = true;
        UpdatedAt = DateTime.UtcNow;
    }
}