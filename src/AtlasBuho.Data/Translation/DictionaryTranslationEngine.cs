using AtlasBuho.Application.Translation;
using AtlasBuho.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AtlasBuho.Data.Translation;

/// <summary>
/// ADR 0001 / 6B.md — dictionary-2.0: the engine reads LexicalEquivalence, not
/// Lexeme.SpanishMeaning. Every translation candidate is backed by a row of evidence with an
/// explicit target language, source reference, corpus version and canonical flag.
///
/// Invariants (see ADR 0001):
///   I1  at most one canonical per (SourceLexemeId, TargetLanguage, CatalogVersionId)
///   I2  directional evidence only; nothing is ever reversed structurally
///   I3  completed versions are immutable; evidence is never silently rewritten
///   I4  variant isolation: the same form in two different variants is a different fact
///
/// Selection: exactly one canonical determines Translation; 0 canonical → null; ≥2 canonical
/// is a data-integrity violation, never a tie resolved by ordering.
/// </summary>
public sealed class DictionaryTranslationEngine : ITranslationEngine
{
    /// Engine version: bump on any change to selection logic (6B.md §38).
    public string EngineVersion => "dictionary-2.0";

    private const string SpanishTag = "español";
    private const string EnglishTag = "english";
    private const string TargetEs = "es";
    private const string TargetEn = "en";

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

        var source = NormalizeLanguage(request.SourceLanguage);
        var target = NormalizeLanguage(request.TargetLanguage);
        var term = request.Text.Trim();

        var datasetVersion = await ResolveDatasetVersionAsync(cancellationToken);

        var sourceVariantId = await ResolveVariantIdAsync(source, cancellationToken);
        var targetVariantId = await ResolveVariantIdAsync(target, cancellationToken);

        if (sourceVariantId == null && targetVariantId == null &&
            !IsSpanishTag(source) && !IsEnglish(source) && !IsSpanishTag(target) && !IsEnglish(target))
        {
            return TranslationResult.UnsupportedLanguage(source, target, term, datasetVersion, EngineVersion);
        }

        // Determine the direction BEFORE resolving targetLangCode. The reverse direction
        // (es/en -> Indigenous) has no linguistic 'target' code for LexicalEquivalence (its
        // output is an indigenous canonical form), so the code is only meaningful when
        // translating OUT of an indigenous variant.
        var isOutgoingFromSourceVariant = sourceVariantId.HasValue && !IsEnglish(source) && !IsSpanishTag(source);
        var isIncomingToTargetVariant = targetVariantId.HasValue && !IsSpanishTag(target) && !IsEnglish(target);

        var targetLangCode = IsSpanishTag(target) ? TargetEs : IsEnglish(target) ? TargetEn : null;
        if (!isIncomingToTargetVariant && targetLangCode == null)
        {
            return TranslationResult.UnsupportedLanguage(source, target, term, datasetVersion, EngineVersion);
        }

        if (isOutgoingFromSourceVariant)
        {
            // Lexeme -> es/en via LexicalEquivalence on the resolved lexeme
            var lexemes = await _context.Lexemes
                .Where(l => l.LanguageVariantId == sourceVariantId!.Value && l.CanonicalForm == term)
                .Select(l => l.Id)
                .ToListAsync(cancellationToken);

            var equivalences = await _context.LexicalEquivalences
                .Where(e => lexemes.Contains(e.SourceLexemeId) &&
                            e.TargetLanguage == targetLangCode &&
                            VerifiedStates.Contains(e.VerificationStatus) &&
                            e.CatalogVersion!.ImportStatus == "Completed")
                .Include(e => e.EvidenceSource)
                .ToListAsync(cancellationToken);

            return BuildResult(source, target, term, datasetVersion, equivalences);
        }

