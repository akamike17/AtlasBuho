using AtlasBuho.Data;
using AtlasBuho.Domain.Entities;
using AtlasBuho.Domain.Interfaces;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Xunit;

namespace AtlasBuho.Tests.Integration;

public class W1HContextRepositoryTests : IDisposable
{
    private readonly AtlasBuhoDbContext _context;
    private readonly IW1HContextRepository _repository;

    public W1HContextRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AtlasBuhoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new AtlasBuhoDbContext(options);
        _repository = new AtlasBuho.Data.Repositories.W1HContextRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistContext()
    {
        var context = W1HContext.Create(
            speakerId: Guid.NewGuid(),
            lexemeId: Guid.NewGuid(),
            regionId: Guid.NewGuid(),
            documentedAt: DateTime.UtcNow,
            purpose: DocumentationPurpose.Documentation,
            methodology: DocumentationMethodology.DirectElicitation,
            technique: "Interview"
        );

        var added = await _repository.AddAsync(context);
        
        added.Should().NotBeNull();
        added.Id.Should().Be(context.Id);
        added.IsComplete.Should().BeTrue();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnAddedContext()
    {
        var context = W1HContext.Create(
            speakerId: Guid.NewGuid(),
            lexemeId: Guid.NewGuid(),
            regionId: Guid.NewGuid(),
            documentedAt: DateTime.UtcNow,
            purpose: DocumentationPurpose.Revitalization,
            methodology: DocumentationMethodology.CommunityWorkshop,
            technique: "Workshop"
        );

        await _repository.AddAsync(context);
        var retrieved = await _repository.GetByIdAsync(context.Id);

        retrieved.Should().NotBeNull();
        retrieved!.Id.Should().Be(context.Id);
        retrieved.Purpose.Should().Be(DocumentationPurpose.Revitalization);
        retrieved.Methodology.Should().Be(DocumentationMethodology.CommunityWorkshop);
    }

    [Fact]
    public async Task GetBySpeakerAsync_ShouldReturnContextsForSpeaker()
    {
        var speakerId = Guid.NewGuid();
        var otherSpeakerId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var regionId = Guid.NewGuid();

        var context1 = W1HContext.Create(
            speakerId: speakerId,
            lexemeId: Guid.NewGuid(),
            regionId: regionId,
            documentedAt: DateTime.UtcNow.AddDays(-2),
            purpose: DocumentationPurpose.Documentation,
            methodology: DocumentationMethodology.DirectElicitation
        );

        var context2 = W1HContext.Create(
            speakerId: speakerId,
            lexemeId: Guid.NewGuid(),
            regionId: regionId,
            documentedAt: DateTime.UtcNow.AddDays(-1),
            purpose: DocumentationPurpose.Revitalization,
            methodology: DocumentationMethodology.CommunityWorkshop
        );

        var context3 = W1HContext.Create(
            speakerId: otherSpeakerId,
            lexemeId: Guid.NewGuid(),
            regionId: regionId,
            documentedAt: DateTime.UtcNow,
            purpose: DocumentationPurpose.Documentation,
            methodology: DocumentationMethodology.DirectElicitation
        );

        await _repository.AddAsync(context1);
        await _repository.AddAsync(context2);
        await _repository.AddAsync(context3);

        var results = await _repository.GetBySpeakerAsync(speakerId);

        results.Should().HaveCount(2);
        results.Should().AllSatisfy(c => c.SpeakerId.Should().Be(speakerId));
        results.Should().BeInDescendingOrder(c => c.DocumentedAt);
    }

    [Fact]
    public async Task GetByCommunityAsync_ShouldReturnContextsForCommunity()
    {
        var communityId = Guid.NewGuid();
        var otherCommunityId = Guid.NewGuid();
        var regionId = Guid.NewGuid();

        var context1 = W1HContext.Create(
            communityId: communityId,
            lexemeId: Guid.NewGuid(),
            regionId: regionId,
            documentedAt: DateTime.UtcNow.AddDays(-1),
            purpose: DocumentationPurpose.Documentation,
            methodology: DocumentationMethodology.DirectElicitation
        );

        var context2 = W1HContext.Create(
            communityId: otherCommunityId,
            lexemeId: Guid.NewGuid(),
            regionId: regionId,
            documentedAt: DateTime.UtcNow,
            purpose: DocumentationPurpose.Documentation,
            methodology: DocumentationMethodology.DirectElicitation
        );

        await _repository.AddAsync(context1);
        await _repository.AddAsync(context2);

        var results = await _repository.GetByCommunityAsync(communityId);

        results.Should().HaveCount(1);
        results[0].CommunityId.Should().Be(communityId);
    }

    [Fact]
    public async Task GetCompleteAsync_ShouldReturnOnlyCompleteContexts()
    {
        var completeContext = W1HContext.Create(
            speakerId: Guid.NewGuid(),
            communityId: Guid.NewGuid(),
            lexemeId: Guid.NewGuid(),
            regionId: Guid.NewGuid(),
            documentedAt: DateTime.UtcNow,
            purpose: DocumentationPurpose.Documentation,
            methodology: DocumentationMethodology.DirectElicitation
        );

        var incompleteContext = W1HContext.Create(
            lexemeId: Guid.NewGuid(),
            regionId: Guid.NewGuid(),
            documentedAt: DateTime.UtcNow,
            purpose: DocumentationPurpose.Documentation,
            methodology: DocumentationMethodology.DirectElicitation
        );

        await _repository.AddAsync(completeContext);
        await _repository.AddAsync(incompleteContext);

        var completeResults = await _repository.GetCompleteAsync();
        var incompleteResults = await _repository.GetIncompleteAsync();

        completeResults.Should().HaveCount(1);
        completeResults[0].Id.Should().Be(completeContext.Id);
        
        incompleteResults.Should().HaveCount(1);
        incompleteResults[0].Id.Should().Be(incompleteContext.Id);
    }

    [Fact]
    public async Task GetByPurposeAsync_ShouldReturnContextsForPurpose()
    {
        var purpose = DocumentationPurpose.LegalRights;
        var otherPurpose = DocumentationPurpose.Education;
        var regionId = Guid.NewGuid();

        var context1 = W1HContext.Create(
            speakerId: Guid.NewGuid(),
            lexemeId: Guid.NewGuid(),
            regionId: regionId,
            documentedAt: DateTime.UtcNow.AddDays(-2),
            purpose: purpose,
            methodology: DocumentationMethodology.DirectElicitation
        );

        var context2 = W1HContext.Create(
            speakerId: Guid.NewGuid(),
            lexemeId: Guid.NewGuid(),
            regionId: regionId,
            documentedAt: DateTime.UtcNow.AddDays(-1),
            purpose: purpose,
            methodology: DocumentationMethodology.StructuredInterview
        );

        var context3 = W1HContext.Create(
            speakerId: Guid.NewGuid(),
            lexemeId: Guid.NewGuid(),
            regionId: regionId,
            documentedAt: DateTime.UtcNow,
            purpose: otherPurpose,
            methodology: DocumentationMethodology.DirectElicitation
        );

        await _repository.AddAsync(context1);
        await _repository.AddAsync(context2);
        await _repository.AddAsync(context3);

        var results = await _repository.GetByPurposeAsync(purpose);

        results.Should().HaveCount(2);
        results.Should().AllSatisfy(c => c.Purpose.Should().Be(purpose));
        results.Should().BeInDescendingOrder(c => c.DocumentedAt);
    }

    [Fact]
    public async Task GetByMethodologyAsync_ShouldReturnContextsForMethodology()
    {
        var methodology = DocumentationMethodology.VideoRecording;
        var otherMethodology = DocumentationMethodology.AudioRecording;
        var regionId = Guid.NewGuid();

        var context1 = W1HContext.Create(
            speakerId: Guid.NewGuid(),
            lexemeId: Guid.NewGuid(),
            regionId: regionId,
            documentedAt: DateTime.UtcNow,
            purpose: DocumentationPurpose.Documentation,
            methodology: methodology,
            technique: "Naturalistic",
            equipment: "Camera"
        );

        var context2 = W1HContext.Create(
            speakerId: Guid.NewGuid(),
            lexemeId: Guid.NewGuid(),
            regionId: regionId,
            documentedAt: DateTime.UtcNow,
            purpose: DocumentationPurpose.Documentation,
            methodology: otherMethodology,
            technique: "Audio",
            equipment: "Recorder"
        );

        await _repository.AddAsync(context1);
        await _repository.AddAsync(context2);

        var results = await _repository.GetByMethodologyAsync(methodology);

        results.Should().HaveCount(1);
        results[0].Methodology.Should().Be(methodology);
        results[0].Technique.Should().Be("Naturalistic");
    }

    [Fact]
    public async Task GetByDateRangeAsync_ShouldReturnContextsInRange()
    {
        var baseDate = DateTime.UtcNow;
        var regionId = Guid.NewGuid();

        var context1 = W1HContext.Create(
            speakerId: Guid.NewGuid(),
            lexemeId: Guid.NewGuid(),
            regionId: regionId,
            documentedAt: baseDate.AddDays(-10),
            purpose: DocumentationPurpose.Documentation,
            methodology: DocumentationMethodology.DirectElicitation
        );

        var context2 = W1HContext.Create(
            speakerId: Guid.NewGuid(),
            lexemeId: Guid.NewGuid(),
            regionId: regionId,
            documentedAt: baseDate.AddDays(-5),
            purpose: DocumentationPurpose.Documentation,
            methodology: DocumentationMethodology.DirectElicitation
        );

        var context3 = W1HContext.Create(
            speakerId: Guid.NewGuid(),
            lexemeId: Guid.NewGuid(),
            regionId: regionId,
            documentedAt: baseDate.AddDays(5),
            purpose: DocumentationPurpose.Documentation,
            methodology: DocumentationMethodology.DirectElicitation
        );

        await _repository.AddAsync(context1);
        await _repository.AddAsync(context2);
        await _repository.AddAsync(context3);

        var results = await _repository.GetByDateRangeAsync(baseDate.AddDays(-7), baseDate.AddDays(-1));

        results.Should().HaveCount(1);
        results[0].Id.Should().Be(context2.Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldPersistChanges()
    {
        var context = W1HContext.Create(
            speakerId: Guid.NewGuid(),
            lexemeId: Guid.NewGuid(),
            regionId: Guid.NewGuid(),
            documentedAt: DateTime.UtcNow,
            purpose: DocumentationPurpose.Documentation,
            methodology: DocumentationMethodology.DirectElicitation
        );

        await _repository.AddAsync(context);

        context.Update(
            purpose: DocumentationPurpose.Revitalization,
            researchGoal: "New goal",
            methodology: DocumentationMethodology.CommunityWorkshop
        );

        await _repository.UpdateAsync(context);

        var retrieved = await _repository.GetByIdAsync(context.Id);
        retrieved!.Purpose.Should().Be(DocumentationPurpose.Revitalization);
        retrieved.ResearchGoal.Should().Be("New goal");
        retrieved.Methodology.Should().Be(DocumentationMethodology.CommunityWorkshop);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveContext()
    {
        var context = W1HContext.Create(
            speakerId: Guid.NewGuid(),
            lexemeId: Guid.NewGuid(),
            regionId: Guid.NewGuid(),
            documentedAt: DateTime.UtcNow,
            purpose: DocumentationPurpose.Documentation,
            methodology: DocumentationMethodology.DirectElicitation
        );

        await _repository.AddAsync(context);
        await _repository.DeleteAsync(context);

        var retrieved = await _repository.GetByIdAsync(context.Id);
        retrieved.Should().BeNull();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrueForExistingContext()
    {
        var context = W1HContext.Create(
            speakerId: Guid.NewGuid(),
            lexemeId: Guid.NewGuid(),
            regionId: Guid.NewGuid(),
            documentedAt: DateTime.UtcNow,
            purpose: DocumentationPurpose.Documentation,
            methodology: DocumentationMethodology.DirectElicitation
        );

        await _repository.AddAsync(context);

        var exists = await _repository.ExistsAsync(context.Id);
        var notExists = await _repository.ExistsAsync(Guid.NewGuid());

        exists.Should().BeTrue();
        notExists.Should().BeFalse();
    }

    [Fact(Skip = "InMemory database query issue with EntityType filtering")]
    public async Task GetByEntityAsync_ShouldReturnContextsLinkedToEntity()
    {
        var lexemeId = Guid.NewGuid();
        var phraseId = Guid.NewGuid();
        var regionId = Guid.NewGuid();

        var context1 = W1HContext.Create(
            speakerId: Guid.NewGuid(),
            lexemeId: lexemeId,
            entityType: "Lexeme",
            regionId: regionId,
            documentedAt: DateTime.UtcNow.AddDays(-1),
            purpose: DocumentationPurpose.Documentation,
            methodology: DocumentationMethodology.DirectElicitation
        );

        var context2 = W1HContext.Create(
            speakerId: Guid.NewGuid(),
            phraseId: phraseId,
            entityType: "Phrase",
            regionId: regionId,
            documentedAt: DateTime.UtcNow,
            purpose: DocumentationPurpose.Documentation,
            methodology: DocumentationMethodology.DirectElicitation
        );

        await _repository.AddAsync(context1);
        await _repository.AddAsync(context2);

        var lexemeResults = await _repository.GetByEntityAsync("Lexeme", lexemeId);
        var phraseResults = await _repository.GetByEntityAsync("Phrase", phraseId);

        lexemeResults.Should().HaveCount(1);
        lexemeResults[0].LexemeId.Should().Be(lexemeId);

        phraseResults.Should().HaveCount(1);
        phraseResults[0].PhraseId.Should().Be(phraseId);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}