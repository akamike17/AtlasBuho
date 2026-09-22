namespace AtlasBuho.Domain.Entities;

public enum VerificationStatus
{
    Unknown = 0,
    Verified = 1,
    Documented = 2,
    CommunityVerified = 3,
    AcademicVerified = 4,
    Provisional = 5,
    Uncertain = 6,
    Disputed = 7
}

public class Lexeme
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid LanguageVariantId { get; private set; }
    public string CanonicalForm { get; private set; } = string.Empty;
    public string? AlternativeForms { get; private set; }
    public string? Autodenomination { get; private set; }
    public string? SpanishMeaning { get; private set; }
    public string? PartOfSpeech { get; private set; }
    public string? PronunciationIpa { get; private set; }
    public string? PronunciationReadable { get; private set; }
    public string? SemanticDomain { get; private set; }
    public string? Register { get; private set; }
    public string? RegionalNotes { get; private set; }
    public string? Etymology { get; private set; }
    public string? Source { get; private set; }
    public VerificationStatus VerificationStatus { get; private set; } = VerificationStatus.Unknown;
    public double Confidence { get; private set; } = 0.0;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private readonly List<Pronunciation> _pronunciations = new();
    public IReadOnlyCollection<Pronunciation> Pronunciations => _pronunciations.AsReadOnly();

    private readonly List<Example> _examples = new();
    public IReadOnlyCollection<Example> Examples => _examples.AsReadOnly();

    private readonly List<Meaning> _meanings = new();
    public IReadOnlyCollection<Meaning> Meanings => _meanings.AsReadOnly();

    private readonly List<PhraseLexeme> _phraseLexemes = new();
    public IReadOnlyCollection<PhraseLexeme> PhraseLexemes => _phraseLexemes.AsReadOnly();

    private readonly List<Evidence> _evidence = new();
    public IReadOnlyCollection<Evidence> Evidence => _evidence.AsReadOnly();

    private readonly List<AudioRecording> _audioRecordings = new();
    public IReadOnlyCollection<AudioRecording> AudioRecordings => _audioRecordings.AsReadOnly();

    private Lexeme() { }

    public Lexeme(
        Guid languageVariantId,
        string canonicalForm,
        string? alternativeForms,
        string? autodenomination,
        string? spanishMeaning,
        string? partOfSpeech,
        string? pronunciationIpa,
        string? pronunciationReadable,
        string? semanticDomain,
        string? register,
        string? regionalNotes,
        string? etymology,
        string? source,
        VerificationStatus verificationStatus = VerificationStatus.Unknown,
        double confidence = 0.0)
    {
        LanguageVariantId = languageVariantId;
        CanonicalForm = canonicalForm ?? throw new ArgumentNullException(nameof(canonicalForm));
        AlternativeForms = alternativeForms;
        Autodenomination = autodenomination;
        SpanishMeaning = spanishMeaning;
        PartOfSpeech = partOfSpeech;
        PronunciationIpa = pronunciationIpa;
        PronunciationReadable = pronunciationReadable;
        SemanticDomain = semanticDomain;
        Register = register;
        RegionalNotes = regionalNotes;
        Etymology = etymology;
        Source = source;
        VerificationStatus = verificationStatus;
        Confidence = confidence;
    }

    public void Update(
        string canonicalForm,
        string? alternativeForms,
        string? autodenomination,
        string? spanishMeaning,
        string? partOfSpeech,
        string? pronunciationIpa,
        string? pronunciationReadable,
        string? semanticDomain,
        string? register,
        string? regionalNotes,
        string? etymology,
        string? source,
        VerificationStatus verificationStatus,
        double confidence)
    {
        CanonicalForm = canonicalForm ?? throw new ArgumentNullException(nameof(canonicalForm));
        AlternativeForms = alternativeForms;
        Autodenomination = autodenomination;
        SpanishMeaning = spanishMeaning;
        PartOfSpeech = partOfSpeech;
        PronunciationIpa = pronunciationIpa;
        PronunciationReadable = pronunciationReadable;
        SemanticDomain = semanticDomain;
        Register = register;
        RegionalNotes = regionalNotes;
        Etymology = etymology;
        Source = source;
        VerificationStatus = verificationStatus;
        Confidence = confidence;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddPronunciation(Pronunciation pronunciation)
    {
        if (pronunciation == null) throw new ArgumentNullException(nameof(pronunciation));
        if (!_pronunciations.Any(p => p.Id == pronunciation.Id))
        {
            _pronunciations.Add(pronunciation);
        }
    }

    public void AddExample(Example example)
    {
        if (example == null) throw new ArgumentNullException(nameof(example));
        if (!_examples.Any(e => e.Id == example.Id))
        {
            _examples.Add(example);
        }
    }

    public void AddMeaning(Meaning meaning)
    {
        if (meaning == null) throw new ArgumentNullException(nameof(meaning));
        if (!_meanings.Any(m => m.Id == meaning.Id))
        {
            _meanings.Add(meaning);
        }
    }

    public void AddPhraseLexeme(PhraseLexeme phraseLexeme)
    {
        if (phraseLexeme == null) throw new ArgumentNullException(nameof(phraseLexeme));
        if (!_phraseLexemes.Any(pl => pl.Id == phraseLexeme.Id))
        {
            _phraseLexemes.Add(phraseLexeme);
        }
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
}