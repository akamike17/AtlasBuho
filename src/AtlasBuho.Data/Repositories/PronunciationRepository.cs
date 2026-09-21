using Microsoft.EntityFrameworkCore;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Repositories;

public class PronunciationRepository : Domain.Interfaces.IPronunciationRepository
{
    private readonly AtlasBuhoDbContext _context;
    public PronunciationRepository(AtlasBuhoDbContext context)
    {
        _context = context;
    }
    public async Task<Domain.Entities.Pronunciation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Pronunciations.FindAsync(new object[] { id }, cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.Pronunciation>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Pronunciations
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.Pronunciation>> GetByLexemeAsync(Guid lexemeId, CancellationToken cancellationToken = default)
    {
        return await _context.Pronunciations
            .Where(p => p.LexemeId == lexemeId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<Domain.Entities.Pronunciation?> GetPrimaryAsync(Guid lexemeId, CancellationToken cancellationToken = default)
    {
        return await _context.Pronunciations
            .Where(p => p.LexemeId == lexemeId && p.VerificationStatus == Domain.Entities.VerificationStatus.Verified)
            .OrderByDescending(p => p.Confidence)
            .FirstOrDefaultAsync(cancellationToken);
    }
    public async Task<Domain.Entities.Pronunciation> AddAsync(Domain.Entities.Pronunciation entity, CancellationToken cancellationToken = default)
    {
        await _context.Pronunciations.AddAsync(entity, cancellationToken);
        return entity;
    }
    public async Task UpdateAsync(Domain.Entities.Pronunciation entity, CancellationToken cancellationToken = default)
    {
        _context.Pronunciations.Update(entity);
        await Task.CompletedTask;
    }
    public async Task DeleteAsync(Domain.Entities.Pronunciation entity, CancellationToken cancellationToken = default)
    {
        _context.Pronunciations.Remove(entity);
        await Task.CompletedTask;
    }
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Pronunciations.AnyAsync(p => p.Id == id, cancellationToken);
    }
}
