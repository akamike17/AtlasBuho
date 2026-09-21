using Microsoft.EntityFrameworkCore;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Repositories;

public class ConsentRecordRepository : Domain.Interfaces.IConsentRecordRepository
{
    private readonly AtlasBuhoDbContext _context;
    public ConsentRecordRepository(AtlasBuhoDbContext context)
    {
        _context = context;
    }
    public async Task<Domain.Entities.ConsentRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ConsentRecords.FindAsync(new object[] { id }, cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.ConsentRecord>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ConsentRecords
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.ConsentRecord>> GetBySpeakerAsync(Guid speakerId, CancellationToken cancellationToken = default)
    {
        return await _context.ConsentRecords
            .Where(c => c.SpeakerId == speakerId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.ConsentRecord>> GetByRecordingAsync(Guid recordingId, CancellationToken cancellationToken = default)
    {
        return await _context.ConsentRecords
            .Where(c => c.AudioRecordingId == recordingId || c.VideoRecordingId == recordingId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.ConsentRecord>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ConsentRecords
            .Where(c => !c.IsRevoked)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<Domain.Entities.ConsentRecord> AddAsync(Domain.Entities.ConsentRecord entity, CancellationToken cancellationToken = default)
    {
        await _context.ConsentRecords.AddAsync(entity, cancellationToken);
        return entity;
    }
    public async Task UpdateAsync(Domain.Entities.ConsentRecord entity, CancellationToken cancellationToken = default)
    {
        _context.ConsentRecords.Update(entity);
        await Task.CompletedTask;
    }
    public async Task DeleteAsync(Domain.Entities.ConsentRecord entity, CancellationToken cancellationToken = default)
    {
        _context.ConsentRecords.Remove(entity);
        await Task.CompletedTask;
    }
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ConsentRecords.AnyAsync(c => c.Id == id, cancellationToken);
    }
}
