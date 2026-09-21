using Microsoft.EntityFrameworkCore;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Repositories;

public class WritingSystemRepository : Domain.Interfaces.IWritingSystemRepository
{
    private readonly AtlasBuhoDbContext _context;
    public WritingSystemRepository(AtlasBuhoDbContext context)
    {
        _context = context;
    }
    public async Task<Domain.Entities.WritingSystem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.WritingSystems.FindAsync(new object[] { id }, cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.WritingSystem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.WritingSystems
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.WritingSystem>> GetByVariantAsync(Guid variantId, CancellationToken cancellationToken = default)
    {
        return await _context.WritingSystems
            .Where(w => w.LanguageVariantId == variantId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<Domain.Entities.WritingSystem?> GetOfficialAsync(Guid variantId, CancellationToken cancellationToken = default)
    {
        return await _context.WritingSystems
            .FirstOrDefaultAsync(w => w.LanguageVariantId == variantId && w.IsOfficial, cancellationToken);
    }
    public async Task<Domain.Entities.WritingSystem> AddAsync(Domain.Entities.WritingSystem entity, CancellationToken cancellationToken = default)
    {
        await _context.WritingSystems.AddAsync(entity, cancellationToken);
        return entity;
    }
    public async Task UpdateAsync(Domain.Entities.WritingSystem entity, CancellationToken cancellationToken = default)
    {
        _context.WritingSystems.Update(entity);
        await Task.CompletedTask;
    }
    public async Task DeleteAsync(Domain.Entities.WritingSystem entity, CancellationToken cancellationToken = default)
    {
        _context.WritingSystems.Remove(entity);
        await Task.CompletedTask;
    }
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.WritingSystems.AnyAsync(w => w.Id == id, cancellationToken);
    }
}
