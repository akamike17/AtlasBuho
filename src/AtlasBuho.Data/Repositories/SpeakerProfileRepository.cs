using Microsoft.EntityFrameworkCore;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Repositories;

public class SpeakerProfileRepository : Domain.Interfaces.ISpeakerProfileRepository
{
    private readonly AtlasBuhoDbContext _context;
    public SpeakerProfileRepository(AtlasBuhoDbContext context)
    {
        _context = context;
    }
    public async Task<Domain.Entities.SpeakerProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Speakers.FindAsync(new object[] { id }, cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.SpeakerProfile>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Speakers
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.SpeakerProfile>> GetByVariantAsync(Guid variantId, CancellationToken cancellationToken = default)
    {
        return await _context.Speakers
            .Where(s => s.LanguageVariantId == variantId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.SpeakerProfile>> GetByCommunityAsync(Guid communityId, CancellationToken cancellationToken = default)
    {
        return await _context.Speakers
            .Where(s => s.CommunityId == communityId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<Domain.Entities.SpeakerProfile> AddAsync(Domain.Entities.SpeakerProfile entity, CancellationToken cancellationToken = default)
    {
        await _context.Speakers.AddAsync(entity, cancellationToken);
        return entity;
    }
    public async Task UpdateAsync(Domain.Entities.SpeakerProfile entity, CancellationToken cancellationToken = default)
    {
        _context.Speakers.Update(entity);
        await Task.CompletedTask;
    }
    public async Task DeleteAsync(Domain.Entities.SpeakerProfile entity, CancellationToken cancellationToken = default)
    {
        _context.Speakers.Remove(entity);
        await Task.CompletedTask;
    }
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Speakers.AnyAsync(s => s.Id == id, cancellationToken);
    }
}
