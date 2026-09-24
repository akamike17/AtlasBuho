namespace AtlasBuho.Application.Translation;

/// 6B.md §3: AI must be an adapter, not the foundation. The translator core depends on
/// ITranslationEngine; EngineVersion is part of the reproduction contract (§38).
public interface ITranslationEngine
{
    string EngineVersion { get; }
    Task<TranslationResult> TranslateAsync(TranslationRequest request, CancellationToken cancellationToken = default);
}
