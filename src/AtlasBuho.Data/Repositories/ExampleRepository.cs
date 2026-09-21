using Microsoft.EntityFrameworkCore;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Repositories;

public class ExampleRepository : Domain.Interfaces.IExampleRepository
{
    private readonly AtlasBuhoDbContext _context;
    public ExampleRepository(AtlasBuhoDbContext context)
    {
        _context = context;
    }
    public async Task<Domain.Entities.Example?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Examples.FindAsync(new object[] { id }, cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.Example>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Examples
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.Example>> GetByLexemeAsync(Guid lexemeId, CancellationToken cancellationToken = default)
    {
        return await _context.Examples
            .Where(e => e.LexemeId == lexemeId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<Domain.Entities.Example> AddAsync(Domain.Entities.Example entity, CancellationToken cancellationToken = default)
    {
        await _context.Examples.AddAsync(entity, cancellationToken);
        return entity;
    }
    public async Task UpdateAsync(Domain.Entities.Example entity, CancellationToken cancellationToken = default)
    {
        _context.Examples.Update(entity);
        await Task.CompletedTask;
    }
    public async Task DeleteAsync(Domain.Entities.Example entity, CancellationToken cancellationToken = default)
    {
        _context.Examples.Remove(entity);
        await Task.CompletedTask;
    }
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Examples.AnyAsync(e => e.Id == id, cancellationToken);
    }
}
