using AtlasBuho.Application.Translation;
using AtlasBuho.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AtlasBuho.Data.Translation;

/// 6B.md §4: the first engine works exclusively from VERIFIED canonical data.
/// Automatic matching tiers (§4/§17): unique exact verified lexical match only.
/// Ambiguity among verified candidates is never silently resolved (§6): the result is
/// Translated with all verified alternatives listed, ordered deterministically.
/// No verified match ever becomes an invented translation (§19): the result is NotFound.
///
/// 6B.md §9 (VARIANT ISOLATION — the P0 fix of this pass): a lexeme is scoped to its
/// LanguageVariantId; the same canonical form in a different variant is a DIFFERENT
/// linguistic fact and must never leak into the result. The engine resolves the source
/// variant ONCE and constrains every lexical query by that variant identity.
///
/// Determinism (§5): same dataset version + engine version + input → same result.
/// No random selection, no arbitrary database ordering (explicit OrderBy on text).
public sealed class DictionaryTranslationEngine : ITranslationEngine
{
    /// Engine version: bump on any change to selection logic (6B.md §38).
    public string EngineVersion => "dictionary-1.1";

    private const string SpanishTag = "español";
    private const string EnglishTag = "english";

    private static readonly VerificationStatus[] VerifiedStates =
    {
        VerificationStatus.Verified, VerificationStatus.Documented,
        VerificationStatus.CommunityVerified, VerificationStatus.AcademicVerified
    };

    private readonly AtlasBuhoDbContext _context;

    public DictionaryTranslationEngine(AtlasBuhoDbContext context)
    {
        _context = context;
    }

