using AtlasBuho.Application.Translation;
using AtlasBuho.Data;
using AtlasBuho.Data.Translation;
using AtlasBuho.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace AtlasBuho.Tests;

/// <summary>
/// 6B.md §45/§46 — DictionaryTranslationEngine contract tests.
///
/// Pins: verified-only results (§4), directional translation (§14), no silent winner among
/// alternatives (§6), first-class NotFound instead of invented output (§19), deterministic
/// ordering/selection (§5), and dataset version reporting (§37).
/// </summary>
public class DictionaryTranslationEngineTests
{
    private const string Dataset = "dataset-v1";

    private static AtlasBuhoDbContext NewContext(string name)
        => new(new DbContextOptionsBuilder<AtlasBuhoDbContext>()
            .UseInMemoryDatabase(name)
            .Options);

    private static Lexeme Lex(Guid variantId, string form, string? spanish, VerificationStatus status)
        => new(variantId, form, alternativeForms: null, autodenomination: null, spanishMeaning: spanish,
               partOfSpeech: null, pronunciationIpa: null, pronunciationReadable: null, semanticDomain: null,
               register: null, regionalNotes: null, etymology: null, source: null, verificationStatus: status);

    private static async Task<(AtlasBuhoDbContext Ctx, LanguageVariant Variant)> SeededContextAsync(
        Action<AtlasBuhoDbContext, LanguageVariant>? seed = null)
    {
        var ctx = NewContext(Guid.NewGuid().ToString());
        var family = new LanguageFamily("oto-mangue", null, null, "test");
        var group = new LanguageGroup(family.Id, "zapoteco", null, null, "test");
        var variant = new LanguageVariant(group.Id, "zapoteco serrano", null, null, null, null, null, null, null);
        ctx.LanguageFamilies.Add(family);
        ctx.LanguageGroups.Add(group);
        ctx.LanguageVariants.Add(variant);
        ctx.CatalogVersions.Add(new CatalogVersion(
            Guid.NewGuid(), Dataset, "hash", DateTime.UtcNow, 11, 68, 364, 474, "test", "Completed"));
        seed?.Invoke(ctx, variant);
        await ctx.SaveChangesAsync();
        return (ctx, variant);
    }

    // ------------------------------------------------------------- §19 unknown state

    [Fact]
    public async Task Translate_UnknownWord_ReturnsNotFound_NotInvented()
    {
        var (ctx, _) = await SeededContextAsync();
        await using (ctx)
        {
            var result = await new DictionaryTranslationEngine(ctx)
                .TranslateAsync(new TranslationRequest("zapoteco serrano", "español", "palabra_inexistente"));

            result.Status.Should().Be(TranslationStatus.NotFound,
                "a missing translation remains missing (6B.md §19, Invariant 6)");
            result.Translation.Should().BeNull();
            result.DatasetVersion.Should().Be(Dataset);
            result.EngineVersion.Should().Be("dictionary-1.0");
        }
    }

    // ------------------------------------------------------------- §4 verified-only

    [Fact]
    public async Task Translate_IndigenousToSpanish_UniqueVerifiedMatch_Resolves()
    {
        var (ctx, _) = await SeededContextAsync((c, v) =>
        {
            c.Lexemes.Add(Lex(v.Id, "bene xono", "fox", VerificationStatus.Verified));
            c.Lexemes.Add(Lex(v.Id, "unverificado", "zorro", VerificationStatus.Unknown));
        });
        await using (ctx)
        {
            var result = await new DictionaryTranslationEngine(ctx)
                .TranslateAsync(new TranslationRequest("zapoteco serrano", "español", "bene xono"));

            result.Status.Should().Be(TranslationStatus.Translated);
            result.Translation.Should().Be("fox");
            result.MatchType.Should().Be("exact_lexical");
            result.VerificationStatus.Should().Be("Verified");
        }
    }

    [Fact]
    public async Task Translate_UnverifiedLexeme_IsNeverTranslated()
    {
        var (ctx, _) = await SeededContextAsync((c, v) =>
            c.Lexemes.Add(Lex(v.Id, "dichsah", "nombre", VerificationStatus.Unknown)));
        await using (ctx)
        {
            var result = await new DictionaryTranslationEngine(ctx)
                .TranslateAsync(new TranslationRequest("zapoteco serrano", "español", "dichsah"));

            // Unverified data is not a verified translation: the unknown state, not a guess.
            result.Status.Should().Be(TranslationStatus.NotFound,
                "unverified lexemes must not surface as translations (6B.md §4 tier 1 requires verified data)");
        }
    }

