using AtlasBuho.Application.Translation;
using AtlasBuho.Data;
using AtlasBuho.Data.Translation;
using AtlasBuho.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace AtlasBuho.Tests;

/// <summary>
/// ADR 0001 §5 + 6B.md §45/§46 — golden tests for the LexicalEquivalence-based
/// dictionary-2.0 contract.
///
/// Asserts the demanded invariants:
///   (1) same lexical form in different variants NEVER cross-pollinates (I4/P0)
///   (2) Indigenous→Spanish via explicit equivalence evidence (I2)
///   (3) Indigenous→English via explicit equivalence evidence (I2)
///   (4) explicit reverse-direction evidence resolves
///   (5) absence of reverse evidence returns NotFound (NOT the forward's inverse)
///   (6) duplicate target text from two different sources is preserved as two alternatives
///   (7) multiple target translations are exposed deterministically, no silent winner
///   (8) exactly one canonical row → canonical determines Translation
///   (9) zero canonical rows → Translation is null
///   (10) TWO canonical rows → integrity violation, never tie-break by ordering (I1)
///   (11) fixed CatalogVersionId reproducibility
/// </summary>
public class LexicalEquivalenceEngineTests
{
    private static AtlasBuhoDbContext NewContext(string name)
        => new(new DbContextOptionsBuilder<AtlasBuhoDbContext>()
            .UseInMemoryDatabase(name)
            .Options);

    private sealed class Fixture
    {
        public AtlasBuhoDbContext Ctx = null!;
        public CatalogVersion Version = null!;
        public Source SourceA = null!;
        public LanguageVariant VA = null!;
        public LanguageVariant VB = null!;
        public Lexeme LexA = null!;
        public Lexeme LexB = null!;
    }

    private static async Task<Fixture> SeedAsync(Action<Fixture>? configure = null)
    {
        var ctx = NewContext(Guid.NewGuid().ToString());
        var family = new LanguageFamily("oto-mangue", null, null, "test");
        var groupA = new LanguageGroup(family.Id, "zapoteco", null, null, "test");
        var groupB = new LanguageGroup(family.Id, "mixteco", null, null, "test");
        var vA = new LanguageVariant(groupA.Id, "zapoteco serrano", null, null, null, null, null, null, null);
        var vB = new LanguageVariant(groupB.Id, "mixteco de Guerrero", null, null, null, null, null, null, null);
        var lexA = new Lexeme(vA.Id, "ra", null, null, null, null, null, null, null, null, null, null, null, VerificationStatus.Unknown);
        var lexB = new Lexeme(vB.Id, "ra", null, null, null, null, null, null, null, null, null, null, null, VerificationStatus.Unknown);
        var sourceA = new Source("Fuente A", SourceLevel.Institutional, null, null, null, null, null, null, null, null, null, null, null, null, null);
        var version = new CatalogVersion(Guid.NewGuid(), "v1", "c-hash", DateTime.UtcNow, 0, 0, 0, 0, "engine-2", "Completed");

        ctx.LanguageFamilies.Add(family);
        ctx.LanguageGroups.Add(groupA);
        ctx.LanguageGroups.Add(groupB);
        ctx.LanguageVariants.Add(vA);
        ctx.LanguageVariants.Add(vB);
        ctx.Lexemes.Add(lexA);
        ctx.Lexemes.Add(lexB);
        ctx.Sources.Add(sourceA);
        ctx.CatalogVersions.Add(version);

        var fixture = new Fixture { Ctx = ctx, Version = version, SourceA = sourceA, VA = vA, VB = vB, LexA = lexA, LexB = lexB };
        configure?.Invoke(fixture);
        await ctx.SaveChangesAsync();
        return fixture;
    }

    private static LexicalEquivalence Eq(Lexeme lex, string lang, string text, bool canonical, VerificationStatus st, Source src, CatalogVersion ver)
        => new(lex.Id, lang, text, canonical, st, src.Id, ver.Id);

    // ---------------------------------------------------------------- (1) I4/P0

