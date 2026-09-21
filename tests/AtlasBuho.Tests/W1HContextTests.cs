using AtlasBuho.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace AtlasBuho.Tests.Domain;

public class W1HContextTests
{
    [Fact]
    public void Create_WithMinimumViableContext_ShouldBeComplete()
    {
        var speakerId = Guid.NewGuid();
        var communityId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var regionId = Guid.NewGuid();
        
        var context = W1HContext.Create(
            speakerId: speakerId,
            communityId: communityId,
            lexemeId: Guid.NewGuid(),
            regionId: regionId,
            documentedAt: DateTime.UtcNow,
            purpose: DocumentationPurpose.Documentation,
            methodology: DocumentationMethodology.DirectElicitation,
            technique: "Interview"
        );

        context.IsComplete.Should().BeTrue();
        context.SpeakerId.Should().Be(speakerId);
        context.CommunityId.Should().Be(communityId);
        context.LexemeId.Should().NotBeNull();
        context.RegionId.Should().Be(regionId);
        context.Purpose.Should().Be(DocumentationPurpose.Documentation);
        context.Methodology.Should().Be(DocumentationMethodology.DirectElicitation);
    }

    [Fact]
    public void Create_WithoutSpeakerOrCommunity_ShouldBeIncomplete()
    {
        var context = W1HContext.Create(
            lexemeId: Guid.NewGuid(),
            regionId: Guid.NewGuid(),
            documentedAt: DateTime.UtcNow,
            purpose: DocumentationPurpose.Documentation,
            methodology: DocumentationMethodology.DirectElicitation
        );

        context.IsComplete.Should().BeFalse();
    }

    [Fact]
    public void Create_WithoutWhat_ShouldBeIncomplete()
    {
        var context = W1HContext.Create(
            speakerId: Guid.NewGuid(),
            regionId: Guid.NewGuid(),
            documentedAt: DateTime.UtcNow,
            purpose: DocumentationPurpose.Documentation,
            methodology: DocumentationMethodology.DirectElicitation
        );

        context.IsComplete.Should().BeFalse();
    }

    [Fact]
    public void Create_WithoutWhere_ShouldBeIncomplete()
    {
        var context = W1HContext.Create(
            speakerId: Guid.NewGuid(),
            lexemeId: Guid.NewGuid(),
            documentedAt: DateTime.UtcNow,
            purpose: DocumentationPurpose.Documentation,
            methodology: DocumentationMethodology.DirectElicitation
        );

        context.IsComplete.Should().BeFalse();
    }

    [Fact]
    public void Create_WithoutWhen_ShouldBeIncomplete()
    {
        var context = W1HContext.Create(
            speakerId: Guid.NewGuid(),
            lexemeId: Guid.NewGuid(),
            regionId: Guid.NewGuid(),
            purpose: DocumentationPurpose.Documentation,
            methodology: DocumentationMethodology.DirectElicitation
        );

        context.IsComplete.Should().BeFalse();
    }

    [Fact]
    public void Create_WithoutWhy_ShouldBeIncomplete()
    {
        var context = W1HContext.Create(
            speakerId: Guid.NewGuid(),
            lexemeId: Guid.NewGuid(),
            regionId: Guid.NewGuid(),
            documentedAt: DateTime.UtcNow,
            methodology: DocumentationMethodology.DirectElicitation
        );

        context.IsComplete.Should().BeFalse();
    }

    [Fact]
    public void Create_WithoutHow_ShouldBeIncomplete()
    {
        var context = W1HContext.Create(
            speakerId: Guid.NewGuid(),
            lexemeId: Guid.NewGuid(),
            regionId: Guid.NewGuid(),
            documentedAt: DateTime.UtcNow,
            purpose: DocumentationPurpose.Documentation
        );

        context.IsComplete.Should().BeFalse();
    }

