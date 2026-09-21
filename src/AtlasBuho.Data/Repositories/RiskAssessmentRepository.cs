using Microsoft.EntityFrameworkCore;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Repositories;

public class RiskAssessmentRepository : Domain.Interfaces.IRiskAssessmentRepository
{
    private readonly AtlasBuhoDbContext _context;
    public RiskAssessmentRepository(AtlasBuhoDbContext context)
    {
        _context = context;
    }
    public async Task<Domain.Entities.RiskAssessment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.RiskAssessments.FindAsync(new object[] { id }, cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.RiskAssessment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.RiskAssessments
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.RiskAssessment>> GetByVariantAsync(Guid variantId, CancellationToken cancellationToken = default)
    {
        return await _context.RiskAssessments
            .Where(r => r.LanguageVariantId == variantId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<Domain.Entities.RiskAssessment?> GetCurrentAsync(Guid variantId, CancellationToken cancellationToken = default)
    {
        return await _context.RiskAssessments
            .Where(r => r.LanguageVariantId == variantId && r.IsCurrent)
            .OrderByDescending(r => r.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.RiskAssessment>> GetByLevelAsync(Domain.Entities.RiskLevel level, CancellationToken cancellationToken = default)
    {
        return await _context.RiskAssessments
            .Where(r => r.IsCurrent && r.RiskLevel == level)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<Domain.Entities.RiskAssessment> AddAsync(Domain.Entities.RiskAssessment entity, CancellationToken cancellationToken = default)
    {
        await _context.RiskAssessments.AddAsync(entity, cancellationToken);
        return entity;
    }
    public async Task UpdateAsync(Domain.Entities.RiskAssessment entity, CancellationToken cancellationToken = default)
    {
        _context.RiskAssessments.Update(entity);
        await Task.CompletedTask;
    }
    public async Task DeleteAsync(Domain.Entities.RiskAssessment entity, CancellationToken cancellationToken = default)
    {
        _context.RiskAssessments.Remove(entity);
        await Task.CompletedTask;
    }
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.RiskAssessments.AnyAsync(r => r.Id == id, cancellationToken);
    }
}
