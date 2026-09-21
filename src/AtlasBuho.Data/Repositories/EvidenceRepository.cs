using Microsoft.EntityFrameworkCore;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Repositories;

public class EvidenceRepository : Domain.Interfaces.IEvidenceRepository
{
    private readonly AtlasBuhoDbContext _context;
    public EvidenceRepository(AtlasBuhoDbContext context)
    {
        _context = context;
    }
    public async Task<Domain.Entities.Evidence?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Evidence.FindAsync(new object[] { id }, cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.Evidence>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Evidence
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.Evidence>> GetBySourceAsync(Guid sourceId, CancellationToken cancellationToken = default)
    {
        return await _context.Evidence
            .Where(e => e.SourceId == sourceId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.Evidence>> GetByEntityAsync(string entityType, Guid entityId, CancellationToken cancellationToken = default)
    {
        return await _context.Evidence
            .Where(e => e.EntityType == entityType && e.EntityId == entityId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<Domain.Entities.Evidence> AddAsync(Domain.Entities.Evidence entity, CancellationToken cancellationToken = default)
    {
        await _context.Evidence.AddAsync(entity, cancellationToken);
        return entity;
    }
    public async Task UpdateAsync(Domain.Entities.Evidence entity, CancellationToken cancellationToken = default)
    {
        _context.Evidence.Update(entity);
        await Task.CompletedTask;
    }
    public async Task DeleteAsync(Domain.Entities.Evidence entity, CancellationToken cancellationToken = default)
    {
        _context.Evidence.Remove(entity);
        await Task.CompletedTask;
    }
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Evidence.AnyAsync(e => e.Id == id, cancellationToken);
    }
}
