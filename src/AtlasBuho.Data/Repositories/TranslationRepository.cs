using Microsoft.EntityFrameworkCore;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Repositories;

public class TranslationRepository : Domain.Interfaces.ITranslationRepository
{
    private readonly AtlasBuhoDbContext _context;
    public TranslationRepository(AtlasBuhoDbContext context)
    {
        _context = context;
    }
    public async Task<Domain.Entities.Translation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Translations.FindAsync(new object[] { id }, cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.Translation>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Translations
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.Translation>> GetBySourceVariantAsync(Guid variantId, CancellationToken cancellationToken = default)
    {
        return await _context.Translations
            .Where(t => t.SourceLanguageVariantId == variantId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.Translation>> GetByTargetVariantAsync(Guid variantId, CancellationToken cancellationToken = default)
    {
        return await _context.Translations
            .Where(t => t.TargetLanguageVariantId == variantId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<Domain.Entities.Translation?> GetByTextsAsync(Guid sourceVariantId, Guid targetVariantId, string sourceText, CancellationToken cancellationToken = default)
    {
        return await _context.Translations
            .FirstOrDefaultAsync(t => t.SourceLanguageVariantId == sourceVariantId 
                && t.TargetLanguageVariantId == targetVariantId 
                && t.SourceText == sourceText, cancellationToken);
    }
    public async Task<Domain.Entities.Translation> AddAsync(Domain.Entities.Translation entity, CancellationToken cancellationToken = default)
    {
        await _context.Translations.AddAsync(entity, cancellationToken);
        return entity;
    }
    public async Task UpdateAsync(Domain.Entities.Translation entity, CancellationToken cancellationToken = default)
    {
        _context.Translations.Update(entity);
        await Task.CompletedTask;
    }
    public async Task DeleteAsync(Domain.Entities.Translation entity, CancellationToken cancellationToken = default)
    {
        _context.Translations.Remove(entity);
        await Task.CompletedTask;
    }
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Translations.AnyAsync(t => t.Id == id, cancellationToken);
    }
}