    [Fact]
    public async Task SameFormInTwoVariants_DoesNotCrossVariants()
    {
        var f = await SeedAsync(fx =>
        {
            fx.Ctx.LexicalEquivalences.Add(Eq(fx.LexA, "es", "sol", true, VerificationStatus.Verified, fx.SourceA, fx.Version));
            fx.Ctx.LexicalEquivalences.Add(Eq(fx.LexB, "es", "estrella", true, VerificationStatus.Verified, fx.SourceA, fx.Version));
        });
        await using (f.Ctx)
        {
            var engine = new DictionaryTranslationEngine(f.Ctx);

            var fromA = await engine.TranslateAsync(new TranslationRequest(f.VA.Name, "español", "ra"));
            fromA.Translation.Should().Be("sol");
            fromA.Alternatives.Should().ContainSingle(a => a.Text == "sol");
            fromA.Alternatives.Should().NotContain(a => a.Text == "estrella");

            var fromB = await engine.TranslateAsync(new TranslationRequest(f.VB.Name, "español", "ra"));
            fromB.Translation.Should().Be("estrella");
        }
    }

    // ---------------------------------------------------------------- (2)+(3) I2 directions

    [Fact]
    public async Task IndigenousToSpanish_ViaExplicitEquivalence()
    {
        var f = await SeedAsync(fx =>
            fx.Ctx.LexicalEquivalences.Add(Eq(fx.LexA, "es", "sol", true, VerificationStatus.Verified, fx.SourceA, fx.Version)));
        await using (f.Ctx)
        {
            var engine = new DictionaryTranslationEngine(f.Ctx);
            var result = await engine.TranslateAsync(new TranslationRequest(f.VA.Name, "español", "ra"));

            result.Status.Should().Be(TranslationStatus.Translated);
            result.Translation.Should().Be("sol");
            result.MatchType.Should().Be("canonical_evidence");
        }
    }

    [Fact]
    public async Task IndigenousToEnglish_ViaExplicitEquivalence()
    {
        var f = await SeedAsync(fx =>
            fx.Ctx.LexicalEquivalences.Add(Eq(fx.LexA, "en", "sun", true, VerificationStatus.Verified, fx.SourceA, fx.Version)));
        await using (f.Ctx)
        {
            var engine = new DictionaryTranslationEngine(f.Ctx);
            var result = await engine.TranslateAsync(new TranslationRequest(f.VA.Name, "english", "ra"));

            result.Status.Should().Be(TranslationStatus.Translated);
            result.Translation.Should().Be("sun");
        }
    }

    // ---------------------------------------------------------------- (4)+(5) reverse direction

    [Fact]
    public async Task ReverseDirection_WithOwnEvidence_Resolves()
    {
        var f = await SeedAsync(fx =>
        {
            // The indigenous lexeme records its own Spanish gloss; the EVIDENCE row for the
            // es/en direction asserts the reverse equivalence explicitly.
            var lexeme = fx.Ctx.Lexemes.Local.Single(l => l.Id == fx.LexA.Id);
            lexeme.Update(
                canonicalForm: lexeme.CanonicalForm,
                alternativeForms: lexeme.AlternativeForms,
                autodenomination: lexeme.Autodenomination,
                spanishMeaning: "sol",
                partOfSpeech: lexeme.PartOfSpeech,
                pronunciationIpa: lexeme.PronunciationIpa,
                pronunciationReadable: lexeme.PronunciationReadable,
                semanticDomain: lexeme.SemanticDomain,
                register: lexeme.Register,
                regionalNotes: lexeme.RegionalNotes,
                etymology: lexeme.Etymology,
                source: lexeme.Source,
                verificationStatus: lexeme.VerificationStatus,
                confidence: lexeme.Confidence);
            fx.Ctx.LexicalEquivalences.Add(Eq(fx.LexA, "es", "sol", true, VerificationStatus.Verified, fx.SourceA, fx.Version));
        });
        await using (f.Ctx)
        {
            var engine = new DictionaryTranslationEngine(f.Ctx);
            var result = await engine.TranslateAsync(new TranslationRequest("español", f.VA.Name, "sol"));

            result.Status.Should().Be(TranslationStatus.Translated);
            result.Translation.Should().Be("ra",
                "explicit reverse-direction evidence resolves to the canonical form (ADR 0001 I2)");
        }
    }

