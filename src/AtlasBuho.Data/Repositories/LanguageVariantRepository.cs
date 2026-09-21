using Microsoft.EntityFrameworkCore;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Repositories;

public class LanguageVariantRepository : Domain.Interfaces.ILanguageVariantRepository
{
    private readonly AtlasBuhoDbContext _context;
    public LanguageVariantRepository(AtlasBuhoDbContext context)
    {
        _context = context;
    }
    public async Task<Domain.Entities.LanguageVariant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.LanguageVariants.FindAsync(new object[] { id }, cancellationToken);
    }
    public async Task<Domain.Entities.LanguageVariant?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.LanguageVariants
            .Include(v => v.Communities)
            .Include(v => v.Lexemes)
            .Include(v => v.Speakers)
            .Include(v => v.RiskAssessments.Where(r => r.IsCurrent))
            .Include(v => v.DialectRelationships)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.LanguageVariant>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.LanguageVariants
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.LanguageVariant>> GetByGroupAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        return await _context.LanguageVariants
            .Where(v => v.LanguageGroupId == groupId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.LanguageVariant>> GetByRiskLevelAsync(Domain.Entities.RiskLevel level, CancellationToken cancellationToken = default)
    {
        return await _context.LanguageVariants
            .Where(v => v.RiskAssessments.Any(r => r.IsCurrent && r.RiskLevel == level))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<Domain.Entities.LanguageVariant?> GetByInaliCodeAsync(string inaliCode, CancellationToken cancellationToken = default)
    {
        return await _context.LanguageVariants
            .FirstOrDefaultAsync(v => v.InaliCode == inaliCode, cancellationToken);
    }
    public async Task<Domain.Entities.LanguageVariant?> GetByIsoCodeAsync(string isoCode, CancellationToken cancellationToken = default)
    {
        return await _context.LanguageVariants
            .FirstOrDefaultAsync(v => v.Iso639_3Code == isoCode, cancellationToken);
    }
    public async Task<Domain.Entities.LanguageVariant> AddAsync(Domain.Entities.LanguageVariant entity, CancellationToken cancellationToken = default)
    {
        await _context.LanguageVariants.AddAsync(entity, cancellationToken);
        return entity;
    }
    public async Task UpdateAsync(Domain.Entities.LanguageVariant entity, CancellationToken cancellationToken = default)
    {
        _context.LanguageVariants.Update(entity);
        await Task.CompletedTask;
    }
    public async Task DeleteAsync(Domain.Entities.LanguageVariant entity, CancellationToken cancellationToken = default)
    {
        _context.LanguageVariants.Remove(entity);
        await Task.CompletedTask;
    }
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.LanguageVariants.AnyAsync(v => v.Id == id, cancellationToken);
    }
}
