using Microsoft.EntityFrameworkCore;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Repositories;

public class MeaningRepository : Domain.Interfaces.IMeaningRepository
{
    private readonly AtlasBuhoDbContext _context;
    public MeaningRepository(AtlasBuhoDbContext context)
    {
        _context = context;
    }
    public async Task<Domain.Entities.Meaning?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Meanings.FindAsync(new object[] { id }, cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.Meaning>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Meanings
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.Meaning>> GetByLexemeAsync(Guid lexemeId, CancellationToken cancellationToken = default)
    {
        return await _context.Meanings
            .Where(m => m.LexemeId == lexemeId)
            .OrderBy(m => m.Order)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<Domain.Entities.Meaning> AddAsync(Domain.Entities.Meaning entity, CancellationToken cancellationToken = default)
    {
        await _context.Meanings.AddAsync(entity, cancellationToken);
        return entity;
    }
    public async Task UpdateAsync(Domain.Entities.Meaning entity, CancellationToken cancellationToken = default)
    {
        _context.Meanings.Update(entity);
        await Task.CompletedTask;
    }
    public async Task DeleteAsync(Domain.Entities.Meaning entity, CancellationToken cancellationToken = default)
    {
        _context.Meanings.Remove(entity);
        await Task.CompletedTask;
    }
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Meanings.AnyAsync(m => m.Id == id, cancellationToken);
    }
}
