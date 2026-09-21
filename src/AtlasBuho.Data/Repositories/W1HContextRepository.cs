namespace AtlasBuho.Data.Repositories;

using Microsoft.EntityFrameworkCore;
using AtlasBuho.Domain.Entities;
using AtlasBuho.Domain.Interfaces;

public class W1HContextRepository : IW1HContextRepository
{
    private readonly AtlasBuhoDbContext _context;

    public W1HContextRepository(AtlasBuhoDbContext context)
    {
        _context = context;
    }

    public async Task<W1HContext?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.W1HContexts.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IReadOnlyList<W1HContext>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.W1HContexts.ToListAsync(cancellationToken);
    }

    public async Task<W1HContext> AddAsync(W1HContext entity, CancellationToken cancellationToken = default)
    {
        _context.W1HContexts.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(W1HContext entity, CancellationToken cancellationToken = default)
    {
        _context.W1HContexts.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(W1HContext entity, CancellationToken cancellationToken = default)
    {
        _context.W1HContexts.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.W1HContexts.AnyAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<W1HContext>> GetBySpeakerAsync(Guid speakerId, CancellationToken cancellationToken = default)
    {
        return await _context.W1HContexts
            .Where(c => c.SpeakerId == speakerId)
            .OrderByDescending(c => c.DocumentedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<W1HContext>> GetByCommunityAsync(Guid communityId, CancellationToken cancellationToken = default)
    {
        return await _context.W1HContexts
            .Where(c => c.CommunityId == communityId)
            .OrderByDescending(c => c.DocumentedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<W1HContext>> GetByVariantAsync(Guid variantId, CancellationToken cancellationToken = default)
    {
        // Get contexts linked through lexeme, phrase, grammar rule, or cultural note of this variant
        var lexemeIds = await _context.Lexemes
            .Where(l => l.LanguageVariantId == variantId)
            .Select(l => l.Id)
            .ToListAsync(cancellationToken);
        
        var phraseIds = await _context.Phrases
            .Where(p => p.LanguageVariantId == variantId)
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);
        
        var grammarIds = await _context.GrammarRules
            .Where(g => g.LanguageVariantId == variantId)
            .Select(g => g.Id)
            .ToListAsync(cancellationToken);
        
        var culturalNoteIds = await _context.CulturalNotes
            .Where(c => c.LanguageVariantId == variantId)
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        return await _context.W1HContexts
            .Where(c => 
                (c.LexemeId.HasValue && lexemeIds.Contains(c.LexemeId.Value)) ||
                (c.PhraseId.HasValue && phraseIds.Contains(c.PhraseId.Value)) ||
                (c.GrammarRuleId.HasValue && grammarIds.Contains(c.GrammarRuleId.Value)) ||
                (c.CulturalNoteId.HasValue && culturalNoteIds.Contains(c.CulturalNoteId.Value)))
            .OrderByDescending(c => c.DocumentedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<W1HContext>> GetByEntityAsync(string entityType, Guid entityId, CancellationToken cancellationToken = default)
    {
        return await _context.W1HContexts
            .Where(c => c.EntityType == entityType && 
                ((entityType == "Lexeme" && c.LexemeId == entityId) ||
                 (entityType == "Phrase" && c.PhraseId == entityId) ||
                 (entityType == "GrammarRule" && c.GrammarRuleId == entityId) ||
                 (entityType == "CulturalNote" && c.CulturalNoteId == entityId) ||
                 (entityType == "Recording" && (c.AudioRecordingId == entityId || c.VideoRecordingId == entityId))))
            .OrderByDescending(c => c.DocumentedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<W1HContext>> GetCompleteAsync(CancellationToken cancellationToken = default)
    {
        return await _context.W1HContexts
            .Where(c => c.IsComplete)
            .OrderByDescending(c => c.DocumentedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<W1HContext>> GetIncompleteAsync(CancellationToken cancellationToken = default)
    {
        return await _context.W1HContexts
            .Where(c => !c.IsComplete)
            .OrderByDescending(c => c.DocumentedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<W1HContext>> GetByPurposeAsync(DocumentationPurpose purpose, CancellationToken cancellationToken = default)
    {
        return await _context.W1HContexts
            .Where(c => c.Purpose == purpose)
            .OrderByDescending(c => c.DocumentedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<W1HContext>> GetByMethodologyAsync(DocumentationMethodology methodology, CancellationToken cancellationToken = default)
    {
        return await _context.W1HContexts
            .Where(c => c.Methodology == methodology)
            .OrderByDescending(c => c.DocumentedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<W1HContext>> GetByDateRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        return await _context.W1HContexts
            .Where(c => c.DocumentedAt >= start && c.DocumentedAt <= end)
            .OrderByDescending(c => c.DocumentedAt)
            .ToListAsync(cancellationToken);
    }
}