        if (isIncomingToTargetVariant && (IsSpanishTag(source) || IsEnglish(source)))
        {
            // es/en -> Indigenous: reverse direction is its OWN evidence (ADR 0001 I2).
            // The Spanish/English input matches a SpanishMeaning on the TARGET variant's
            // lexemes, and the resolved translation is that lexeme's canonical form.
            var lexemes = await _context.Lexemes
                .Where(l => l.LanguageVariantId == targetVariantId!.Value &&
                            l.SpanishMeaning == term)
                .ToListAsync(cancellationToken);

            if (lexemes.Count == 0)
            {
                return TranslationResult.NotFound(source, target, term, datasetVersion, EngineVersion);
            }

            var lexemeIds = lexemes.Select(l => l.Id).ToList();

            var equivalences = await _context.LexicalEquivalences
                .Where(e => lexemeIds.Contains(e.SourceLexemeId) &&
                            e.TargetLanguage == (IsSpanishTag(source) ? TargetEs : TargetEn) &&
                            VerifiedStates.Contains(e.VerificationStatus) &&
                            e.CatalogVersion!.ImportStatus == "Completed")
                .Include(e => e.EvidenceSource)
                .ToListAsync(cancellationToken);

            // Only equivalences whose language matches the source side AND whose source lexeme
            // has the matching SpanishMeaning.
            var lexemeById = lexemes.ToDictionary(l => l.Id);
            var matches = equivalences
                .Where(e => lexemeById.TryGetValue(e.SourceLexemeId, out var lx) &&
                            lx.SpanishMeaning == term)
                .ToList();

            if (matches.Count == 0)
            {
                return TranslationResult.NotFound(source, target, term, datasetVersion, EngineVersion);
            }

            var reverseCanonical = matches.Where(e => e.IsCanonical).ToList();
            if (reverseCanonical.Count > 1)
                throw new InvalidOperationException("INTEGRITY VIOLATION (reverse direction I1)");

            var reversePrimary = reverseCanonical.Count == 1
                ? lexemeById[reverseCanonical[0].SourceLexemeId].CanonicalForm
                : null;

            var reverseAlternatives = matches
                .OrderByDescending(e => e.IsCanonical)
                .ThenBy(e => lexemeById[e.SourceLexemeId].CanonicalForm, StringComparer.Ordinal)
                .ThenBy(e => e.EvidenceSource!.Name, StringComparer.Ordinal)
                .Select(e => new TranslationAlternative(
                    lexemeById[e.SourceLexemeId].CanonicalForm,
                    e.IsCanonical ? "canonical" : "alternative",
                    e.VerificationStatus.ToString()))
                .ToList();

            return new TranslationResult(
                reversePrimary != null ? TranslationStatus.Translated : TranslationStatus.NotFound,
                source, target, term,
                reversePrimary,
                reverseCanonical.Count == 1 ? "canonical_evidence" : null,
                reverseCanonical.Count == 1 ? reverseCanonical[0].VerificationStatus.ToString() : null,
                datasetVersion, EngineVersion, reverseAlternatives);
        }

        if ((IsSpanishTag(source) && IsSpanishTag(target)) || (IsEnglish(source) && IsEnglish(target)))
        {
            return TranslationResult.UnsupportedLanguage(source, target, term, datasetVersion, EngineVersion);
        }

        // Any remaining combination (e.g. English -> Indigenous with no recorded reverse
        // evidence) yields an honest NotFound rather than a guess (6B.md §19, Invariant 6).
        return TranslationResult.NotFound(source, target, term, datasetVersion, EngineVersion);
    }

    private TranslationResult BuildResult(string source, string target, string term, string datasetVersion,
        List<LexicalEquivalence> equivalences)
    {
        if (equivalences.Count == 0)
            return TranslationResult.NotFound(source, target, term, datasetVersion, EngineVersion);

        // Never collapse provenance: distinct TargetText may still appear with multiple sources.
        var canonical = equivalences.Where(e => e.IsCanonical).ToList();
        if (canonical.Count > 1)
            throw new InvalidOperationException(
                $"INTEGRITY VIOLATION: {canonical.Count} canonical LexicalEquivalence rows for the same " +
                $"(lexeme scope, target language, catalog version). Refusing to pick an arbitrary winner " +
                $"(ADR 0001 I1).");

        var primary = canonical.Count == 1 ? canonical[0].TargetText : null;

        // Present specially-determined deterministic order, never by alphabetically privileging.
        var alternatives = equivalences
            .OrderByDescending(e => e.IsCanonical)
            .ThenBy(e => e.TargetText, StringComparer.Ordinal)
            .ThenBy(e => e.EvidenceSource!.Name, StringComparer.Ordinal)
            .Select(e => new TranslationAlternative(
                e.TargetText,
                e.IsCanonical ? "canonical" : "alternative",
                e.VerificationStatus.ToString()))
            .ToList();

        return new TranslationResult(
            primary != null ? TranslationStatus.Translated : TranslationStatus.NotFound,
            source, target, term,
            primary,
            canonical.Count == 1 ? "canonical_evidence" : null,
            canonical.Count == 1 ? canonical[0].VerificationStatus.ToString() : null,
            datasetVersion, EngineVersion, alternatives);
    }

    private async Task<Guid?> ResolveVariantIdAsync(string language, CancellationToken cancellationToken)
    {
        if (IsSpanishTag(language) || IsEnglish(language)) return null;
        var matches = await _context.LanguageVariants
            .Where(v => v.Name.ToLower() == language.ToLower())
            .Select(v => v.Id)
            .ToListAsync(cancellationToken);
        return matches.Count == 1 ? matches[0] : null;
    }

    private async Task<string> ResolveDatasetVersionAsync(CancellationToken cancellationToken)
    {
        var version = await _context.CatalogVersions
            .Where(v => v.ImportStatus == "Completed")
            .OrderByDescending(v => v.RetrievedAt)
            .Select(v => v.VersionNumber)
            .FirstOrDefaultAsync(cancellationToken);

        return version ?? "unversioned";
    }

    private static bool IsSpanishTag(string language)
    {
        var lower = language.ToLowerInvariant();
        return lower == "español" || lower == "espanol" || lower == "spanish";
    }
    private static bool IsEnglish(string language) => language.Equals(EnglishTag, StringComparison.OrdinalIgnoreCase);
    private static string NormalizeLanguage(string language) => language.Trim();
}