    [Fact]
    public void GetCompletenessReport_ShouldReportCorrectly()
    {
        var context = W1HContext.Create(
            speakerId: Guid.NewGuid(),
            communityId: Guid.NewGuid(),
            researcherId: Guid.NewGuid(),
            role: "Speaker",
            lexemeId: Guid.NewGuid(),
            phraseId: Guid.NewGuid(),
            grammarRuleId: Guid.NewGuid(),
            culturalNoteId: Guid.NewGuid(),
            entityType: "Lexeme",
            regionId: Guid.NewGuid(),
            communityLocationId: Guid.NewGuid(),
            latitude: 19.4326,
            longitude: -99.1332,
            locationDescription: "Ciudad de México",
            locationType: "Home",
            documentedAt: DateTime.UtcNow,
            era: "Contemporary",
            season: "Spring",
            timeOfDay: "Morning",
            purpose: DocumentationPurpose.Revitalization,
            researchGoal: "Dictionary creation",
            preservationAction: "Community workshops",
            communityRequest: "Language classes",
            methodology: DocumentationMethodology.CommunityWorkshop,
            technique: "Group elicitation",
            protocol: "CommunityProtocol",
            equipment: "Zoom H6",
            software: "ELAN",
            audioRecordingId: Guid.NewGuid()
        );

        var report = context.GetCompletenessReport();

        report.IsComplete.Should().BeTrue();
        report.CompleteDimensions.Should().Be(6);
        report.TotalDimensions.Should().Be(6);
        report.CompletenessPercentage.Should().Be(100);
        
        report.HasWho.Should().BeTrue();
        report.WhoDetails.Should().Contain("Speaker");
        report.WhoDetails.Should().Contain("Community");
        report.WhoDetails.Should().Contain("Researcher");

        report.HasWhat.Should().BeTrue();
        report.WhatDetails.Should().Contain("Lexeme");
        report.WhatDetails.Should().Contain("Phrase");
        report.WhatDetails.Should().Contain("GrammarRule");
        report.WhatDetails.Should().Contain("CulturalNote");
        report.WhatDetails.Should().Contain("Lexeme"); // entityType

        report.HasWhere.Should().BeTrue();
        report.WhereDetails.Should().Contain("Region");
        report.WhereDetails.Should().Contain("Community");
        report.WhereDetails.Should().Contain("Lat:19.4326");
        report.WhereDetails.Should().Contain("Lon:-99.1332");

        report.HasWhen.Should().BeTrue();
        report.WhenDetails.Should().Contain("Documented:");

        report.HasWhy.Should().BeTrue();
        report.WhyDetails.Should().Contain("Revitalization");
        report.WhyDetails.Should().Contain("Dictionary creation");
        report.WhyDetails.Should().Contain("Community workshops");
        report.WhyDetails.Should().Contain("Language classes");

        report.HasHow.Should().BeTrue();
        report.HowDetails.Should().Contain("CommunityWorkshop");
        report.HowDetails.Should().Contain("Group elicitation");
        report.HowDetails.Should().Contain("CommunityProtocol");
        report.HowDetails.Should().Contain("Zoom H6");
        report.HowDetails.Should().Contain("ELAN");
    }

