namespace AtlasBuho.Domain.Entities;

public enum SourceLevel
{
    Institutional = 1,      // Nivel A: INALI, ALIN, INEGI, INPI, UNAM, INAH, universities
    LinguisticArchive = 2,  // Nivel B: ALIN, AILLA, university archives, linguistic documentation repos
    AcademicMaterial = 3,   // Nivel C: dictionaries, grammars, theses, corpora, lexical lists
    CommunityMaterial = 4,  // Nivel D: speakers, writers, teachers, authorities, narrators
    InternetContent = 5     // Nivel E: audiovisual content, web - discovery only, not truth
}

public class Source
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public SourceLevel Level { get; private set; }
    public string? Institution { get; private set; }
    public string? Url { get; private set; }
    public string? Doi { get; private set; }
    public string? Isbn { get; private set; }
    public string? Issn { get; private set; }
    public DateTime? PublicationDate { get; private set; }
    public string? Authors { get; private set; }
    public string? Editors { get; private set; }
    public string? Publisher { get; private set; }
    public string? Location { get; private set; }
    public string? Language { get; private set; }
    public string? Notes { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private Source() { }

    public Source(
        string name,
        SourceLevel level,
        string? description,
        string? institution,
        string? url,
        string? doi,
        string? isbn,
        string? issn,
        DateTime? publicationDate,
        string? authors,
        string? editors,
        string? publisher,
        string? location,
        string? language,
        string? notes)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Level = level;
        Description = description;
        Institution = institution;
        Url = url;
        Doi = doi;
        Isbn = isbn;
        Issn = issn;
        PublicationDate = publicationDate;
        Authors = authors;
        Editors = editors;
        Publisher = publisher;
        Location = location;
        Language = language;
        Notes = notes;
    }

    public void Update(
        string name,
        SourceLevel level,
        string? description,
        string? institution,
        string? url,
        string? doi,
        string? isbn,
        string? issn,
        DateTime? publicationDate,
        string? authors,
        string? editors,
        string? publisher,
        string? location,
        string? language,
        string? notes,
        bool isActive)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Level = level;
        Description = description;
        Institution = institution;
        Url = url;
        Doi = doi;
        Isbn = isbn;
        Issn = issn;
        PublicationDate = publicationDate;
        Authors = authors;
        Editors = editors;
        Publisher = publisher;
        Location = location;
        Language = language;
        Notes = notes;
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }
}