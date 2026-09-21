using Microsoft.EntityFrameworkCore;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Repositories;

public class CommunityRepository : Domain.Interfaces.ICommunityRepository
{
    private readonly AtlasBuhoDbContext _context;
    public CommunityRepository(AtlasBuhoDbContext context)
    {
        _context = context;
    }
    public async Task<Domain.Entities.Community?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Communities.FindAsync(new object[] { id }, cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.Community>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Communities
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.Community>> GetByVariantAsync(Guid variantId, CancellationToken cancellationToken = default)
    {
        return await _context.Communities
            .Where(c => c.LanguageVariantId == variantId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.Community>> GetByRegionAsync(Guid regionId, CancellationToken cancellationToken = default)
    {
        return await _context.Communities
            .Where(c => c.RegionId == regionId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.Community>> GetByStateAsync(string state, CancellationToken cancellationToken = default)
    {
        return await _context.Communities
            .Where(c => c.State == state)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<Domain.Entities.Community> AddAsync(Domain.Entities.Community entity, CancellationToken cancellationToken = default)
    {
        await _context.Communities.AddAsync(entity, cancellationToken);
        return entity;
    }
    public async Task UpdateAsync(Domain.Entities.Community entity, CancellationToken cancellationToken = default)
    {
        _context.Communities.Update(entity);
        await Task.CompletedTask;
    }
    public async Task DeleteAsync(Domain.Entities.Community entity, CancellationToken cancellationToken = default)
    {
        _context.Communities.Remove(entity);
        await Task.CompletedTask;
    }
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Communities.AnyAsync(c => c.Id == id, cancellationToken);
    }
}