    [Fact]
    public void Update_ShouldModifyPropertiesAndMarkUpdated()
    {
        var context = W1HContext.Create(
            speakerId: Guid.NewGuid(),
            lexemeId: Guid.NewGuid(),
            regionId: Guid.NewGuid(),
            documentedAt: DateTime.UtcNow,
            purpose: DocumentationPurpose.Documentation,
            methodology: DocumentationMethodology.DirectElicitation
        );

        var originalUpdatedAt = context.UpdatedAt;
        
        System.Threading.Thread.Sleep(10);
        
        context.Update(
            role: "Elder",
            entityType: "Phrase",
            locationDescription: "Community Center",
            locationType: "CommunityCenter",
            purpose: DocumentationPurpose.Revitalization,
            researchGoal: "New goal",
            methodology: DocumentationMethodology.CommunityWorkshop,
            technique: "Workshop"
        );

        context.Role.Should().Be("Elder");
        context.EntityType.Should().Be("Phrase");
        context.LocationDescription.Should().Be("Community Center");
        context.LocationType.Should().Be("CommunityCenter");
        context.Purpose.Should().Be(DocumentationPurpose.Revitalization);
        context.ResearchGoal.Should().Be("New goal");
        context.Methodology.Should().Be(DocumentationMethodology.CommunityWorkshop);
        context.Technique.Should().Be("Workshop");
        context.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void LinkEntity_ShouldUpdateEntityReference()
    {
        var context = W1HContext.Create(
            speakerId: Guid.NewGuid(),
            regionId: Guid.NewGuid(),
            documentedAt: DateTime.UtcNow,
            purpose: DocumentationPurpose.Documentation,
            methodology: DocumentationMethodology.DirectElicitation
        );

        var newLexemeId = Guid.NewGuid();
        context.LinkEntity(newLexemeId, "Lexeme");

        context.LexemeId.Should().Be(newLexemeId);
        context.EntityType.Should().Be("Lexeme");
        context.IsComplete.Should().BeTrue();
    }

    [Fact]
    public void LinkLocation_ShouldUpdateLocationInfo()
    {
        var context = W1HContext.Create(
            speakerId: Guid.NewGuid(),
            lexemeId: Guid.NewGuid(),
            regionId: Guid.NewGuid(),
            documentedAt: DateTime.UtcNow,
            purpose: DocumentationPurpose.Documentation,
            methodology: DocumentationMethodology.DirectElicitation
        );

        var newRegionId = Guid.NewGuid();
        var newCommunityId = Guid.NewGuid();
        context.LinkLocation(newRegionId, newCommunityId, 20.6597, -103.3496, "Guadalajara", "School");

        context.RegionId.Should().Be(newRegionId);
        context.CommunityLocationId.Should().Be(newCommunityId);
        context.Latitude.Should().Be(20.6597);
        context.Longitude.Should().Be(-103.3496);
        context.LocationDescription.Should().Be("Guadalajara");
        context.LocationType.Should().Be("School");
    }

    [Fact]
    public void LinkTemporal_ShouldUpdateTemporalInfo()
    {
        var context = W1HContext.Create(
            speakerId: Guid.NewGuid(),
            lexemeId: Guid.NewGuid(),
            regionId: Guid.NewGuid(),
            documentedAt: DateTime.UtcNow,
            purpose: DocumentationPurpose.Documentation,
            methodology: DocumentationMethodology.DirectElicitation
        );

        var newDate = DateTime.UtcNow.AddDays(-10);
        context.LinkTemporal(
            documentedAt: newDate,
            start: newDate.AddDays(-30),
            end: newDate.AddDays(30),
            era: "Modern",
            season: "Summer",
            timeOfDay: "Afternoon"
        );

        context.DocumentedAt.Should().Be(newDate);
        context.PeriodStart.Should().Be(newDate.AddDays(-30));
        context.PeriodEnd.Should().Be(newDate.AddDays(30));
        context.Era.Should().Be("Modern");
        context.Season.Should().Be("Summer");
        context.TimeOfDay.Should().Be("Afternoon");
    }

    [Fact]
    public void LinkPurpose_ShouldUpdatePurposeInfo()
    {
        var context = W1HContext.Create(
            speakerId: Guid.NewGuid(),
            lexemeId: Guid.NewGuid(),
            regionId: Guid.NewGuid(),
            documentedAt: DateTime.UtcNow,
            purpose: DocumentationPurpose.Documentation,
            methodology: DocumentationMethodology.DirectElicitation
        );

        context.LinkPurpose(
            purpose: DocumentationPurpose.LegalRights,
            goal: "Land rights documentation",
            action: "Submit to court",
            request: "Community petition"
        );

        context.Purpose.Should().Be(DocumentationPurpose.LegalRights);
        context.ResearchGoal.Should().Be("Land rights documentation");
        context.PreservationAction.Should().Be("Submit to court");
        context.CommunityRequest.Should().Be("Community petition");
    }

    [Fact]
    public void LinkMethodology_ShouldUpdateMethodologyInfo()
    {
        var context = W1HContext.Create(
            speakerId: Guid.NewGuid(),
            lexemeId: Guid.NewGuid(),
            regionId: Guid.NewGuid(),
            documentedAt: DateTime.UtcNow,
            purpose: DocumentationPurpose.Documentation,
            methodology: DocumentationMethodology.DirectElicitation
        );

        var audioId = Guid.NewGuid();
        var videoId = Guid.NewGuid();
        var sourceId = Guid.NewGuid();
        
        context.LinkMethodology(
            methodology: DocumentationMethodology.VideoRecording,
            technique: "Naturalistic recording",
            protocol: "INALI",
            equipment: "Sony A7S III",
            software: "DaVinci Resolve",
            audioId: audioId,
            videoId: videoId,
            sourceId: sourceId
        );

        context.Methodology.Should().Be(DocumentationMethodology.VideoRecording);
        context.Technique.Should().Be("Naturalistic recording");
        context.Protocol.Should().Be("INALI");
        context.Equipment.Should().Be("Sony A7S III");
        context.Software.Should().Be("DaVinci Resolve");
        context.AudioRecordingId.Should().Be(audioId);
        context.VideoRecordingId.Should().Be(videoId);
        context.SourceId.Should().Be(sourceId);
        context.IsComplete.Should().BeTrue();
    }

    [Fact]
    public void Create_WithCoordinates_ShouldStorePreciseLocation()
    {
        var context = W1HContext.Create(
            speakerId: Guid.NewGuid(),
            communityId: Guid.NewGuid(),
            lexemeId: Guid.NewGuid(),
            latitude: 19.432608,
            longitude: -99.133209,
            locationDescription: "Zócalo, CDMX",
            documentedAt: DateTime.UtcNow,
            purpose: DocumentationPurpose.Documentation,
            methodology: DocumentationMethodology.DirectElicitation
        );

        context.Latitude.Should().Be(19.432608);
        context.Longitude.Should().Be(-99.133209);
    }

    [Fact]
    public void Enum_DocumentationPurpose_ShouldHaveAllValues()
    {
        var values = Enum.GetValues<DocumentationPurpose>();
        values.Should().Contain(DocumentationPurpose.Documentation);
        values.Should().Contain(DocumentationPurpose.Research);
        values.Should().Contain(DocumentationPurpose.Education);
        values.Should().Contain(DocumentationPurpose.Revitalization);
        values.Should().Contain(DocumentationPurpose.LegalRights);
        values.Should().Contain(DocumentationPurpose.CulturalPreservation);
        values.Should().Contain(DocumentationPurpose.CommunityRequest);
        values.Should().Contain(DocumentationPurpose.LinguisticAnalysis);
        values.Should().Contain(DocumentationPurpose.DictionaryCreation);
        values.Should().Contain(DocumentationPurpose.GrammarDescription);
        values.Should().Contain(DocumentationPurpose.OralHistory);
        values.Should().Contain(DocumentationPurpose.LanguagePlanning);
    }

    [Fact]
    public void Enum_DocumentationMethodology_ShouldHaveAllValues()
    {
        var values = Enum.GetValues<DocumentationMethodology>();
        values.Should().Contain(DocumentationMethodology.DirectElicitation);
        values.Should().Contain(DocumentationMethodology.ParticipantObservation);
        values.Should().Contain(DocumentationMethodology.StructuredInterview);
        values.Should().Contain(DocumentationMethodology.UnstructuredInterview);
        values.Should().Contain(DocumentationMethodology.AudioRecording);
        values.Should().Contain(DocumentationMethodology.VideoRecording);
        values.Should().Contain(DocumentationMethodology.ExperimentalTask);
        values.Should().Contain(DocumentationMethodology.CorpusAnalysis);
        values.Should().Contain(DocumentationMethodology.HistoricalDocumentAnalysis);
        values.Should().Contain(DocumentationMethodology.CommunityWorkshop);
        values.Should().Contain(DocumentationMethodology.RapidAppraisal);
        values.Should().Contain(DocumentationMethodology.LongitudinalStudy);
    }
}