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
    // Two DIFFERENT states are kept separate (P1):
    //   request == null  -> 400 InvalidInput         (client sent a malformed request)
    //   _engine == null  -> 503 ServiceUnavailable    (server-side infrastructure/config not ready)
    public TranslateController(ITranslationEngine? engine = null)
    {
        _engine = engine;
    }

    [HttpPost]
    public async Task<ActionResult<TranslationResult>> Post([FromBody] TranslationRequest? request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return BadRequest(new TranslationResult(
                TranslationStatus.InvalidInput, string.Empty, string.Empty, string.Empty,
                null, null, null, string.Empty, _engine?.EngineVersion ?? "unavailable",
                Array.Empty<TranslationAlternative>()));
        }

        if (_engine == null)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new TranslationResult(
                TranslationStatus.ServiceUnavailable, request.SourceLanguage ?? string.Empty,
                request.TargetLanguage ?? string.Empty, request.Text ?? string.Empty,
                null, null, null, string.Empty, "unavailable",
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
