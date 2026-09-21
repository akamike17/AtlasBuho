using Microsoft.EntityFrameworkCore;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Repositories;

public class DialectRelationshipRepository : Domain.Interfaces.IDialectRelationshipRepository
{
    private readonly AtlasBuhoDbContext _context;
    public DialectRelationshipRepository(AtlasBuhoDbContext context)
    {
        _context = context;
    }
    public async Task<Domain.Entities.DialectRelationship?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.DialectRelationships.FindAsync(new object[] { id }, cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.DialectRelationship>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DialectRelationships
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.DialectRelationship>> GetBySourceVariantAsync(Guid variantId, CancellationToken cancellationToken = default)
    {
        return await _context.DialectRelationships
            .Where(d => d.SourceLanguageVariantId == variantId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.DialectRelationship>> GetByTargetVariantAsync(Guid variantId, CancellationToken cancellationToken = default)
    {
        return await _context.DialectRelationships
            .Where(d => d.TargetLanguageVariantId == variantId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<Domain.Entities.DialectRelationship?> GetBetweenVariantsAsync(Guid sourceId, Guid targetId, CancellationToken cancellationToken = default)
    {
        return await _context.DialectRelationships
            .FirstOrDefaultAsync(d => d.SourceLanguageVariantId == sourceId && d.TargetLanguageVariantId == targetId, cancellationToken);
    }
    public async Task<Domain.Entities.DialectRelationship> AddAsync(Domain.Entities.DialectRelationship entity, CancellationToken cancellationToken = default)
    {
        await _context.DialectRelationships.AddAsync(entity, cancellationToken);
        return entity;
    }
    public async Task UpdateAsync(Domain.Entities.DialectRelationship entity, CancellationToken cancellationToken = default)
    {
        _context.DialectRelationships.Update(entity);
        await Task.CompletedTask;
    }
    public async Task DeleteAsync(Domain.Entities.DialectRelationship entity, CancellationToken cancellationToken = default)
    {
        _context.DialectRelationships.Remove(entity);
        await Task.CompletedTask;
    }
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.DialectRelationships.AnyAsync(d => d.Id == id, cancellationToken);
    }
}
