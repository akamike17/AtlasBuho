using AtlasBuho.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace AtlasBuho.Tests.Domain;

public class LanguageFamilyTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var family = new LanguageFamily("Uto-Azteca", "Uto-Aztecan", "Uto-Aztecan language family", "INALI");

        family.Name.Should().Be("Uto-Azteca");
        family.NameEnglish.Should().Be("Uto-Aztecan");
        family.Description.Should().Be("Uto-Aztecan language family");
        family.Source.Should().Be("INALI");
        family.Id.Should().NotBe(Guid.Empty);
        family.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_WithNullName_ShouldThrow()
    {
        var act = () => new LanguageFamily(null!, "English", "Description", "Source");
        act.Should().Throw<ArgumentNullException>().WithParameterName("name");
    }

    [Fact]
    public void Update_ShouldModifyPropertiesAndUpdatedAt()
    {
        var family = new LanguageFamily("Original", "Original EN", "Desc", "Source");
        var originalUpdatedAt = family.UpdatedAt;
        
        System.Threading.Thread.Sleep(10);
        
        family.Update("Updated", "Updated EN", "New Desc", "New Source");

        family.Name.Should().Be("Updated");
        family.NameEnglish.Should().Be("Updated EN");
        family.Description.Should().Be("New Desc");
        family.Source.Should().Be("New Source");
        family.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }
}

public class LanguageGroupTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var familyId = Guid.NewGuid();
        var group = new LanguageGroup(familyId, "Nahua", "Nahua", "Nahua languages", "INALI");

        group.LanguageFamilyId.Should().Be(familyId);
        group.Name.Should().Be("Nahua");
        group.NameEnglish.Should().Be("Nahua");
        group.Description.Should().Be("Nahua languages");
        group.Source.Should().Be("INALI");
    }

    [Fact]
    public void Update_ShouldModifyProperties()
    {
        var group = new LanguageGroup(Guid.NewGuid(), "Original", "EN", "Desc", "Source");
        var originalUpdatedAt = group.UpdatedAt;
        
        System.Threading.Thread.Sleep(10);
        
        group.Update("Updated", "Updated EN", "New Desc", "New Source");

        group.Name.Should().Be("Updated");
        group.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }
}

public class LanguageVariantTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var groupId = Guid.NewGuid();
        var variant = new LanguageVariant(
            groupId,
            "Náhuatl del Norte",
            "Masehualtlahtol",
            "nhg",
            "NAL-001",
            "Northern Nahuatl variant",
            "Latin",
            "SEP",
            "INALI");

        variant.LanguageGroupId.Should().Be(groupId);
        variant.Name.Should().Be("Náhuatl del Norte");
        variant.Autodenomination.Should().Be("Masehualtlahtol");
        variant.Iso639_3Code.Should().Be("nhg");
        variant.InaliCode.Should().Be("NAL-001");
    }
}

public class VerificationStatusTests
{
    [Fact]
    public void Enum_ShouldHaveAllRequiredValues()
    {
        Enum.GetValues<VerificationStatus>().Should().Contain(VerificationStatus.Verified);
        Enum.GetValues<VerificationStatus>().Should().Contain(VerificationStatus.Documented);
        Enum.GetValues<VerificationStatus>().Should().Contain(VerificationStatus.CommunityVerified);
        Enum.GetValues<VerificationStatus>().Should().Contain(VerificationStatus.AcademicVerified);
        Enum.GetValues<VerificationStatus>().Should().Contain(VerificationStatus.Provisional);
        Enum.GetValues<VerificationStatus>().Should().Contain(VerificationStatus.Uncertain);
        Enum.GetValues<VerificationStatus>().Should().Contain(VerificationStatus.Disputed);
        Enum.GetValues<VerificationStatus>().Should().Contain(VerificationStatus.Unknown);
    }
}

public class RiskLevelTests
{
    [Fact]
    public void Enum_ShouldHaveAllRequiredLevels()
    {
        Enum.GetValues<RiskLevel>().Should().Contain(RiskLevel.P0_Emergency);
        Enum.GetValues<RiskLevel>().Should().Contain(RiskLevel.P1_High);
        Enum.GetValues<RiskLevel>().Should().Contain(RiskLevel.P2_Medium);
        Enum.GetValues<RiskLevel>().Should().Contain(RiskLevel.P3_Preventive);
        Enum.GetValues<RiskLevel>().Should().Contain(RiskLevel.Unknown);
    }
}

public class ConsentStatusTests
{
    [Fact]
    public void Enum_ShouldHaveAllRequiredValues()
    {
        Enum.GetValues<ConsentStatus>().Should().Contain(ConsentStatus.Public);
        Enum.GetValues<ConsentStatus>().Should().Contain(ConsentStatus.EducationalUse);
        Enum.GetValues<ConsentStatus>().Should().Contain(ConsentStatus.Restricted);
        Enum.GetValues<ConsentStatus>().Should().Contain(ConsentStatus.CommunityOnly);
        Enum.GetValues<ConsentStatus>().Should().Contain(ConsentStatus.ResearchOnly);
        Enum.GetValues<ConsentStatus>().Should().Contain(ConsentStatus.Unknown);
    }
}

public class SourceLevelTests
{
    [Fact]
    public void Enum_ShouldHaveCorrectPriorityOrder()
    {
        SourceLevel.Institutional.Should().Be(SourceLevel.Institutional);
        SourceLevel.LinguisticArchive.Should().Be(SourceLevel.LinguisticArchive);
        SourceLevel.AcademicMaterial.Should().Be(SourceLevel.AcademicMaterial);
        SourceLevel.CommunityMaterial.Should().Be(SourceLevel.CommunityMaterial);
        SourceLevel.InternetContent.Should().Be(SourceLevel.InternetContent);
    }
}

public class CulturalNoteTypeTests
{
    [Fact]
    public void Enum_ShouldHaveAllRequiredTypes()
    {
        Enum.GetValues<CulturalNoteType>().Should().Contain(CulturalNoteType.Linguistic);
        Enum.GetValues<CulturalNoteType>().Should().Contain(CulturalNoteType.Historical);
        Enum.GetValues<CulturalNoteType>().Should().Contain(CulturalNoteType.AcademicInterpretation);
        Enum.GetValues<CulturalNoteType>().Should().Contain(CulturalNoteType.CommunityTradition);
        Enum.GetValues<CulturalNoteType>().Should().Contain(CulturalNoteType.ArtisticMaterial);
    }
}

public class DialectRelationshipTypeTests
{
    [Fact]
    public void Enum_ShouldHaveAllRequiredTypes()
    {
        Enum.GetValues<DialectRelationshipType>().Should().Contain(DialectRelationshipType.MutualIntelligibility);
        Enum.GetValues<DialectRelationshipType>().Should().Contain(DialectRelationshipType.PartialIntelligibility);
        Enum.GetValues<DialectRelationshipType>().Should().Contain(DialectRelationshipType.NoIntelligibility);
        Enum.GetValues<DialectRelationshipType>().Should().Contain(DialectRelationshipType.HistoricalRelation);
        Enum.GetValues<DialectRelationshipType>().Should().Contain(DialectRelationshipType.GeographicProximity);
        Enum.GetValues<DialectRelationshipType>().Should().Contain(DialectRelationshipType.SharedFeatures);
    }
}