using AtlasBuho.Application.Translation;
using Microsoft.AspNetCore.Mvc;

namespace AtlasBuho.Controllers;

/// 6B.md §22: small, clean translator API contract. The controller never depends on an AI
/// vendor — only on ITranslationEngine (§3). States are explicit (§21), never a vague error.
[ApiController]
[Route("api/translate")]
public class TranslateController : ControllerBase
{
    private readonly ITranslationEngine? _engine;

    // 6B.md §0: the application must run with AI off / without a dataset configured.
    // When ITranslationEngine is not registered (no connection string), the endpoint still
    // answers with a first-class UnsupportedLanguage state rather than a 500.
    public TranslateController(ITranslationEngine? engine = null)
    {
        _engine = engine;
    }

    [HttpPost]
    public async Task<ActionResult<TranslationResult>> Post([FromBody] TranslationRequest? request, CancellationToken cancellationToken)
    {
        if (request == null || _engine == null)
        {
            return BadRequest(new TranslationResult(
                TranslationStatus.InvalidInput, request?.SourceLanguage ?? string.Empty,
                request?.TargetLanguage ?? string.Empty, request?.Text ?? string.Empty,
                null, null, null, string.Empty, _engine?.EngineVersion ?? "unavailable",
                Array.Empty<TranslationAlternative>()));
        }

        var result = await _engine.TranslateAsync(request, cancellationToken);

        return result.Status switch
        {
            TranslationStatus.InvalidInput => BadRequest(result),
            TranslationStatus.UnsupportedLanguage => UnprocessableEntity(result),
            _ => Ok(result) // Translated and NotFound are both valid linguistic states (§19)
        };
    }
}