    // ------------------------------------------------------------- §6 no silent winner

    [Fact]
    public async Task Translate_MultipleVerifiedMatches_ReturnsDeterministicAlternatives()
    {
        var (ctx, _) = await SeededContextAsync((c, v) =>
        {
            c.Lexemes.Add(Lex(v.Id, "ra", "zzz", VerificationStatus.Verified));
            c.Lexemes.Add(Lex(v.Id, "ra", "aaa", VerificationStatus.Verified));
        });
        await using (ctx)
        {
            var engine = new DictionaryTranslationEngine(ctx);
            var first = await engine.TranslateAsync(new TranslationRequest("zapoteco serrano", "español", "ra"));
            var second = await engine.TranslateAsync(new TranslationRequest("zapoteco serrano", "español", "ra"));

            first.Status.Should().Be(TranslationStatus.Translated);
            first.Alternatives.Should().HaveCount(2,
                "multiple verified translations are exposed, never hidden (6B.md §6)");
            first.Alternatives.Select(a => a.Text).Should().Equal(new[] { "aaa", "zzz" },
                "ordering is deterministic, not random nor database-order");
            first.Translation.Should().Be(second.Translation,
                "same dataset+engine+input → same primary result (6B.md §5, Invariant 10)");
            first.Alternatives.Select(a => a.Text).Should().Equal(
                second.Alternatives.Select(a => a.Text),
                "same dataset+engine+input → same alternatives in the same order (6B.md §5, Invariant 10)");
        }
    }

    // ------------------------------------------------------------- §14 directionality

    [Fact]
    public async Task Translate_SpanishToIndigenous_UsesOnlySpanishEvidence_NotReverseGuess()
    {
        var (ctx, _) = await SeededContextAsync((c, v) =>
            c.Lexemes.Add(Lex(v.Id, "bene xono", "fox", VerificationStatus.Verified)));
        await using (ctx)
        {
            // Only SpanishMeaning="fox" exists for source lexeme "bene xono"; the Spanish input
            // "bene xono" has NO Spanish-lexeme evidence, so the reverse must NOT reuse the forward.
            var reverse = await new DictionaryTranslationEngine(ctx)
                .TranslateAsync(new TranslationRequest("español", "zapoteco serrano", "bene xono"));

            reverse.Status.Should().Be(TranslationStatus.NotFound,
                "A→B does not imply B→A without evidence (6B.md §14)");
        }
    }

    [Fact]
    public async Task Translate_SpanishToIndigenous_WithSpanishEvidence_Resolves()
    {
        var (ctx, _) = await SeededContextAsync((c, v) =>
            c.Lexemes.Add(Lex(v.Id, "bene xono", "zorro", VerificationStatus.Verified)));
        await using (ctx)
        {
            var result = await new DictionaryTranslationEngine(ctx)
                .TranslateAsync(new TranslationRequest("español", "zapoteco serrano", "zorro"));

            result.Status.Should().Be(TranslationStatus.Translated);
            result.Translation.Should().Be("bene xono");
        }
    }

    // ------------------------------------------------------------- result contract

    [Fact]
    public async Task Translate_EmptyInput_IsInvalidInput()
    {
        var (ctx, _) = await SeededContextAsync();
        await using (ctx)
        {
            var result = await new DictionaryTranslationEngine(ctx)
                .TranslateAsync(new TranslationRequest("zapoteco", "español", ""));

            result.Status.Should().Be(TranslationStatus.InvalidInput);
        }
    }

    [Fact]
    public async Task Translate_UnknownSourceLanguage_IsUnsupportedLanguage()
    {
        var (ctx, _) = await SeededContextAsync();
        await using (ctx)
        {
            var result = await new DictionaryTranslationEngine(ctx)
                .TranslateAsync(new TranslationRequest("klingon", "español", "hola"));

            result.Status.Should().Be(TranslationStatus.UnsupportedLanguage);
        }
    }
}
