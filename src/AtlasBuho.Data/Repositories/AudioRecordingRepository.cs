using Microsoft.EntityFrameworkCore;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Repositories;

public class AudioRecordingRepository : Domain.Interfaces.IAudioRecordingRepository
{
    private readonly AtlasBuhoDbContext _context;
    public AudioRecordingRepository(AtlasBuhoDbContext context)
    {
        _context = context;
    }
    public async Task<Domain.Entities.AudioRecording?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AudioRecordings.FindAsync(new object[] { id }, cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.AudioRecording>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AudioRecordings
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.AudioRecording>> GetByVariantAsync(Guid variantId, CancellationToken cancellationToken = default)
    {
        return await _context.AudioRecordings
            .Where(a => a.LanguageVariantId == variantId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.AudioRecording>> GetBySpeakerAsync(Guid speakerId, CancellationToken cancellationToken = default)
    {
        return await _context.AudioRecordings
            .Where(a => a.SpeakerId == speakerId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.AudioRecording>> GetByConsentStatusAsync(Domain.Entities.ConsentStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.AudioRecordings
            .Where(a => a.ConsentStatus == status)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.AudioRecording>> GetByLexemeAsync(Guid lexemeId, CancellationToken cancellationToken = default)
    {
        return await _context.AudioRecordings
            .Where(a => a.LexemeId == lexemeId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Domain.Entities.AudioRecording>> GetByPhraseAsync(Guid phraseId, CancellationToken cancellationToken = default)
    {
        return await _context.AudioRecordings
            .Where(a => a.PhraseId == phraseId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<Domain.Entities.AudioRecording> AddAsync(Domain.Entities.AudioRecording entity, CancellationToken cancellationToken = default)
    {
        await _context.AudioRecordings.AddAsync(entity, cancellationToken);
        return entity;
    }
    public async Task UpdateAsync(Domain.Entities.AudioRecording entity, CancellationToken cancellationToken = default)
    {
        _context.AudioRecordings.Update(entity);
        await Task.CompletedTask;
    }
    public async Task DeleteAsync(Domain.Entities.AudioRecording entity, CancellationToken cancellationToken = default)
    {
        _context.AudioRecordings.Remove(entity);
        await Task.CompletedTask;
    }
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AudioRecordings.AnyAsync(a => a.Id == id, cancellationToken);
    }
}
