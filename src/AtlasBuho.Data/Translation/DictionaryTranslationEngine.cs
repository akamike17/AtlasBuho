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
/// Determinism (§5): same dataset version + engine version + input → same result.
/// No random selection, no arbitrary database ordering (explicit OrderBy on text).
public sealed class DictionaryTranslationEngine : ITranslationEngine
{
    /// Engine version: bump on any change to selection logic (6B.md §38).
    public string EngineVersion => "dictionary-1.0";

    private const string SpanishTag = "español";
    private const string EnglishTag = "english";

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
        var term = request.Text.Trim();

        // Unsupported langauge is a first-class state (6B.md §21): a language pair that the
        // canonical dataset does not support is never reported as a generic NotFound.
        var sourceKnown = IsSpanish(source) || IsEnglish(source) ||
            await _context.LanguageVariants.AnyAsync(v => v.Name.ToLower() == source.ToLower(), cancellationToken);
        var targetKnown = IsSpanish(target) || IsEnglish(target) ||
            await _context.LanguageVariants.AnyAsync(v => v.Name.ToLower() == target.ToLower(), cancellationToken);

        if (!sourceKnown || !targetKnown || (IsSpanish(source) && IsSpanish(target)) || (IsEnglish(source) && IsEnglish(target)))
        {
            return TranslationResult.UnsupportedLanguage(source, target, term, datasetVersion, EngineVersion);
        }

        // 6B.md §14: translation is directional; each direction queries only records that
        // evidence that direction (lexeme form -> meaning language, never the reverse without data).

        if (IsSpanish(target))
        {
            // Indigenous -> Spanish: lexeme canonical form -> verified SpanishMeaning.
            var raw = await VerifiedLexemeMatchesAsync(term)
                .Select(l => new { Text = l.SpanishMeaning!, l.VerificationStatus })
                .ToListAsync(cancellationToken);
            var matches = DistinctOrdered(raw.Select(m => (m.Text, m.VerificationStatus)));

            return BuildResult(source, target, term, datasetVersion,
                matches.Select(m => new TranslationAlternative(m.Text, "exact_lexical", m.Status.ToString())).ToList());
        }

        if (IsEnglish(target))
        {
            // Indigenous -> English: lexeme canonical form -> verified Meaning.EnglishMeaning.
            var raw = await VerifiedLexemeMatchesAsync(term)
                .SelectMany(l => l.Meanings)
                .Where(m => m.EnglishMeaning != null &&
                    (m.VerificationStatus == VerificationStatus.Verified ||
                     m.VerificationStatus == VerificationStatus.Documented ||
                     m.VerificationStatus == VerificationStatus.CommunityVerified ||
                     m.VerificationStatus == VerificationStatus.AcademicVerified))
                .Select(m => new { Text = m.EnglishMeaning!, m.VerificationStatus })
                .ToListAsync(cancellationToken);
            var matches = DistinctOrdered(raw.Select(m => (m.Text, m.VerificationStatus)));

            return BuildResult(source, target, term, datasetVersion,
                matches.Select(m => new TranslationAlternative(m.Text, "exact_lexical", m.Status.ToString())).ToList());
        }

        if (IsSpanish(source))
        {
            // Spanish -> Indigenous: verified SpanishMeaning -> lexeme canonical form.
            var raw = await _context.Lexemes
                .Where(l => l.SpanishMeaning != null && l.SpanishMeaning == term &&
                    (l.VerificationStatus == VerificationStatus.Verified ||
                     l.VerificationStatus == VerificationStatus.Documented ||
                     l.VerificationStatus == VerificationStatus.CommunityVerified ||
                     l.VerificationStatus == VerificationStatus.AcademicVerified))
                .Select(l => new { Text = l.CanonicalForm, l.VerificationStatus })
                .ToListAsync(cancellationToken);
            var matches = DistinctOrdered(raw.Select(m => (m.Text, m.VerificationStatus)));

            return BuildResult(source, target, term, datasetVersion,
                matches.Select(m => new TranslationAlternative(m.Text, "exact_lexical", m.Status.ToString())).ToList());
        }

        // English -> Indigenous: no verified reverse evidence recorded in the dataset yet.
        return TranslationResult.NotFound(source, target, term, datasetVersion, EngineVersion);
    }

    private IQueryable<Lexeme> VerifiedLexemeMatchesAsync(string term)
        => _context.Lexemes
            .Include(l => l.Meanings)
            .Where(l => l.CanonicalForm == term && l.SpanishMeaning != null &&
                (l.VerificationStatus == VerificationStatus.Verified ||
                 l.VerificationStatus == VerificationStatus.Documented ||
                 l.VerificationStatus == VerificationStatus.CommunityVerified ||
                 l.VerificationStatus == VerificationStatus.AcademicVerified));

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
