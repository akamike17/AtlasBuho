using Microsoft.EntityFrameworkCore;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Repositories;

public class PhraseRepository : Domain.Interfaces.IPhraseRepository
{
    private readonly AtlasBuhoDbContext _context;
    public PhraseRepository(AtlasBuhoDbContext context)
    {
        _context = context;
    }
    public async Task<Domain.Entities.Phrase?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Phrases.FindAsync(new object[] { id }, cancellationToken);
    }
    public async Task<Domain.Entities.Phrase?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Phrases
            .Include(p => p.PhraseLexemes)
                .ThenInclude(pl => pl.Lexeme)
            .Include(p => p.AudioRecordings)
            .Include(p => p.Evidence)
                .ThenInclude(e => e.Source)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.Phrase>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Phrases
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.Phrase>> GetByVariantAsync(Guid variantId, CancellationToken cancellationToken = default)
    {
        return await _context.Phrases
            .Where(p => p.LanguageVariantId == variantId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<Domain.Entities.Phrase> AddAsync(Domain.Entities.Phrase entity, CancellationToken cancellationToken = default)
    {
        await _context.Phrases.AddAsync(entity, cancellationToken);
        return entity;
    }
    public async Task UpdateAsync(Domain.Entities.Phrase entity, CancellationToken cancellationToken = default)
    {
        _context.Phrases.Update(entity);
        await Task.CompletedTask;
    }
    public async Task DeleteAsync(Domain.Entities.Phrase entity, CancellationToken cancellationToken = default)
    {
        _context.Phrases.Remove(entity);
        await Task.CompletedTask;
    }
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Phrases.AnyAsync(p => p.Id == id, cancellationToken);
    }
}
