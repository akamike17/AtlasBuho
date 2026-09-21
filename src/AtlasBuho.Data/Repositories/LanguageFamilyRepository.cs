using Microsoft.EntityFrameworkCore;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Repositories;

public class LanguageFamilyRepository : Domain.Interfaces.ILanguageFamilyRepository
{
    private readonly AtlasBuhoDbContext _context;
    public LanguageFamilyRepository(AtlasBuhoDbContext context)
    {
        _context = context;
    }
    public async Task<Domain.Entities.LanguageFamily?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.LanguageFamilies.FindAsync(new object[] { id }, cancellationToken);
    }
    public async Task<Domain.Entities.LanguageFamily?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.LanguageFamilies
            .FirstOrDefaultAsync(f => f.Name == name, cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.LanguageFamily>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.LanguageFamilies
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.LanguageFamily>> GetWithGroupsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.LanguageFamilies
            .Include(f => f.LanguageGroups)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<Domain.Entities.LanguageFamily> AddAsync(Domain.Entities.LanguageFamily entity, CancellationToken cancellationToken = default)
    {
        await _context.LanguageFamilies.AddAsync(entity, cancellationToken);
        return entity;
    }
    public async Task UpdateAsync(Domain.Entities.LanguageFamily entity, CancellationToken cancellationToken = default)
    {
        _context.LanguageFamilies.Update(entity);
        await Task.CompletedTask;
    }
    public async Task DeleteAsync(Domain.Entities.LanguageFamily entity, CancellationToken cancellationToken = default)
    {
        _context.LanguageFamilies.Remove(entity);
        await Task.CompletedTask;
    }
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.LanguageFamilies.AnyAsync(f => f.Id == id, cancellationToken);
    }
}
