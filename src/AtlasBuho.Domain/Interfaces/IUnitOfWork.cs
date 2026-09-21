namespace AtlasBuho.Domain.Interfaces;

using AtlasBuho.Domain.Entities;

public interface IUnitOfWork : IDisposable
{
    ILanguageFamilyRepository LanguageFamilies { get; }
    ILanguageGroupRepository LanguageGroups { get; }
    ILanguageVariantRepository LanguageVariants { get; }
    ICommunityRepository Communities { get; }
    IRegionRepository Regions { get; }
    ISpeakerProfileRepository Speakers { get; }
    ILexemeRepository Lexemes { get; }
    IMeaningRepository Meanings { get; }
    IPhraseRepository Phrases { get; }
    IExampleRepository Examples { get; }
    IPronunciationRepository Pronunciations { get; }
    IAudioRecordingRepository AudioRecordings { get; }
    IVideoRecordingRepository VideoRecordings { get; }
    IGrammarRuleRepository GrammarRules { get; }
    IWritingSystemRepository WritingSystems { get; }
    IOrthographyRepository Orthographies { get; }
    ITranslationRepository Translations { get; }
    ISourceRepository Sources { get; }
    IEvidenceRepository Evidence { get; }
    IRiskAssessmentRepository RiskAssessments { get; }
    IConsentRecordRepository ConsentRecords { get; }
    ICulturalNoteRepository CulturalNotes { get; }
    IDialectRelationshipRepository DialectRelationships { get; }
    IW1HContextRepository W1HContexts { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}