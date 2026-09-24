using AtlasBuho.Data;
using AtlasBuho.Data.Seeding;
using AtlasBuho.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;

namespace AtlasBuho.Tests;

/// <summary>
/// 5B.md FASE 14 — autodenomination counting integrity / 474-row exact reconciliation.
///
/// These tests pin the two properties that the previous implementation violated:
///   1. an ambiguous candidate set is NEVER resolved by picking one candidate (H-103), and
///   2. every documentary Appendix 4 source row produces exactly ONE terminal outcome,
///      so Imported + Quarantined == 474.
/// </summary>
public class InaliAutodenominationReconciliationTests
{
    private const int DocumentaryValidAutodenominations = 474;

    private static LanguageVariant Variant(string name)
        => new(Guid.NewGuid(), name, null, null, null, null, null, null, null);

    private static InaliCatalogImporter.Appendix4Dto Row(
        string autodenom,
        string spanishName,
        string agrupacion = "zapoteco",
        string familia = "oto-mangue",
        int page = 246,
        int ordinal = 0)
        => new()
        {
            Page = page,
            Autodenom = autodenom,
            SpanishName = spanishName,
            Agrupacion = agrupacion,
            Familia = familia,
            SourceOrdinal = ordinal
        };

    // ---------------------------------------------------------------- matching

    [Fact]
    public void Resolve_UniqueExactSpanishName_Resolves()
    {
        var variants = new List<LanguageVariant> { Variant("chontal de Tabasco del norte"), Variant("chontal de Tabasco del sur") };

        var result = InaliCatalogImporter.ResolveVariantForAutodenomination(
            Row("yoko t'an", "chontal de Tabasco del norte"), variants);

        result.VariantId.Should().Be(variants[0].Id);
        result.Method.Should().Be("spanish_name_exact");
    }

    [Fact]
    public void Resolve_TwoExactCandidates_IsAmbiguous_AndNeverPicksOne()
    {
        var variants = new List<LanguageVariant> { Variant("zapoteco"), Variant("zapoteco") };

        var result = InaliCatalogImporter.ResolveVariantForAutodenomination(
            Row("dichsah", "zapoteco"), variants);

        result.VariantId.Should().BeNull("two equally valid exact candidates must not be silently resolved");
        result.FailureMethod.Should().Be("spanish_name_exact_ambiguous");
    }

    [Fact]
    public void Resolve_UniquePrefix_Resolves()
    {
        var variants = new List<LanguageVariant> { Variant("mixteco de Santa Cruz Itundujia"), Variant("chatino occidental bajo") };

        var result = InaliCatalogImporter.ResolveVariantForAutodenomination(
            Row("tu'un savi", "mixteco de Santa"), variants);

        result.VariantId.Should().Be(variants[0].Id);
        result.Method.Should().Be("spanish_name_prefix_unique");
    }

    [Fact]
    public void Resolve_AmbiguousPrefix_WithoutEvidence_Quarantines()
    {
        var variants = new List<LanguageVariant>
        {
            Variant("zapoteco de Valles, del norte"),
            Variant("zapoteco de Valles, del sur"),
            Variant("zapoteco de Valles, del este")
        };

        var result = InaliCatalogImporter.ResolveVariantForAutodenomination(
            Row("dichsah", "zapoteco de Valles"), variants);

        result.VariantId.Should().BeNull("an ambiguous prefix must be quarantined, not resolved to prefixMatches[0]");
        result.FailureMethod.Should().Be("spanish_name_prefix_ambiguous");
        result.FailureReason.Should().Contain("3 candidates");
    }

    /// <summary>
    /// Regression for the 474 -> 484 accounting defect. A row whose progressive prefix is
    /// ambiguous used to add a terminal outcome and then keep iterating, producing a SECOND
    /// terminal outcome for the same source row. The resolver must report exactly one
    /// unresolved outcome and must be deterministic.
    /// </summary>
    [Fact]
    public void Resolve_AmbiguousProgressivePrefix_ReportsExactlyOneUnresolvedOutcome()
    {
        var variants = Enumerable.Range(0, 15)
            .Select(i => Variant($"zapoteco de Valles, del {i}"))
            .ToList();

        var row = Row("dichsah", "zapoteco de Valles, del este central", page: 247, ordinal: 101);

        var first = InaliCatalogImporter.ResolveVariantForAutodenomination(row, variants);
        var second = InaliCatalogImporter.ResolveVariantForAutodenomination(row, variants);

        first.VariantId.Should().BeNull();
        first.FailureMethod.Should().Be("spanish_name_progressive_prefix_ambiguous");
        first.FailureReason.Should().Contain("15 candidates");
        second.Should().Be(first, "resolution must be deterministic for the same source row");
    }

