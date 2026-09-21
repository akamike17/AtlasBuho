using Microsoft.EntityFrameworkCore;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Repositories;

public class LexemeRepository : Domain.Interfaces.ILexemeRepository
{
    private readonly AtlasBuhoDbContext _context;
    public LexemeRepository(AtlasBuhoDbContext context)
    {
        _context = context;
    }
    public async Task<Domain.Entities.Lexeme?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Lexemes.FindAsync(new object[] { id }, cancellationToken);
    }
    public async Task<Domain.Entities.Lexeme?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Lexemes
            .Include(l => l.Pronunciations)
            .Include(l => l.Examples)
                .ThenInclude(e => e.AudioRecordings)
            .Include(l => l.Meanings)
            .Include(l => l.Phrases)
            .Include(l => l.Evidence)
                .ThenInclude(e => e.Source)
            .Include(l => l.AudioRecordings)
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.Lexeme>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Lexemes
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.Lexeme>> GetByVariantAsync(Guid variantId, CancellationToken cancellationToken = default)
    {
        return await _context.Lexemes
            .Where(l => l.LanguageVariantId == variantId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.Lexeme>> GetBySemanticDomainAsync(string domain, CancellationToken cancellationToken = default)
    {
        return await _context.Lexemes
            .Where(l => l.SemanticDomain == domain)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<Domain.Entities.Lexeme?> GetByCanonicalFormAsync(Guid variantId, string canonicalForm, CancellationToken cancellationToken = default)
    {
        return await _context.Lexemes
            .FirstOrDefaultAsync(l => l.LanguageVariantId == variantId && l.CanonicalForm == canonicalForm, cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.Lexeme>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        return await _context.Lexemes
            .Where(l => l.CanonicalForm.Contains(query) || 
                       l.SpanishMeaning!.Contains(query) || 
                       l.AlternativeForms!.Contains(query))
            .AsNoTracking()
            .Take(50)
            .ToListAsync(cancellationToken);
    }
    public async Task<Domain.Entities.Lexeme> AddAsync(Domain.Entities.Lexeme entity, CancellationToken cancellationToken = default)
    {
        await _context.Lexemes.AddAsync(entity, cancellationToken);
        return entity;
    }
    public async Task UpdateAsync(Domain.Entities.Lexeme entity, CancellationToken cancellationToken = default)
    {
        _context.Lexemes.Update(entity);
        await Task.CompletedTask;
    }
    public async Task DeleteAsync(Domain.Entities.Lexeme entity, CancellationToken cancellationToken = default)
    {
        _context.Lexemes.Remove(entity);
        await Task.CompletedTask;
    }
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Lexemes.AnyAsync(l => l.Id == id, cancellationToken);
    }
}
