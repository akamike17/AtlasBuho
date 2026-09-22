namespace AtlasBuho.Domain.Entities;

public class LanguageVariantAutodenomination
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid LanguageVariantId { get; private set; }
    public string Autodenomination { get; private set; } = string.Empty;
    public string? SpanishName { get; private set; }
    public string? Agrupacion { get; private set; }
    public string? Familia { get; private set; }
    public int SourcePage { get; private set; }
    public string SourceSection { get; private set; } = string.Empty;
    public Guid SourceDocumentId { get; private set; }
    public string SourceHash { get; private set; } = string.Empty;
    public DateTime ExtractedAt { get; private set; } = DateTime.UtcNow;
    public string ParserVersion { get; private set; } = "1.0.0";
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    public LanguageVariant? LanguageVariant { get; private set; }

    private LanguageVariantAutodenomination() { }

    public LanguageVariantAutodenomination(
        Guid languageVariantId,
        string autodenomination,
        string? spanishName,
        string? agrupacion,
        string? familia,
        int sourcePage,
        string sourceSection,
        Guid sourceDocumentId,
        string sourceHash,
        string parserVersion)
    {
        LanguageVariantId = languageVariantId;
        Autodenomination = autodenomination ?? throw new ArgumentNullException(nameof(autodenomination));
        SpanishName = spanishName;
        Agrupacion = agrupacion;
        Familia = familia;
        SourcePage = sourcePage;
        SourceSection = sourceSection ?? string.Empty;
        SourceDocumentId = sourceDocumentId;
        SourceHash = sourceHash ?? throw new ArgumentNullException(nameof(sourceHash));
        ParserVersion = parserVersion ?? "1.0.0";
    }

    public void Update(
        string autodenomination,
        string? spanishName,
        string? agrupacion,
        string? familia,
        int sourcePage,
        string sourceSection,
        Guid sourceDocumentId,
        string sourceHash,
        string parserVersion)
    {
        Autodenomination = autodenomination ?? throw new ArgumentNullException(nameof(autodenomination));
        SpanishName = spanishName;
        Agrupacion = agrupacion;
        Familia = familia;
        SourcePage = sourcePage;
        SourceSection = sourceSection ?? string.Empty;
        SourceDocumentId = sourceDocumentId;
        SourceHash = sourceHash ?? throw new ArgumentNullException(nameof(sourceHash));
        ParserVersion = parserVersion ?? "1.0.0";
        UpdatedAt = DateTime.UtcNow;
    }
}