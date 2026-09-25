using AtlasBuho.Application.Translation;
using AtlasBuho.Domain.AiReview;
using Microsoft.EntityFrameworkCore;

namespace AtlasBuho.Data.Translation;

/// <summary>
/// EF Core implementation of AI orchestration persistence (B10 §19).
/// Bridges Application layer abstraction to concrete DbContext.
/// </summary>
public sealed class AiOrchestrationPersistence : IAiOrchestrationPersistence
{
    private readonly AtlasBuhoDbContext _context;

    public AiOrchestrationPersistence(AtlasBuhoDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task AddAiReviewAsync(AiTranslationReview review, CancellationToken ct = default)
    {
        await _context.AiTranslationReviews.AddAsync(review, ct);
    }

    public async Task AddV2CandidateAsync(V2Candidate candidate, CancellationToken ct = default)
    {
        await _context.V2Candidates.AddAsync(candidate, ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }
}