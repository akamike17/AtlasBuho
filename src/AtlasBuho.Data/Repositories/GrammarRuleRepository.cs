using Microsoft.EntityFrameworkCore;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Repositories;

public class GrammarRuleRepository : Domain.Interfaces.IGrammarRuleRepository
{
    private readonly AtlasBuhoDbContext _context;
    public GrammarRuleRepository(AtlasBuhoDbContext context)
    {
        _context = context;
    }
    public async Task<Domain.Entities.GrammarRule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.GrammarRules.FindAsync(new object[] { id }, cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.GrammarRule>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.GrammarRules
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.GrammarRule>> GetByVariantAsync(Guid variantId, CancellationToken cancellationToken = default)
    {
        return await _context.GrammarRules
            .Where(g => g.LanguageVariantId == variantId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.GrammarRule>> GetByCategoryAsync(Guid variantId, string category, CancellationToken cancellationToken = default)
    {
        return await _context.GrammarRules
            .Where(g => g.LanguageVariantId == variantId && g.Category == category)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<Domain.Entities.GrammarRule> AddAsync(Domain.Entities.GrammarRule entity, CancellationToken cancellationToken = default)
    {
        await _context.GrammarRules.AddAsync(entity, cancellationToken);
        return entity;
    }
    public async Task UpdateAsync(Domain.Entities.GrammarRule entity, CancellationToken cancellationToken = default)
    {
        _context.GrammarRules.Update(entity);
        await Task.CompletedTask;
    }
    public async Task DeleteAsync(Domain.Entities.GrammarRule entity, CancellationToken cancellationToken = default)
    {
        _context.GrammarRules.Remove(entity);
        await Task.CompletedTask;
    }
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.GrammarRules.AnyAsync(g => g.Id == id, cancellationToken);
    }
}
