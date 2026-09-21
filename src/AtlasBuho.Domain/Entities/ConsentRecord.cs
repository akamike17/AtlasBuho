namespace AtlasBuho.Domain.Entities;

public class ConsentRecord
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid SpeakerId { get; private set; }
    public Guid? AudioRecordingId { get; private set; }
    public Guid? VideoRecordingId { get; private set; }
    public Guid? LexemeId { get; private set; }
    public Guid? PhraseId { get; private set; }
    public ConsentStatus Status { get; private set; } = ConsentStatus.Unknown;
    public string? GrantedBy { get; private set; }
    public DateTime? GrantedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public string? Scope { get; private set; } // What the consent covers
    public string? Restrictions { get; private set; }
    public string? Notes { get; private set; }
    public bool IsRevoked { get; private set; } = false;
    public DateTime? RevokedAt { get; private set; }
    public string? RevokedBy { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private ConsentRecord() { }

    public ConsentRecord(
        Guid speakerId,
        ConsentStatus status,
        string? grantedBy,
        DateTime? grantedAt,
        DateTime? expiresAt,
        string? scope,
        string? restrictions,
        string? notes,
        Guid? audioRecordingId = null,
        Guid? videoRecordingId = null,
        Guid? lexemeId = null,
        Guid? phraseId = null)
    {
        SpeakerId = speakerId;
        Status = status;
        GrantedBy = grantedBy;
        GrantedAt = grantedAt;
        ExpiresAt = expiresAt;
        Scope = scope;
        Restrictions = restrictions;
        Notes = notes;
        AudioRecordingId = audioRecordingId;
        VideoRecordingId = videoRecordingId;
        LexemeId = lexemeId;
        PhraseId = phraseId;
    }

    public void Update(
        ConsentStatus status,
        string? grantedBy,
        DateTime? grantedAt,
        DateTime? expiresAt,
        string? scope,
        string? restrictions,
        string? notes)
    {
        Status = status;
        GrantedBy = grantedBy;
        GrantedAt = grantedAt;
        ExpiresAt = expiresAt;
        Scope = scope;
        Restrictions = restrictions;
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Revoke(string? revokedBy, string? notes)
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        RevokedBy = revokedBy;
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }
}