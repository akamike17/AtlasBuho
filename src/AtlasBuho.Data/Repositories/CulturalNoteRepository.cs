using Microsoft.EntityFrameworkCore;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Repositories;

public class CulturalNoteRepository : Domain.Interfaces.ICulturalNoteRepository
{
    private readonly AtlasBuhoDbContext _context;
    public CulturalNoteRepository(AtlasBuhoDbContext context)
    {
        _context = context;
    }
    public async Task<Domain.Entities.CulturalNote?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.CulturalNotes.FindAsync(new object[] { id }, cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.CulturalNote>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.CulturalNotes
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.CulturalNote>> GetByVariantAsync(Guid variantId, CancellationToken cancellationToken = default)
    {
        return await _context.CulturalNotes
            .Where(c => c.LanguageVariantId == variantId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.CulturalNote>> GetByTypeAsync(Domain.Entities.CulturalNoteType type, CancellationToken cancellationToken = default)
    {
        return await _context.CulturalNotes
            .Where(c => c.Type == type)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.CulturalNote>> GetByCommunityAsync(Guid communityId, CancellationToken cancellationToken = default)
    {
        return await _context.CulturalNotes
            .Where(c => c.CommunityId == communityId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<Domain.Entities.CulturalNote> AddAsync(Domain.Entities.CulturalNote entity, CancellationToken cancellationToken = default)
    {
        await _context.CulturalNotes.AddAsync(entity, cancellationToken);
        return entity;
    }
    public async Task UpdateAsync(Domain.Entities.CulturalNote entity, CancellationToken cancellationToken = default)
    {
        _context.CulturalNotes.Update(entity);
        await Task.CompletedTask;
    }
    public async Task DeleteAsync(Domain.Entities.CulturalNote entity, CancellationToken cancellationToken = default)
    {
        _context.CulturalNotes.Remove(entity);
        await Task.CompletedTask;
    }
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.CulturalNotes.AnyAsync(c => c.Id == id, cancellationToken);
    }
}
