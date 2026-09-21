namespace AtlasBuho.Domain.Entities;

/// <summary>
/// 5W1H Framework for Linguistic Documentation
/// WHO: Speaker / Community / Researcher
/// WHAT: Lexeme / Phrase / GrammarRule / CulturalNote
/// WHERE: Region / Community / Coordinates
/// WHEN: DateTime / Period / Era
/// WHY: DocumentationPurpose / ResearchGoal / PreservationAction
/// HOW: Methodology / Technique / Protocol
/// </summary>
public class W1HContext
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    
    // WHO - Person/Entity involved
    public Guid? SpeakerId { get; private set; }
    public Guid? CommunityId { get; private set; }
    public Guid? ResearcherId { get; private set; }
    public string? Role { get; private set; } // Speaker, Elder, Researcher, CommunityLeader, Witness
    
    // WHAT - Linguistic entity documented
    public Guid? LexemeId { get; private set; }
    public Guid? PhraseId { get; private set; }
    public Guid? GrammarRuleId { get; private set; }
    public Guid? CulturalNoteId { get; private set; }
    public string? EntityType { get; private set; } // Lexeme, Phrase, Grammar, CulturalNote, Recording
    
    // WHERE - Location context
    public Guid? RegionId { get; private set; }
    public Guid? CommunityLocationId { get; private set; }
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }
    public string? LocationDescription { get; private set; }
    public string? LocationType { get; private set; } // Home, CommunityCenter, Field, School, CeremonialSite
    
    // WHEN - Temporal context
    public DateTime? DocumentedAt { get; private set; }
    public DateTime? PeriodStart { get; private set; }
    public DateTime? PeriodEnd { get; private set; }
    public string? Era { get; private set; } // PreColonial, Colonial, Modern, Contemporary
    public string? Season { get; private set; }
    public string? TimeOfDay { get; private set; }
    
    // WHY - Purpose/Reason
    public DocumentationPurpose Purpose { get; private set; }
    public string? ResearchGoal { get; private set; }
    public string? PreservationAction { get; private set; }
    public string? CommunityRequest { get; private set; }
    
    // HOW - Methodology
    public DocumentationMethodology Methodology { get; private set; }
    public string? Technique { get; private set; } // Elicitation, Observation, Interview, Recording, Experiment
    public string? Protocol { get; private set; } // INALI, UNESCO, CommunityProtocol, Academic
    public string? Equipment { get; private set; }
    public string? Software { get; private set; }
    public Guid? AudioRecordingId { get; private set; }
    public Guid? VideoRecordingId { get; private set; }
    public Guid? SourceId { get; private set; }
    
    // Metadata
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;
    public bool IsComplete { get; private set; }
    
    private W1HContext() { }
    
    public static W1HContext Create(
        Guid? speakerId = null,
        Guid? communityId = null,
        Guid? researcherId = null,
        string? role = null,
        Guid? lexemeId = null,
        Guid? phraseId = null,
        Guid? grammarRuleId = null,
        Guid? culturalNoteId = null,
        string? entityType = null,
        Guid? regionId = null,
        Guid? communityLocationId = null,
        double? latitude = null,
        double? longitude = null,
        string? locationDescription = null,
        string? locationType = null,
        DateTime? documentedAt = null,
        DateTime? periodStart = null,
        DateTime? periodEnd = null,
        string? era = null,
        string? season = null,
        string? timeOfDay = null,
        DocumentationPurpose? purpose = null,
        string? researchGoal = null,
        string? preservationAction = null,
        string? communityRequest = null,
        DocumentationMethodology? methodology = null,
        string? technique = null,
        string? protocol = null,
        string? equipment = null,
        string? software = null,
        Guid? audioRecordingId = null,
        Guid? videoRecordingId = null,
        Guid? sourceId = null,
        string? notes = null)
    {
        var context = new W1HContext
        {
            SpeakerId = speakerId,
            CommunityId = communityId,
            ResearcherId = researcherId,
            Role = role,
            LexemeId = lexemeId,
            PhraseId = phraseId,
            GrammarRuleId = grammarRuleId,
            CulturalNoteId = culturalNoteId,
            EntityType = entityType,
            RegionId = regionId,
            CommunityLocationId = communityLocationId,
            Latitude = latitude,
            Longitude = longitude,
            LocationDescription = locationDescription,
            LocationType = locationType,
            DocumentedAt = documentedAt ?? DateTime.UtcNow,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            Era = era,
            Season = season,
            TimeOfDay = timeOfDay,
            Purpose = purpose ?? DocumentationPurpose.Documentation,
            ResearchGoal = researchGoal,
            PreservationAction = preservationAction,
            CommunityRequest = communityRequest,
            Methodology = methodology ?? DocumentationMethodology.DirectElicitation,
            Technique = technique,
            Protocol = protocol,
            Equipment = equipment,
            Software = software,
            AudioRecordingId = audioRecordingId,
            VideoRecordingId = videoRecordingId,
            SourceId = sourceId,
            Notes = notes,
            IsComplete = ValidateCompleteness(
                speakerId, communityId, researcherId, role,
                lexemeId, phraseId, grammarRuleId, culturalNoteId, entityType,
                regionId, communityLocationId, latitude, longitude, locationDescription,
                documentedAt, purpose, methodology, technique)
        };
        
        return context;
    }
    
    public void Update(
        string? role = null,
        string? entityType = null,
        string? locationDescription = null,
        string? locationType = null,
        DateTime? documentedAt = null,
        DateTime? periodStart = null,
        DateTime? periodEnd = null,
        string? era = null,
        string? season = null,
        string? timeOfDay = null,
        DocumentationPurpose? purpose = null,
        string? researchGoal = null,
        string? preservationAction = null,
        string? communityRequest = null,
        DocumentationMethodology? methodology = null,
        string? technique = null,
        string? protocol = null,
        string? equipment = null,
        string? software = null,
        string? notes = null)
    {
        if (role != null) Role = role;
        if (entityType != null) EntityType = entityType;
        if (locationDescription != null) LocationDescription = locationDescription;
        if (locationType != null) LocationType = locationType;
        if (documentedAt != null) DocumentedAt = documentedAt.Value;
        if (periodStart != null) PeriodStart = periodStart;
        if (periodEnd != null) PeriodEnd = periodEnd;
        if (era != null) Era = era;
        if (season != null) Season = season;
        if (timeOfDay != null) TimeOfDay = timeOfDay;
        if (purpose != null) Purpose = purpose.Value;
        if (researchGoal != null) ResearchGoal = researchGoal;
        if (preservationAction != null) PreservationAction = preservationAction;
        if (communityRequest != null) CommunityRequest = communityRequest;
        if (methodology != null) Methodology = methodology.Value;
        if (technique != null) Technique = technique;
        if (protocol != null) Protocol = protocol;
        if (equipment != null) Equipment = equipment;
        if (software != null) Software = software;
        if (notes != null) Notes = notes;
        
        UpdatedAt = DateTime.UtcNow;
        IsComplete = ValidateCompleteness(
            SpeakerId, CommunityId, ResearcherId, Role,
            LexemeId, PhraseId, GrammarRuleId, CulturalNoteId, EntityType,
            RegionId, CommunityLocationId, Latitude, Longitude, LocationDescription,
            DocumentedAt, Purpose, Methodology, Technique);
    }
    
    public void LinkEntity(Guid entityId, string entityType)
    {
        switch (entityType.ToLowerInvariant())
        {
            case "lexeme": LexemeId = entityId; break;
            case "phrase": PhraseId = entityId; break;
            case "grammarrule": GrammarRuleId = entityId; break;
            case "culturalnote": CulturalNoteId = entityId; break;
            case "recording": 
                if (entityId.ToString().StartsWith("audio")) AudioRecordingId = entityId;
                else VideoRecordingId = entityId;
                break;
        }
        EntityType = entityType;
        UpdatedAt = DateTime.UtcNow;
        IsComplete = ValidateCompleteness(
            SpeakerId, CommunityId, ResearcherId, Role,
            LexemeId, PhraseId, GrammarRuleId, CulturalNoteId, EntityType,
            RegionId, CommunityLocationId, Latitude, Longitude, LocationDescription,
            DocumentedAt, Purpose, Methodology, Technique);
    }
    
    public void LinkLocation(Guid? regionId = null, Guid? communityId = null, double? lat = null, double? lon = null, string? description = null, string? type = null)
    {
        if (regionId != null) RegionId = regionId;
        if (communityId != null) CommunityLocationId = communityId;
        if (lat != null) Latitude = lat;
        if (lon != null) Longitude = lon;
        if (description != null) LocationDescription = description;
        if (type != null) LocationType = type;
        UpdatedAt = DateTime.UtcNow;
        IsComplete = ValidateCompleteness(
            SpeakerId, CommunityId, ResearcherId, Role,
            LexemeId, PhraseId, GrammarRuleId, CulturalNoteId, EntityType,
            RegionId, CommunityLocationId, Latitude, Longitude, LocationDescription,
            DocumentedAt, Purpose, Methodology, Technique);
    }
    
    public void LinkTemporal(DateTime? documentedAt = null, DateTime? start = null, DateTime? end = null, string? era = null, string? season = null, string? timeOfDay = null)
    {
        if (documentedAt != null) DocumentedAt = documentedAt;
        if (start != null) PeriodStart = start;
        if (end != null) PeriodEnd = end;
        if (era != null) Era = era;
        if (season != null) Season = season;
        if (timeOfDay != null) TimeOfDay = timeOfDay;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void LinkPurpose(DocumentationPurpose? purpose = null, string? goal = null, string? action = null, string? request = null)
    {
        if (purpose != null) Purpose = purpose.Value;
        if (goal != null) ResearchGoal = goal;
        if (action != null) PreservationAction = action;
        if (request != null) CommunityRequest = request;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void LinkMethodology(DocumentationMethodology? methodology = null, string? technique = null, string? protocol = null, string? equipment = null, string? software = null, Guid? audioId = null, Guid? videoId = null, Guid? sourceId = null)
    {
        if (methodology != null) Methodology = methodology.Value;
        if (technique != null) Technique = technique;
        if (protocol != null) Protocol = protocol;
        if (equipment != null) Equipment = equipment;
        if (software != null) Software = software;
        if (audioId != null) AudioRecordingId = audioId;
        if (videoId != null) VideoRecordingId = videoId;
        if (sourceId != null) SourceId = sourceId;
        UpdatedAt = DateTime.UtcNow;
        IsComplete = ValidateCompleteness(
            SpeakerId, CommunityId, ResearcherId, Role,
            LexemeId, PhraseId, GrammarRuleId, CulturalNoteId, EntityType,
            RegionId, CommunityLocationId, Latitude, Longitude, LocationDescription,
            DocumentedAt, Purpose, Methodology, Technique);
    }
    
    private static bool ValidateCompleteness(
        Guid? speakerId, Guid? communityId, Guid? researcherId, string? role,
        Guid? lexemeId, Guid? phraseId, Guid? grammarRuleId, Guid? culturalNoteId, string? entityType,
        Guid? regionId, Guid? communityLocationId, double? lat, double? lon, string? locationDesc,
        DateTime? documentedAt, DocumentationPurpose? purpose, DocumentationMethodology? methodology, string? technique)
    {
        // Minimum viable context: WHO + WHAT + WHERE + WHEN + WHY + HOW
        bool hasWho = speakerId.HasValue || communityId.HasValue || researcherId.HasValue;
        bool hasWhat = lexemeId.HasValue || phraseId.HasValue || grammarRuleId.HasValue || culturalNoteId.HasValue || !string.IsNullOrEmpty(entityType);
        bool hasWhere = regionId.HasValue || communityLocationId.HasValue || lat.HasValue || lon.HasValue || !string.IsNullOrEmpty(locationDesc);
        bool hasWhen = documentedAt.HasValue;
        bool hasWhy = purpose.HasValue;
        bool hasHow = methodology.HasValue || !string.IsNullOrEmpty(technique);
        
        return hasWho && hasWhat && hasWhere && hasWhen && hasWhy && hasHow;
    }
    
    public W1HCompletenessReport GetCompletenessReport()
    {
        return new W1HCompletenessReport
        {
            HasWho = SpeakerId.HasValue || CommunityId.HasValue || ResearcherId.HasValue,
            WhoDetails = new List<string> { 
                SpeakerId.HasValue ? "Speaker" : null, 
                CommunityId.HasValue ? "Community" : null, 
                ResearcherId.HasValue ? "Researcher" : null 
            }.Where(x => x != null).Select(x => x!).ToList(),
            
            HasWhat = LexemeId.HasValue || PhraseId.HasValue || GrammarRuleId.HasValue || CulturalNoteId.HasValue || !string.IsNullOrEmpty(EntityType),
            WhatDetails = new List<string> { 
                LexemeId.HasValue ? "Lexeme" : null, 
                PhraseId.HasValue ? "Phrase" : null, 
                GrammarRuleId.HasValue ? "GrammarRule" : null, 
                CulturalNoteId.HasValue ? "CulturalNote" : null,
                !string.IsNullOrEmpty(EntityType) ? EntityType : null
            }.Where(x => x != null).Select(x => x!).ToList(),
            
            HasWhere = RegionId.HasValue || CommunityLocationId.HasValue || Latitude.HasValue || Longitude.HasValue || !string.IsNullOrEmpty(LocationDescription),
            WhereDetails = new List<string> { 
                RegionId.HasValue ? "Region" : null, 
                CommunityLocationId.HasValue ? "Community" : null,
                Latitude.HasValue ? $"Lat:{Latitude}" : null,
                Longitude.HasValue ? $"Lon:{Longitude}" : null
            }.Where(x => x != null).Select(x => x!).ToList(),
            
            HasWhen = DocumentedAt.HasValue,
            WhenDetails = DocumentedAt.HasValue ? $"Documented: {DocumentedAt:yyyy-MM-dd}" : "Not specified",
            
            HasWhy = Purpose != DocumentationPurpose.Documentation || !string.IsNullOrEmpty(ResearchGoal) || !string.IsNullOrEmpty(PreservationAction),
            WhyDetails = new List<string> { 
                Purpose.ToString(), 
                ResearchGoal, 
                PreservationAction, 
                CommunityRequest 
            }.Where(x => !string.IsNullOrEmpty(x)).ToList(),
            
            HasHow = Methodology != DocumentationMethodology.DirectElicitation || !string.IsNullOrEmpty(Technique) || !string.IsNullOrEmpty(Protocol),
            HowDetails = new List<string> { 
                Methodology.ToString(), 
                Technique, 
                Protocol, 
                Equipment, 
                Software 
            }.Where(x => !string.IsNullOrEmpty(x)).ToList(),
            
            IsComplete = IsComplete
        };
    }
}

public enum DocumentationPurpose
{
    Documentation = 0,
    Research = 1,
    Education = 2,
    Revitalization = 3,
    LegalRights = 4,
    CulturalPreservation = 5,
    CommunityRequest = 6,
    LinguisticAnalysis = 7,
    DictionaryCreation = 8,
    GrammarDescription = 9,
    OralHistory = 10,
    LanguagePlanning = 11
}

public enum DocumentationMethodology
{
    DirectElicitation = 0,
    ParticipantObservation = 1,
    StructuredInterview = 2,
    UnstructuredInterview = 3,
    AudioRecording = 4,
    VideoRecording = 5,
    ExperimentalTask = 6,
    CorpusAnalysis = 7,
    HistoricalDocumentAnalysis = 8,
    CommunityWorkshop = 9,
    RapidAppraisal = 10,
    LongitudinalStudy = 11
}

public class W1HCompletenessReport
{
    public bool HasWho { get; set; }
    public List<string> WhoDetails { get; set; } = new();
    public bool HasWhat { get; set; }
    public List<string> WhatDetails { get; set; } = new();
    public bool HasWhere { get; set; }
    public List<string> WhereDetails { get; set; } = new();
    public bool HasWhen { get; set; }
    public string WhenDetails { get; set; } = string.Empty;
    public bool HasWhy { get; set; }
    public List<string> WhyDetails { get; set; } = new();
    public bool HasHow { get; set; }
    public List<string> HowDetails { get; set; } = new();
    public bool IsComplete { get; set; }
    
    public int CompleteDimensions => new[] { HasWho, HasWhat, HasWhere, HasWhen, HasWhy, HasHow }.Count(x => x);
    public int TotalDimensions => 6;
    public double CompletenessPercentage => (double)CompleteDimensions / TotalDimensions * 100;
    
    public override string ToString()
    {
        return $"5W1H Completeness: {CompletenessPercentage:F0}% ({CompleteDimensions}/6)\n" +
               $"WHO: {(HasWho ? "✓" : "✗")} {string.Join(", ", WhoDetails)}\n" +
               $"WHAT: {(HasWhat ? "✓" : "✗")} {string.Join(", ", WhatDetails)}\n" +
               $"WHERE: {(HasWhere ? "✓" : "✗")} {string.Join(", ", WhereDetails)}\n" +
               $"WHEN: {(HasWhen ? "✓" : "✗")} {WhenDetails}\n" +
               $"WHY: {(HasWhy ? "✓" : "✗")} {string.Join(", ", WhyDetails)}\n" +
               $"HOW: {(HasHow ? "✓" : "✗")} {string.Join(", ", HowDetails)}";
    }
}