    [Fact]
    public void Resolve_UniqueProgressivePrefix_Resolves()
    {
        var variants = new List<LanguageVariant> { Variant("zapoteco serrano"), Variant("chatino occidental bajo") };

        var result = InaliCatalogImporter.ResolveVariantForAutodenomination(
            Row("bene xono", "zapoteco serrano, del noroeste bajo"), variants);

        result.VariantId.Should().Be(variants[0].Id);
        result.Method.Should().Be("spanish_name_progressive_prefix_unique");
    }

    [Fact]
    public void Resolve_UniqueSuffix_Resolves()
    {
        var variants = new List<LanguageVariant> { Variant("zapoteco serrano"), Variant("chatino occidental bajo") };

        var result = InaliCatalogImporter.ResolveVariantForAutodenomination(
            Row("bene xono", "zapoteco serrano del noroeste bajo"), variants);

        result.VariantId.Should().Be(variants[0].Id);
        result.Method.Should().Be("spanish_name_suffix_unique");
    }

    [Fact]
    public void Resolve_NoCandidatesAtAll_ReportsNoMatch()
    {
        var variants = new List<LanguageVariant> { Variant("chatino occidental bajo") };

        var result = InaliCatalogImporter.ResolveVariantForAutodenomination(
            Row("dichsah", "zapoteco de Valles, del este central", page: 247), variants);

        result.VariantId.Should().BeNull();
        result.FailureMethod.Should().Be("no_match");
    }

    [Fact]
    public void Resolve_HeaderRowSpanishName_IsNotMapped()
    {
        var result = InaliCatalogImporter.ResolveVariantForAutodenomination(
            Row("Autodenominación", "Nombre en español"), new List<LanguageVariant>());

        result.VariantId.Should().BeNull();
        result.FailureMethod.Should().Be("no_spanish_name");
    }

    // ---------------------------------------------------- quarantine identity

    [Fact]
    public void Quarantine_RawDataHash_IsDeterministicAndWhitespaceInsensitive()
    {
        var a = new ImportQuarantine(Guid.NewGuid(), Guid.NewGuid(), "LanguageVariantAutodenomination",
            "{\"autodenom\":\"dichsah\",\"page\":247}", null, 247, "Appendix4", "reason", "no_match");
        var b = new ImportQuarantine(Guid.NewGuid(), Guid.NewGuid(), "LanguageVariantAutodenomination",
            "{ \"autodenom\": \"dichsah\",\n  \"page\": 247 }", null, 247, "Appendix4", "reason", "no_match");

        b.RawDataHash.Should().Be(a.RawDataHash, "whitespace canonicalization must not change the identity hash");
    }

    /// <summary>
    /// 5B.md FASE 9: the hash must distinguish source ROWS, not merely source content. Two
    /// documentarily distinct rows (different ordinal / page) must not collapse into one identity.
    /// The serialized source row carries SourceOrdinal, so the hash does include provenance.
    /// </summary>
    [Fact]
    public void Quarantine_RawDataHash_DistinguishesIdenticalContentOnDifferentSourceRows()
    {
        var shared = "\"autodenom\":\"dichsah\",\"spanish_name\":\"zapoteco de Valles\",\"page\":247";
        var row101 = new ImportQuarantine(Guid.NewGuid(), Guid.NewGuid(), "LanguageVariantAutodenomination",
            $"{{" + shared + ",\"SourceOrdinal\":101}", null, 247, "Appendix4", "reason", "no_match");
        var row108 = new ImportQuarantine(Guid.NewGuid(), Guid.NewGuid(), "LanguageVariantAutodenomination",
            $"{{" + shared + ",\"SourceOrdinal\":108}", null, 247, "Appendix4", "reason", "no_match");

        row101.RawDataHash.Should().NotBe(row108.RawDataHash);
    }

    // ------------------------------------------------------ end-to-end import

    private static AtlasBuhoDbContext NewInMemoryContext(string name)
        => new(new DbContextOptionsBuilder<AtlasBuhoDbContext>()
            .UseInMemoryDatabase(name)
            // The importer wraps the whole run in a transaction; the in-memory store has none.
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options);

