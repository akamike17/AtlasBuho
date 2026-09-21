using Microsoft.EntityFrameworkCore;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Repositories;

public class SourceRepository : Domain.Interfaces.ISourceRepository
{
    private readonly AtlasBuhoDbContext _context;
    public SourceRepository(AtlasBuhoDbContext context)
    {
        _context = context;
    }
    public async Task<Domain.Entities.Source?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sources.FindAsync(new object[] { id }, cancellationToken);
    }
    public async Task<Domain.Entities.Source?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Sources
            .FirstOrDefaultAsync(s => s.Name == name, cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.Source>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Sources
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.Source>> GetByLevelAsync(Domain.Entities.SourceLevel level, CancellationToken cancellationToken = default)
    {
        return await _context.Sources
            .Where(s => s.Level == level)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<Domain.Entities.Source> AddAsync(Domain.Entities.Source entity, CancellationToken cancellationToken = default)
    {
        await _context.Sources.AddAsync(entity, cancellationToken);
        return entity;
    }
    public async Task UpdateAsync(Domain.Entities.Source entity, CancellationToken cancellationToken = default)
    {
        _context.Sources.Update(entity);
        await Task.CompletedTask;
    }
    public async Task DeleteAsync(Domain.Entities.Source entity, CancellationToken cancellationToken = default)
    {
        _context.Sources.Remove(entity);
        await Task.CompletedTask;
    }
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sources.AnyAsync(s => s.Id == id, cancellationToken);
    }
}
