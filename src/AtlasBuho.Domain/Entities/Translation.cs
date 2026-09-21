namespace AtlasBuho.Domain.Entities;

public class Translation
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid SourceLanguageVariantId { get; private set; }
    public Guid TargetLanguageVariantId { get; private set; }
    public string SourceText { get; private set; } = string.Empty;
    public string TargetText { get; private set; } = string.Empty;
    public string? Context { get; private set; }
    public string? Source { get; private set; }
    public double Confidence { get; private set; } = 0.0;
    public VerificationStatus VerificationStatus { get; private set; } = VerificationStatus.Unknown;
    public string? ModelUsed { get; private set; }
    public string? ModelVersion { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private readonly List<Evidence> _evidence = new();
    public IReadOnlyCollection<Evidence> Evidence => _evidence.AsReadOnly();

    private Translation() { }

    public Translation(
        Guid sourceLanguageVariantId,
        Guid targetLanguageVariantId,
        string sourceText,
        string targetText,
        string? context,
        string? source,
        double confidence,
        VerificationStatus verificationStatus,
        string? modelUsed,
        string? modelVersion)
    {
        SourceLanguageVariantId = sourceLanguageVariantId;
        TargetLanguageVariantId = targetLanguageVariantId;
        SourceText = sourceText ?? throw new ArgumentNullException(nameof(sourceText));
        TargetText = targetText ?? throw new ArgumentNullException(nameof(targetText));
        Context = context;
        Source = source;
        Confidence = confidence;
        VerificationStatus = verificationStatus;
        ModelUsed = modelUsed;
        ModelVersion = modelVersion;
    }

    public void Update(
        string sourceText,
        string targetText,
        string? context,
        string? source,
        double confidence,
        VerificationStatus verificationStatus,
        string? modelUsed,
        string? modelVersion)
    {
        SourceText = sourceText ?? throw new ArgumentNullException(nameof(sourceText));
        TargetText = targetText ?? throw new ArgumentNullException(nameof(targetText));
        Context = context;
        Source = source;
        Confidence = confidence;
        VerificationStatus = verificationStatus;
        ModelUsed = modelUsed;
        ModelVersion = modelVersion;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddEvidence(Evidence evidence)
    {
        if (evidence == null) throw new ArgumentNullException(nameof(evidence));
        _evidence.Add(evidence);
    }
}