    public async Task<TranslationResult> TranslateAsync(TranslationRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Text) ||
            string.IsNullOrWhiteSpace(request.SourceLanguage) ||
            string.IsNullOrWhiteSpace(request.TargetLanguage))
        {
            return TranslationResult.InvalidInput(request.SourceLanguage ?? "", request.TargetLanguage ?? "");
        }

        var datasetVersion = await ResolveDatasetVersionAsync(cancellationToken);

        var source = NormalizeLanguage(request.SourceLanguage);
        var target = NormalizeLanguage(request.TargetLanguage);
        var term = request.Text.Trim(); // comparison value only; canonical form is never rewritten

        // 6B.md §7/§9: resolve the language identity to a STABLE internal ID, never matching
        // the lexeme by display name. An ambiguous name (0/2+ variants) is a first-class
        // UnsupportedLanguage, never silently "closest" picked.
        var languageIds = await ResolveLanguageIdsAsync(source, target, cancellationToken);
        if (languageIds == null || (IsSpanish(source) && IsSpanish(target)) || (IsEnglish(source) && IsEnglish(target)))
        {
            return TranslationResult.UnsupportedLanguage(source, target, term, datasetVersion, EngineVersion);
        }

        var (sourceVariantId, targetVariantId) = languageIds.Value;

        if (IsSpanish(target))
        {
            // Indigenous -> Spanish: canonical form within the SOURCE variant only -> verified SpanishMeaning.
            var raw = await VerifiedLexemeMatchesAsync(sourceVariantId!.Value, term)
                .Select(l => new { Text = l.SpanishMeaning!, l.VerificationStatus })
                .ToListAsync(cancellationToken);
            var matches = DistinctOrdered(raw.Select(m => (m.Text, m.VerificationStatus)));

            return BuildResult(source, target, term, datasetVersion,
                matches.Select(m => new TranslationAlternative(m.Text, "exact_lexical", m.Status.ToString())).ToList());
        }

        if (IsEnglish(target))
        {
            // Indigenous -> English: canonical form within the SOURCE variant only -> verified Meaning.EnglishMeaning.
            var raw = await VerifiedLexemeMatchesAsync(sourceVariantId!.Value, term)
                .SelectMany(l => l.Meanings)
                .Where(m => m.EnglishMeaning != null && VerifiedStates.Contains(m.VerificationStatus))
                .Select(m => new { Text = m.EnglishMeaning!, m.VerificationStatus })
                .ToListAsync(cancellationToken);
            var matches = DistinctOrdered(raw.Select(m => (m.Text, m.VerificationStatus)));

            return BuildResult(source, target, term, datasetVersion,
                matches.Select(m => new TranslationAlternative(m.Text, "exact_lexical", m.Status.ToString())).ToList());
        }

        if (IsSpanish(source))
        {
            // Spanish -> Indigenous: verified SpanishMeaning within the TARGET variant only.
            var raw = await _context.Lexemes
                .Where(l => l.LanguageVariantId == targetVariantId!.Value && l.SpanishMeaning == term &&
                            VerifiedStates.Contains(l.VerificationStatus))
                .Select(l => new { Text = l.CanonicalForm, l.VerificationStatus })
                .ToListAsync(cancellationToken);
            var matches = DistinctOrdered(raw.Select(m => (m.Text, m.VerificationStatus)));

            return BuildResult(source, target, term, datasetVersion,
                matches.Select(m => new TranslationAlternative(m.Text, "exact_lexical", m.Status.ToString())).ToList());
        }

        // English -> Indigenous: no verified reverse evidence recorded in the dataset yet.
        return TranslationResult.NotFound(source, target, term, datasetVersion, EngineVersion);
    }

    /// 6B.md §7/§9: language name -> EXACTLY ONE stable LanguageVariantId, or null when the
    /// language is unsupported / ambiguous. Never resolves by partial name, never picks first.
    private async Task<(Guid? SourceVariantId, Guid? TargetVariantId)?> ResolveLanguageIdsAsync(
        string source, string target, CancellationToken cancellationToken)
    {
        var sourceId = await ResolveLanguageIdAsync(source, cancellationToken);
        var targetId = await ResolveLanguageIdAsync(target, cancellationToken);
        return sourceId == null || targetId == null ? null : (sourceId, targetId);
    }

    private async Task<Guid?> ResolveLanguageIdAsync(string language, CancellationToken cancellationToken)
    {
        if (IsSpanish(language) || IsEnglish(language)) return Guid.Empty; // null-variant sentinel for es/en
        var matches = await _context.LanguageVariants
            .Where(v => v.Name.ToLower() == language.ToLower())
            .Select(v => v.Id)
            .ToListAsync(cancellationToken);
        return matches.Count == 1 ? matches[0] : null;
    }

    private IQueryable<Lexeme> VerifiedLexemeMatchesAsync(Guid variantId, string term)
        => _context.Lexemes
            .Include(l => l.Meanings)
            .Where(l => l.LanguageVariantId == variantId && l.CanonicalForm == term &&
                        l.SpanishMeaning != null && VerifiedStates.Contains(l.VerificationStatus));

    /// Deterministic dedup + ordinal ordering, applied in memory on the bounded candidate set
    /// (never relies on database row ordering — 6B.md §5).
    private static List<(string Text, VerificationStatus Status)> DistinctOrdered(
        IEnumerable<(string Text, VerificationStatus Status)> matches)
        => matches
            .GroupBy(m => m.Text, StringComparer.Ordinal)
            .Select(g => g.OrderByDescending(m => (int)m.Status).First())
            .OrderBy(m => m.Text, StringComparer.Ordinal)
            .ToList();

    private TranslationResult BuildResult(string source, string target, string term, string datasetVersion,
        IReadOnlyList<TranslationAlternative> matches)
    {
        if (matches.Count == 0)
            return TranslationResult.NotFound(source, target, term, datasetVersion, EngineVersion);

        // 6B.md §6: never silently invent a winner; the primary entry is the deterministic
        // first by ordinal text order and every verified match is exposed as an alternative.
        var first = matches[0];
        return new TranslationResult(
            TranslationStatus.Translated,
            source, target, term,
            first.Text, first.MatchType, first.VerificationStatus,
            datasetVersion, EngineVersion, matches);
    }

    private async Task<string> ResolveDatasetVersionAsync(CancellationToken cancellationToken)
    {
        // 6B.md §37: results report which dataset produced them. The dataset of record is the
        // latest COMPLETED catalog version — never a Pending/Failed one (5B.md FASE 12).
        var version = await _context.CatalogVersions
            .Where(v => v.ImportStatus == "Completed")
            .OrderByDescending(v => v.RetrievedAt)
            .Select(v => v.VersionNumber)
            .FirstOrDefaultAsync(cancellationToken);

        return version ?? "unversioned";
    }

    private static bool IsSpanish(string language) => language.Equals(SpanishTag, StringComparison.OrdinalIgnoreCase);
    private static bool IsEnglish(string language) => language.Equals(EnglishTag, StringComparison.OrdinalIgnoreCase);

    private static string NormalizeLanguage(string language) => language.Trim();
}