    [Fact]
    public async Task Import_AccountsForExactly474AutodenominationRows_AndIsIdempotent()
    {
        var dbName = $"inali-import-{Guid.NewGuid()}";

        // ---- Run 1 on a clean context -------------------------------------
        ImportResult first;
        await using (var ctx = NewInMemoryContext(dbName))
        {
            var importer = new InaliCatalogImporter(ctx, NullLogger<InaliCatalogImporter>.Instance);
            first = await importer.ImportInaliCatalogAsync();
        }

        first.Success.Should().BeTrue(first.ErrorMessage);
        (first.AutodenominationsImported + first.AutodenominationsQuarantined)
            .Should().Be(DocumentaryValidAutodenominations,
                "Precisely 474 valid Appendix 4 rows must produce 474 terminal outcomes");

        int autodenominationsAfterRun1;
        int quarantinesAfterRun1;
        await using (var ctx = NewInMemoryContext(dbName))
        {
            autodenominationsAfterRun1 = await ctx.LanguageVariantAutodenominations.CountAsync();
            quarantinesAfterRun1 = await ctx.ImportQuarantines.CountAsync();
            autodenominationsAfterRun1.Should().Be(first.AutodenominationsImported);
        }

        // ---- Run 2, same source + same parser ------------------------------
        ImportResult second;
        await using (var ctx = NewInMemoryContext(dbName))
        {
            var importer = new InaliCatalogImporter(ctx, NullLogger<InaliCatalogImporter>.Instance);
            second = await importer.ImportInaliCatalogAsync();
        }

        second.Success.Should().BeTrue(second.ErrorMessage);
        second.AutodenominationsImported.Should().Be(first.AutodenominationsImported,
            "a re-import of the same source+parser must not change which rows are imported");
        second.AutodenominationsQuarantined.Should().Be(first.AutodenominationsQuarantined);

        await using (var ctx = NewInMemoryContext(dbName))
        {
            (await ctx.LanguageVariantAutodenominations.CountAsync()).Should().Be(autodenominationsAfterRun1,
                "re-import must not create duplicate catalog rows");
            (await ctx.ImportQuarantines.CountAsync()).Should().Be(quarantinesAfterRun1,
                "re-import must not accumulate duplicate quarantine rows");
        }
    }

    /// <summary>
    /// 5B.md FASE 12 / FASE 14 Test G — a failed validation must leave NO partial import state.
    /// The tampered source file is restored in a finally block, so this test is safe to re-run.
    /// </summary>
    [Fact]
    public async Task Import_WhenValidationFails_RollsBackAllPartialState()
    {
        var dbName = $"inali-import-rollback-{Guid.NewGuid()}";
        var basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
        var appendix4Path = Path.Combine(basePath, "catalogs", "parsed", "appendix4_parsed.json");

        // First run must succeed and complete.
        ImportResult first;
        await using (var ctx = NewInMemoryContext(dbName))
        {
            first = await new InaliCatalogImporter(ctx, NullLogger<InaliCatalogImporter>.Instance)
                .ImportInaliCatalogAsync();
        }
        first.Success.Should().BeTrue(first.ErrorMessage);

        var snapshot = await File.ReadAllBytesAsync(appendix4Path);
        try
        {
            // Tamper: delete one valid appendix row so the 474 baseline cannot be met.
            var rows = System.Text.Json.JsonDocument.Parse(snapshot).RootElement.EnumerateArray()
                .Select(e => e.GetRawText()).ToList();
            rows.RemoveAt(rows.Count - 2); // last row is the trailing header artifact observation
            await File.WriteAllTextAsync(appendix4Path, "[" + string.Join(",", rows) + "]");

            ImportResult failed;
            await using (var ctx = NewInMemoryContext(dbName))
            {
                failed = await new InaliCatalogImporter(ctx, NullLogger<InaliCatalogImporter>.Instance)
                    .ImportInaliCatalogAsync();
            }

            failed.Success.Should().BeFalse("the 474 documentary baseline cannot be met with a row missing");
            failed.ErrorMessage.Should().Contain("DOCUMENTARY BASELINE VALIDATION FAILED");

            // Rollback guarantees: nothing changed relative to the completed first run.
            await using (var ctx = NewInMemoryContext(dbName))
            {
                (await ctx.LanguageFamilies.CountAsync()).Should().Be(11);
                (await ctx.LanguageGroups.CountAsync()).Should().Be(68);
                (await ctx.LanguageVariants.CountAsync()).Should().Be(350);
                (await ctx.LanguageVariantAutodenominations.CountAsync())
                    .Should().Be(first.AutodenominationsImported, "rollback must not leave partial imported rows");
                (await ctx.ImportQuarantines.CountAsync())
                    .Should().Be(first.VariantsQuarantined + first.AutodenominationsQuarantined,
                        "rollback must not leave partial quarantine rows");

                var version = await ctx.CatalogVersions
                    .SingleAsync(v => v.ImportStatus != null);
                version.ImportStatus.Should().Be("Completed",
                    "a failed re-import must not corrupt the previously completed CatalogVersion");
            }
        }
        finally
        {
            await File.WriteAllBytesAsync(appendix4Path, snapshot);
        }
    }
}
