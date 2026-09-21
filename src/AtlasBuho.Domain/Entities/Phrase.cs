namespace AtlasBuho.Domain.Entities;

public class Phrase
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid LanguageVariantId { get; private set; }
    public string Text { get; private set; } = string.Empty;
    public string? SpanishTranslation { get; private set; }
    public string? EnglishTranslation { get; private set; }
    public string? Context { get; private set; }
    public string? GrammarNotes { get; private set; }
    public string? Source { get; private set; }
    public VerificationStatus VerificationStatus { get; private set; } = VerificationStatus.Unknown;
    public double Confidence { get; private set; } = 0.0;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private readonly List<PhraseLexeme> _phraseLexemes = new();
    public IReadOnlyCollection<PhraseLexeme> PhraseLexemes => _phraseLexemes.AsReadOnly();

    private readonly List<AudioRecording> _audioRecordings = new();
    public IReadOnlyCollection<AudioRecording> AudioRecordings => _audioRecordings.AsReadOnly();

    private readonly List<Evidence> _evidence = new();
    public IReadOnlyCollection<Evidence> Evidence => _evidence.AsReadOnly();

    private Phrase() { }

    public Phrase(
        Guid languageVariantId,
        string text,
        string? spanishTranslation,
        string? englishTranslation,
        string? context,
        string? grammarNotes,
        string? source,
        VerificationStatus verificationStatus = VerificationStatus.Unknown,
        double confidence = 0.0)
    {
        LanguageVariantId = languageVariantId;
        Text = text ?? throw new ArgumentNullException(nameof(text));
        SpanishTranslation = spanishTranslation;
        EnglishTranslation = englishTranslation;
        Context = context;
        GrammarNotes = grammarNotes;
        Source = source;
        VerificationStatus = verificationStatus;
        Confidence = confidence;
    }

    public void Update(
        string text,
        string? spanishTranslation,
        string? englishTranslation,
        string? context,
        string? grammarNotes,
        string? source,
        VerificationStatus verificationStatus,
        double confidence)
    {
        Text = text ?? throw new ArgumentNullException(nameof(text));
        SpanishTranslation = spanishTranslation;
        EnglishTranslation = englishTranslation;
        Context = context;
        GrammarNotes = grammarNotes;
        Source = source;
        VerificationStatus = verificationStatus;
        Confidence = confidence;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddPhraseLexeme(PhraseLexeme phraseLexeme)
    {
        if (phraseLexeme == null) throw new ArgumentNullException(nameof(phraseLexeme));
        if (!_phraseLexemes.Any(pl => pl.Id == phraseLexeme.Id))
        {
            _phraseLexemes.Add(phraseLexeme);
        }
    }

    public void AddAudioRecording(AudioRecording recording)
    {
        if (recording == null) throw new ArgumentNullException(nameof(recording));
        if (!_audioRecordings.Any(r => r.Id == recording.Id))
        {
            _audioRecordings.Add(recording);
        }
    }

    public void AddEvidence(Evidence evidence)
    {
        if (evidence == null) throw new ArgumentNullException(nameof(evidence));
        _evidence.Add(evidence);
    }
}