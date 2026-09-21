namespace AtlasBuho.Domain.Interfaces;

using AtlasBuho.Domain.Entities;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ILanguageFamilyRepository : IRepository<LanguageFamily>
{
    Task<LanguageFamily?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LanguageFamily>> GetWithGroupsAsync(CancellationToken cancellationToken = default);
}

public interface ILanguageGroupRepository : IRepository<LanguageGroup>
{
    Task<IReadOnlyList<LanguageGroup>> GetByFamilyAsync(Guid familyId, CancellationToken cancellationToken = default);
    Task<LanguageGroup?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}

public interface ILanguageVariantRepository : IRepository<LanguageVariant>
{
    Task<IReadOnlyList<LanguageVariant>> GetByGroupAsync(Guid groupId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LanguageVariant>> GetByRiskLevelAsync(Domain.Entities.RiskLevel level, CancellationToken cancellationToken = default);
    Task<LanguageVariant?> GetByInaliCodeAsync(string inaliCode, CancellationToken cancellationToken = default);
    Task<LanguageVariant?> GetByIsoCodeAsync(string isoCode, CancellationToken cancellationToken = default);
    Task<LanguageVariant?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ICommunityRepository : IRepository<Community>
{
    Task<IReadOnlyList<Community>> GetByVariantAsync(Guid variantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Community>> GetByRegionAsync(Guid regionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Community>> GetByStateAsync(string state, CancellationToken cancellationToken = default);
}

public interface IRegionRepository : IRepository<Region>
{
    Task<IReadOnlyList<Region>> GetByCountryAsync(string country, CancellationToken cancellationToken = default);
}

public interface ISpeakerProfileRepository : IRepository<SpeakerProfile>
{
    Task<IReadOnlyList<SpeakerProfile>> GetByVariantAsync(Guid variantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SpeakerProfile>> GetByCommunityAsync(Guid communityId, CancellationToken cancellationToken = default);
}

public interface ILexemeRepository : IRepository<Lexeme>
{
    Task<IReadOnlyList<Lexeme>> GetByVariantAsync(Guid variantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Lexeme>> GetBySemanticDomainAsync(string domain, CancellationToken cancellationToken = default);
    Task<Lexeme?> GetByCanonicalFormAsync(Guid variantId, string canonicalForm, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Lexeme>> SearchAsync(string query, CancellationToken cancellationToken = default);
    Task<Lexeme?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IMeaningRepository : IRepository<Meaning>
{
    Task<IReadOnlyList<Meaning>> GetByLexemeAsync(Guid lexemeId, CancellationToken cancellationToken = default);
}

public interface IPhraseRepository : IRepository<Phrase>
{
    Task<IReadOnlyList<Phrase>> GetByVariantAsync(Guid variantId, CancellationToken cancellationToken = default);
    Task<Phrase?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IExampleRepository : IRepository<Example>
{
    Task<IReadOnlyList<Example>> GetByLexemeAsync(Guid lexemeId, CancellationToken cancellationToken = default);
}

public interface IPronunciationRepository : IRepository<Pronunciation>
{
    Task<IReadOnlyList<Pronunciation>> GetByLexemeAsync(Guid lexemeId, CancellationToken cancellationToken = default);
    Task<Pronunciation?> GetPrimaryAsync(Guid lexemeId, CancellationToken cancellationToken = default);
}

public interface IAudioRecordingRepository : IRepository<AudioRecording>
{
    Task<IReadOnlyList<AudioRecording>> GetByVariantAsync(Guid variantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AudioRecording>> GetBySpeakerAsync(Guid speakerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AudioRecording>> GetByConsentStatusAsync(Domain.Entities.ConsentStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AudioRecording>> GetByLexemeAsync(Guid lexemeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AudioRecording>> GetByPhraseAsync(Guid phraseId, CancellationToken cancellationToken = default);
}

public interface IVideoRecordingRepository : IRepository<VideoRecording>
{
    Task<IReadOnlyList<VideoRecording>> GetByVariantAsync(Guid variantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<VideoRecording>> GetBySpeakerAsync(Guid speakerId, CancellationToken cancellationToken = default);
}

public interface IGrammarRuleRepository : IRepository<GrammarRule>
{
    Task<IReadOnlyList<GrammarRule>> GetByVariantAsync(Guid variantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GrammarRule>> GetByCategoryAsync(Guid variantId, string category, CancellationToken cancellationToken = default);
}

public interface IWritingSystemRepository : IRepository<WritingSystem>
{
    Task<IReadOnlyList<WritingSystem>> GetByVariantAsync(Guid variantId, CancellationToken cancellationToken = default);
    Task<WritingSystem?> GetOfficialAsync(Guid variantId, CancellationToken cancellationToken = default);
}

public interface IOrthographyRepository : IRepository<Orthography>
{
    Task<IReadOnlyList<Orthography>> GetByVariantAsync(Guid variantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Orthography>> GetByWritingSystemAsync(Guid writingSystemId, CancellationToken cancellationToken = default);
}

public interface ITranslationRepository : IRepository<Translation>
{
    Task<IReadOnlyList<Translation>> GetBySourceVariantAsync(Guid variantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Translation>> GetByTargetVariantAsync(Guid variantId, CancellationToken cancellationToken = default);
    Task<Translation?> GetByTextsAsync(Guid sourceVariantId, Guid targetVariantId, string sourceText, CancellationToken cancellationToken = default);
}

public interface ISourceRepository : IRepository<Source>
{
    Task<IReadOnlyList<Source>> GetByLevelAsync(Domain.Entities.SourceLevel level, CancellationToken cancellationToken = default);
    Task<Source?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}

public interface IEvidenceRepository : IRepository<Evidence>
{
    Task<IReadOnlyList<Evidence>> GetBySourceAsync(Guid sourceId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Evidence>> GetByEntityAsync(string entityType, Guid entityId, CancellationToken cancellationToken = default);
}

public interface IRiskAssessmentRepository : IRepository<RiskAssessment>
{
    Task<IReadOnlyList<RiskAssessment>> GetByVariantAsync(Guid variantId, CancellationToken cancellationToken = default);
    Task<RiskAssessment?> GetCurrentAsync(Guid variantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RiskAssessment>> GetByLevelAsync(Domain.Entities.RiskLevel level, CancellationToken cancellationToken = default);
}

public interface IConsentRecordRepository : IRepository<ConsentRecord>
{
    Task<IReadOnlyList<ConsentRecord>> GetBySpeakerAsync(Guid speakerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ConsentRecord>> GetByRecordingAsync(Guid recordingId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ConsentRecord>> GetActiveAsync(CancellationToken cancellationToken = default);
}

public interface ICulturalNoteRepository : IRepository<CulturalNote>
{
    Task<IReadOnlyList<CulturalNote>> GetByVariantAsync(Guid variantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CulturalNote>> GetByTypeAsync(Domain.Entities.CulturalNoteType type, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CulturalNote>> GetByCommunityAsync(Guid communityId, CancellationToken cancellationToken = default);
}

public interface IDialectRelationshipRepository : IRepository<DialectRelationship>
{
    Task<IReadOnlyList<DialectRelationship>> GetBySourceVariantAsync(Guid variantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DialectRelationship>> GetByTargetVariantAsync(Guid variantId, CancellationToken cancellationToken = default);
    Task<DialectRelationship?> GetBetweenVariantsAsync(Guid sourceId, Guid targetId, CancellationToken cancellationToken = default);
}

public interface IW1HContextRepository : IRepository<W1HContext>
{
    Task<IReadOnlyList<W1HContext>> GetBySpeakerAsync(Guid speakerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<W1HContext>> GetByCommunityAsync(Guid communityId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<W1HContext>> GetByVariantAsync(Guid variantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<W1HContext>> GetByEntityAsync(string entityType, Guid entityId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<W1HContext>> GetCompleteAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<W1HContext>> GetIncompleteAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<W1HContext>> GetByPurposeAsync(DocumentationPurpose purpose, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<W1HContext>> GetByMethodologyAsync(DocumentationMethodology methodology, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<W1HContext>> GetByDateRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
}