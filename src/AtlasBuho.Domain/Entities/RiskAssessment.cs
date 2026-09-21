namespace AtlasBuho.Domain.Entities;

public enum RiskLevel
{
    Unknown = 0,
    P0_Emergency = 1,      // Emergencia documental
    P1_High = 2,           // Riesgo alto
    P2_Medium = 3,         // Riesgo medio
    P3_Preventive = 4      // Preservación preventiva
}

public class RiskAssessment
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid LanguageVariantId { get; private set; }
    public RiskLevel RiskLevel { get; private set; } = RiskLevel.Unknown;
    public string? Source { get; private set; } // INALI, UNESCO, INEGI, etc.
    public int? Year { get; private set; }
    public string? Methodology { get; private set; }
    public int? Population { get; private set; }
    public string? Criterion { get; private set; }
    public string? Notes { get; private set; }
    public bool IsCurrent { get; private set; } = true;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private RiskAssessment() { }

    public RiskAssessment(
        Guid languageVariantId,
        RiskLevel riskLevel,
        string? source,
        int? year,
        string? methodology,
        int? population,
        string? criterion,
        string? notes)
    {
        LanguageVariantId = languageVariantId;
        RiskLevel = riskLevel;
        Source = source;
        Year = year;
        Methodology = methodology;
        Population = population;
        Criterion = criterion;
        Notes = notes;
    }

    public void Update(
        RiskLevel riskLevel,
        string? source,
        int? year,
        string? methodology,
        int? population,
        string? criterion,
        string? notes,
        bool isCurrent)
    {
        RiskLevel = riskLevel;
        Source = source;
        Year = year;
        Methodology = methodology;
        Population = population;
        Criterion = criterion;
        Notes = notes;
        IsCurrent = isCurrent;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetNotCurrent()
    {
        IsCurrent = false;
        UpdatedAt = DateTime.UtcNow;
    }
}