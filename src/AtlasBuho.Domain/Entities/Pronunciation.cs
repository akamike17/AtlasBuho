namespace AtlasBuho.Domain.Entities;

public class Pronunciation
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid LexemeId { get; private set; }
    public string? Ipa { get; private set; }
    public string? Readable { get; private set; }
    public string? AudioUrl { get; private set; }
    public Guid? AudioRecordingId { get; private set; }
    public Guid? SpeakerId { get; private set; }
    public string? Source { get; private set; }
    public VerificationStatus VerificationStatus { get; private set; } = VerificationStatus.Unknown;
    public double Confidence { get; private set; } = 0.0;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private Pronunciation() { }

    public Pronunciation(
        Guid lexemeId,
        string? ipa,
        string? readable,
        string? audioUrl,
        Guid? audioRecordingId,
        Guid? speakerId,
        string? source,
        VerificationStatus verificationStatus = VerificationStatus.Unknown,
        double confidence = 0.0)
    {
        LexemeId = lexemeId;
        Ipa = ipa;
        Readable = readable;
        AudioUrl = audioUrl;
        AudioRecordingId = audioRecordingId;
        SpeakerId = speakerId;
        Source = source;
        VerificationStatus = verificationStatus;
        Confidence = confidence;
    }

    public void Update(
        string? ipa,
        string? readable,
        string? audioUrl,
        Guid? audioRecordingId,
        Guid? speakerId,
        string? source,
        VerificationStatus verificationStatus,
        double confidence)
    {
        Ipa = ipa;
        Readable = readable;
        AudioUrl = audioUrl;
        AudioRecordingId = audioRecordingId;
        SpeakerId = speakerId;
        Source = source;
        VerificationStatus = verificationStatus;
        Confidence = confidence;
        UpdatedAt = DateTime.UtcNow;
    }
}