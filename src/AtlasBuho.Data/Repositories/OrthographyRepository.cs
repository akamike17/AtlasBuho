using Microsoft.EntityFrameworkCore;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Repositories;

public class OrthographyRepository : Domain.Interfaces.IOrthographyRepository
{
    private readonly AtlasBuhoDbContext _context;
    public OrthographyRepository(AtlasBuhoDbContext context)
    {
        _context = context;
    }
    public async Task<Domain.Entities.Orthography?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Orthographies.FindAsync(new object[] { id }, cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.Orthography>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Orthographies
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.Orthography>> GetByVariantAsync(Guid variantId, CancellationToken cancellationToken = default)
    {
        return await _context.Orthographies
            .Where(o => o.LanguageVariantId == variantId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.Orthography>> GetByWritingSystemAsync(Guid writingSystemId, CancellationToken cancellationToken = default)
    {
        return await _context.Orthographies
            .Where(o => o.WritingSystemId == writingSystemId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<Domain.Entities.Orthography> AddAsync(Domain.Entities.Orthography entity, CancellationToken cancellationToken = default)
    {
        await _context.Orthographies.AddAsync(entity, cancellationToken);
        return entity;
    }
    public async Task UpdateAsync(Domain.Entities.Orthography entity, CancellationToken cancellationToken = default)
    {
        _context.Orthographies.Update(entity);
        await Task.CompletedTask;
    }
    public async Task DeleteAsync(Domain.Entities.Orthography entity, CancellationToken cancellationToken = default)
    {
        _context.Orthographies.Remove(entity);
        await Task.CompletedTask;
    }
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Orthographies.AnyAsync(o => o.Id == id, cancellationToken);
    }
}