    [Fact]
    public async Task ReverseDirection_WithoutEvidence_IsNotFound_NotInverted()
    {
        var f = await SeedAsync();
        await using (f.Ctx)
        {
            var engine = new DictionaryTranslationEngine(f.Ctx);
            var result = await engine.TranslateAsync(new TranslationRequest("español", f.VA.Name, "ghostword"));

            result.Status.Should().Be(TranslationStatus.NotFound,
                "without reverse-direction evidence the engine returns nothing (ADR 0001 I2)");
            result.Translation.Should().BeNull();
        }
    }

    // ---------------------------------------------------------------- (6) provenance preserved

    [Fact]
    public async Task TwoSourcesSameTargetText_DoNotCollapseProvenance()
    {
        var f = await SeedAsync(fx =>
        {
            var sourceB = new Source("Fuente B", SourceLevel.LinguisticArchive, null, null, null, null, null, null, null, null, null, null, null, null, null);
            fx.Ctx.Sources.Add(sourceB);
            fx.Ctx.SaveChanges();
            fx.Ctx.LexicalEquivalences.Add(Eq(fx.LexA, "es", "sol", true, VerificationStatus.Verified, fx.SourceA, fx.Version));
            fx.Ctx.LexicalEquivalences.Add(Eq(fx.LexA, "es", "sol", false, VerificationStatus.Documented, sourceB, fx.Version));
        });
        await using (f.Ctx)
        {
            var engine = new DictionaryTranslationEngine(f.Ctx);
            var result = await engine.TranslateAsync(new TranslationRequest(f.VA.Name, "español", "ra"));

            result.Translation.Should().Be("sol");
            // Two distinct evidence rows with the SAME TargetText must survive (no Distinct()).
            f.Ctx.LexicalEquivalences.Local.Count(e => e.SourceLexemeId == f.LexA.Id && e.TargetLanguage == "es")
                .Should().Be(2, "provenance is never collapsed (ADR 0001 §2)");
            result.Alternatives.Should().HaveCount(2);
        }
    }

    // ---------------------------------------------------------------- (7) alternatives

    [Fact]
    public async Task MultipleTargets_AllReturnedDeterministically_NoSilentWinner()
    {
        var f = await SeedAsync(fx =>
        {
            fx.Ctx.LexicalEquivalences.Add(Eq(fx.LexA, "es", "zeta", false, VerificationStatus.Verified, fx.SourceA, fx.Version));
            fx.Ctx.LexicalEquivalences.Add(Eq(fx.LexA, "es", "alfa", false, VerificationStatus.Verified, fx.SourceA, fx.Version));
        });
        await using (f.Ctx)
        {
            var engine = new DictionaryTranslationEngine(f.Ctx);
            var first = await engine.TranslateAsync(new TranslationRequest(f.VA.Name, "español", "ra"));
            var second = await engine.TranslateAsync(new TranslationRequest(f.VA.Name, "español", "ra"));

            first.Status.Should().Be(TranslationStatus.NotFound, "no canonical row => no primary Translation");
            first.Translation.Should().BeNull();
            first.Alternatives.Should().HaveCount(2);
            first.Alternatives.Select(a => a.Text).Should().Equal(second.Alternatives.Select(a => a.Text),
                "ordering is deterministic, never arbitrary");
        }
    }

    // ---------------------------------------------------------------- (8)(9)(10) I1 canonical

