using Microsoft.EntityFrameworkCore;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Repositories;

public class LanguageGroupRepository : Domain.Interfaces.ILanguageGroupRepository
{
    private readonly AtlasBuhoDbContext _context;
    public LanguageGroupRepository(AtlasBuhoDbContext context)
    {
        _context = context;
    }
    public async Task<Domain.Entities.LanguageGroup?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.LanguageGroups.FindAsync(new object[] { id }, cancellationToken);
    }
    public async Task<Domain.Entities.LanguageGroup?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.LanguageGroups
            .FirstOrDefaultAsync(g => g.Name == name, cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.LanguageGroup>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.LanguageGroups
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.LanguageGroup>> GetByFamilyAsync(Guid familyId, CancellationToken cancellationToken = default)
    {
        return await _context.LanguageGroups
            .Where(g => g.LanguageFamilyId == familyId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<Domain.Entities.LanguageGroup> AddAsync(Domain.Entities.LanguageGroup entity, CancellationToken cancellationToken = default)
    {
        await _context.LanguageGroups.AddAsync(entity, cancellationToken);
        return entity;
    }
    public async Task UpdateAsync(Domain.Entities.LanguageGroup entity, CancellationToken cancellationToken = default)
    {
        _context.LanguageGroups.Update(entity);
        await Task.CompletedTask;
    }
    public async Task DeleteAsync(Domain.Entities.LanguageGroup entity, CancellationToken cancellationToken = default)
    {
        _context.LanguageGroups.Remove(entity);
        await Task.CompletedTask;
    }
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.LanguageGroups.AnyAsync(g => g.Id == id, cancellationToken);
    }
}
