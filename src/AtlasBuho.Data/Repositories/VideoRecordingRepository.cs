using Microsoft.EntityFrameworkCore;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Repositories;

public class VideoRecordingRepository : Domain.Interfaces.IVideoRecordingRepository
{
    private readonly AtlasBuhoDbContext _context;
    public VideoRecordingRepository(AtlasBuhoDbContext context)
    {
        _context = context;
    }
    public async Task<Domain.Entities.VideoRecording?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.VideoRecordings.FindAsync(new object[] { id }, cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.VideoRecording>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.VideoRecordings
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.VideoRecording>> GetByVariantAsync(Guid variantId, CancellationToken cancellationToken = default)
    {
        return await _context.VideoRecordings
            .Where(v => v.LanguageVariantId == variantId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.VideoRecording>> GetBySpeakerAsync(Guid speakerId, CancellationToken cancellationToken = default)
    {
        return await _context.VideoRecordings
            .Where(v => v.SpeakerId == speakerId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<Domain.Entities.VideoRecording> AddAsync(Domain.Entities.VideoRecording entity, CancellationToken cancellationToken = default)
    {
        await _context.VideoRecordings.AddAsync(entity, cancellationToken);
        return entity;
    }
    public async Task UpdateAsync(Domain.Entities.VideoRecording entity, CancellationToken cancellationToken = default)
    {
        _context.VideoRecordings.Update(entity);
        await Task.CompletedTask;
    }
    public async Task DeleteAsync(Domain.Entities.VideoRecording entity, CancellationToken cancellationToken = default)
    {
        _context.VideoRecordings.Remove(entity);
        await Task.CompletedTask;
    }
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.VideoRecordings.AnyAsync(v => v.Id == id, cancellationToken);
    }
}
