namespace AtlasBuho.Domain.Entities;

public class LanguageFamilySeed
{
    public string Name { get; set; } = string.Empty;
    public string? NameEnglish { get; set; }
    public string? Description { get; set; }
    public string? Region { get; set; }
    public string Source { get; set; } = "INALI";
    public List<LanguageGroupSeed> Groups { get; set; } = new();
}

public class LanguageGroupSeed
{
    public string Name { get; set; } = string.Empty;
    public string? NameEnglish { get; set; }
    public string? Description { get; set; }
    public string Source { get; set; } = "INALI";
    public List<LanguageVariantSeed> Variants { get; set; } = new();
}

public class LanguageVariantSeed
{
    public string Name { get; set; } = string.Empty;
    public string? Autodenomination { get; set; }
    public string? Iso639_3Code { get; set; }
    public string? InaliCode { get; set; }
    public string? Description { get; set; }
    public string? WritingSystem { get; set; }
    public string Source { get; set; } = "INALI";
    public string Region { get; set; } = string.Empty;
    public string? State { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public RiskLevel RiskLevel { get; set; } = RiskLevel.Unknown;
    public string? RiskSource { get; set; }
    public int? SpeakerCount { get; set; }
    public int? YearOfData { get; set; }
}