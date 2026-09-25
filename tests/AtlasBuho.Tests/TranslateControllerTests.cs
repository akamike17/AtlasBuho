using AtlasBuho.Application.Translation;
using AtlasBuho.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace AtlasBuho.Tests;

/// <summary>
/// 6B.md §21/§22 + P1 hardening — the API must keep CLIENT malformed input separate from
/// SERVER infrastructure/configuration state:
///   request == null  -> 400 InvalidInput
///   engine == null   -> 503 ServiceUnavailable (a first-class TranslationStatus, not
///                       a linguistic UnsupportedLanguage masquerading as the problem)
/// </summary>
public class TranslateControllerTests
{
    private sealed class FakeEngine : ITranslationEngine
    {
        public string EngineVersion => "fake-1.0";
        public Task<TranslationResult> TranslateAsync(TranslationRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(TranslationResult.NotFound("x", "y", request.Text, "ds", EngineVersion));
    }

    [Fact]
    public async Task Post_NullRequest_Returns400_InvalidInput()
    {
        var controller = new TranslateController(new FakeEngine());

        var action = await controller.Post(null, CancellationToken.None);

        var bad = action.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        bad.StatusCode.Should().Be(400);
        bad.Value.Should().BeOfType<TranslationResult>()
            .Subject.Status.Should().Be(TranslationStatus.InvalidInput);
    }

    [Fact]
    public async Task Post_NullEngine_Returns503_ServiceUnavailable_NotUnsupportedLanguage()
    {
        // engine not configured (no connection string at startup) is INFRA, not linguistics.
        var controller = new TranslateController(engine: null);

        var action = await controller.Post(cancellationToken: CancellationToken.None, request: new TranslationRequest("zapoteco", "español", "ra"));

        var obj = action.Result.Should().BeOfType<ObjectResult>().Subject;
        obj.StatusCode.Should().Be(503);
        var body = obj.Value.Should().BeOfType<TranslationResult>().Subject;
        body.Status.Should().Be(TranslationStatus.ServiceUnavailable,
            "HTTP 503 body must not claim a linguistic UnsupportedLanguage state");
        body.EngineVersion.Should().Be("unavailable");
    }

    [Fact]
    public async Task Post_ValidRequest_WithEngine_Returns200()
    {
        var controller = new TranslateController(new FakeEngine());

        var action = await controller.Post(cancellationToken: CancellationToken.None, request: new TranslationRequest("zapoteco", "español", "ghostword"));

        var ok = action.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<TranslationResult>().Subject.Status.Should().Be(TranslationStatus.NotFound);
    }
}