    [Fact]
    public async Task ExactlyOneCanonical_DeterminesTranslation()
    {
        var f = await SeedAsync(fx =>
        {
            fx.Ctx.LexicalEquivalences.Add(Eq(fx.LexA, "es", "estrella", false, VerificationStatus.Verified, fx.SourceA, fx.Version));
            fx.Ctx.LexicalEquivalences.Add(Eq(fx.LexA, "es", "sol", true, VerificationStatus.Verified, fx.SourceA, fx.Version));
        });
        await using (f.Ctx)
        {
            var engine = new DictionaryTranslationEngine(f.Ctx);
            var result = await engine.TranslateAsync(new TranslationRequest(f.VA.Name, "español", "ra"));

            result.Status.Should().Be(TranslationStatus.Translated);
            result.Translation.Should().Be("sol");
            result.MatchType.Should().Be("canonical_evidence");
        }
    }

    [Fact]
    public async Task ZeroCanonical_ZeroTranslation_OnlyAlternatives()
    {
        var f = await SeedAsync(fx =>
            fx.Ctx.LexicalEquivalences.Add(Eq(fx.LexA, "es", "sol", false, VerificationStatus.Verified, fx.SourceA, fx.Version)));
        await using (f.Ctx)
        {
            var engine = new DictionaryTranslationEngine(f.Ctx);
            var result = await engine.TranslateAsync(new TranslationRequest(f.VA.Name, "español", "ra"));

            result.Status.Should().Be(TranslationStatus.NotFound);
            result.Translation.Should().BeNull();
            result.Alternatives.Should().ContainSingle(a => a.Text == "sol");
        }
    }

    [Fact]
    public async Task TwoCanonical_ThrowsIntegrityViolation_NeverAlphabeticalPick()
    {
        var f = await SeedAsync(fx =>
        {
            var sourceB = new Source("Fuente B", SourceLevel.LinguisticArchive, null, null, null, null, null, null, null, null, null, null, null, null, null);
            fx.Ctx.Sources.Add(sourceB);
            fx.Ctx.SaveChanges();
            fx.Ctx.LexicalEquivalences.Add(Eq(fx.LexA, "es", "alfa", true, VerificationStatus.Verified, fx.SourceA, fx.Version));
            fx.Ctx.LexicalEquivalences.Add(Eq(fx.LexA, "es", "zeta", true, VerificationStatus.Verified, sourceB, fx.Version));
        });
        await using (f.Ctx)
        {
            var engine = new DictionaryTranslationEngine(f.Ctx);
            var act = () => engine.TranslateAsync(new TranslationRequest(f.VA.Name, "español", "ra"));

            await act.Should().ThrowAsync<InvalidOperationException>(
                "two canonical rows for the same (lexeme, target language, catalog version) violate ADR 0001 I1");
        }
    }

    // ---------------------------------------------------------------- (11) version reproducibility

    [Fact]
    public async Task FixedCatalogVersion_YieldsReproducibleResults_AcrossQueries()
    {
        var f = await SeedAsync(fx =>
            fx.Ctx.LexicalEquivalences.Add(Eq(fx.LexA, "es", "sol", true, VerificationStatus.Verified, fx.SourceA, fx.Version)));
        await using (f.Ctx)
        {
            var engine = new DictionaryTranslationEngine(f.Ctx);
            var a = await engine.TranslateAsync(new TranslationRequest(f.VA.Name, "español", "ra"));
            var b = await engine.TranslateAsync(new TranslationRequest(f.VA.Name, "español", "ra"));

            a.DatasetVersion.Should().Be(b.DatasetVersion);
            a.Translation.Should().Be(b.Translation);
            a.Alternatives.Select(x => x.Text).Should().Equal(b.Alternatives.Select(x => x.Text));
        }
    }

    [Fact]
    public async Task DatasetVersion_IsTheCompletedCatalogVersion()
    {
        var f = await SeedAsync(fx =>
            fx.Ctx.LexicalEquivalences.Add(Eq(fx.LexA, "es", "sol", true, VerificationStatus.Verified, fx.SourceA, fx.Version)));
        await using (f.Ctx)
        {
            var engine = new DictionaryTranslationEngine(f.Ctx);
            var result = await engine.TranslateAsync(new TranslationRequest(f.VA.Name, "español", "ra"));

            result.DatasetVersion.Should().Be(f.Version.VersionNumber,
                "the engine binds to the Completed CatalogVersion (ADR 0001 I3)");
        }
    }
}
