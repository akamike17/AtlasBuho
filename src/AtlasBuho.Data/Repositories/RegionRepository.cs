using Microsoft.EntityFrameworkCore;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Repositories;

public class RegionRepository : Domain.Interfaces.IRegionRepository
{
    private readonly AtlasBuhoDbContext _context;
    public RegionRepository(AtlasBuhoDbContext context)
    {
        _context = context;
    }
    public async Task<Domain.Entities.Region?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Regions.FindAsync(new object[] { id }, cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.Region>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Regions
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.Region>> GetByCountryAsync(string country, CancellationToken cancellationToken = default)
    {
        return await _context.Regions
            .Where(r => r.Country == country)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<Domain.Entities.Region> AddAsync(Domain.Entities.Region entity, CancellationToken cancellationToken = default)
    {
        await _context.Regions.AddAsync(entity, cancellationToken);
        return entity;
    }
    public async Task UpdateAsync(Domain.Entities.Region entity, CancellationToken cancellationToken = default)
    {
        _context.Regions.Update(entity);
        await Task.CompletedTask;
    }
    public async Task DeleteAsync(Domain.Entities.Region entity, CancellationToken cancellationToken = default)
    {
        _context.Regions.Remove(entity);
        await Task.CompletedTask;
    }
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Regions.AnyAsync(r => r.Id == id, cancellationToken);
    }
}
