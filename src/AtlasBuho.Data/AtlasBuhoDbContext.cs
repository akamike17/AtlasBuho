namespace AtlasBuho.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

public class AtlasBuhoDbContext : DbContext, Domain.Interfaces.IUnitOfWork
{
    public AtlasBuhoDbContext(DbContextOptions<AtlasBuhoDbContext> options) : base(options) { }

    // DbSets for EF Core (public for repository access)
    public DbSet<Domain.Entities.LanguageFamily> LanguageFamilies => Set<Domain.Entities.LanguageFamily>();
    public DbSet<Domain.Entities.LanguageGroup> LanguageGroups => Set<Domain.Entities.LanguageGroup>();
    public DbSet<Domain.Entities.LanguageVariant> LanguageVariants => Set<Domain.Entities.LanguageVariant>();
    public DbSet<Domain.Entities.Community> Communities => Set<Domain.Entities.Community>();
    public DbSet<Domain.Entities.Region> Regions => Set<Domain.Entities.Region>();
    public DbSet<Domain.Entities.SpeakerProfile> Speakers => Set<Domain.Entities.SpeakerProfile>();
    public DbSet<Domain.Entities.Lexeme> Lexemes => Set<Domain.Entities.Lexeme>();
    public DbSet<Domain.Entities.Meaning> Meanings => Set<Domain.Entities.Meaning>();
    public DbSet<Domain.Entities.Phrase> Phrases => Set<Domain.Entities.Phrase>();
    public DbSet<Domain.Entities.PhraseLexeme> PhraseLexemes => Set<Domain.Entities.PhraseLexeme>();
    public DbSet<Domain.Entities.Example> Examples => Set<Domain.Entities.Example>();
    public DbSet<Domain.Entities.Pronunciation> Pronunciations => Set<Domain.Entities.Pronunciation>();
    public DbSet<Domain.Entities.AudioRecording> AudioRecordings => Set<Domain.Entities.AudioRecording>();
    public DbSet<Domain.Entities.VideoRecording> VideoRecordings => Set<Domain.Entities.VideoRecording>();
    public DbSet<Domain.Entities.GrammarRule> GrammarRules => Set<Domain.Entities.GrammarRule>();
    public DbSet<Domain.Entities.WritingSystem> WritingSystems => Set<Domain.Entities.WritingSystem>();
    public DbSet<Domain.Entities.Orthography> Orthographies => Set<Domain.Entities.Orthography>();
    public DbSet<Domain.Entities.Translation> Translations => Set<Domain.Entities.Translation>();
    public DbSet<Domain.Entities.Source> Sources => Set<Domain.Entities.Source>();
    public DbSet<Domain.Entities.Evidence> Evidence => Set<Domain.Entities.Evidence>();
    public DbSet<Domain.Entities.RiskAssessment> RiskAssessments => Set<Domain.Entities.RiskAssessment>();
    public DbSet<Domain.Entities.ConsentRecord> ConsentRecords => Set<Domain.Entities.ConsentRecord>();
    public DbSet<Domain.Entities.CulturalNote> CulturalNotes => Set<Domain.Entities.CulturalNote>();
    public DbSet<Domain.Entities.DialectRelationship> DialectRelationships => Set<Domain.Entities.DialectRelationship>();
    public DbSet<Domain.Entities.W1HContext> W1HContexts => Set<Domain.Entities.W1HContext>();
    
    // Provenance & Versioning
    public DbSet<Domain.Entities.SourceDocument> SourceDocuments => Set<Domain.Entities.SourceDocument>();
    public DbSet<Domain.Entities.SourcePage> SourcePages => Set<Domain.Entities.SourcePage>();
    public DbSet<Domain.Entities.CatalogSource> CatalogSources => Set<Domain.Entities.CatalogSource>();
    public DbSet<Domain.Entities.CatalogVersion> CatalogVersions => Set<Domain.Entities.CatalogVersion>();
    public DbSet<Domain.Entities.CatalogRecord> CatalogRecords => Set<Domain.Entities.CatalogRecord>();
    public DbSet<Domain.Entities.LanguageVariantAutodenomination> LanguageVariantAutodenominations => Set<Domain.Entities.LanguageVariantAutodenomination>();
    public DbSet<Domain.Entities.ImportQuarantine> ImportQuarantines => Set<Domain.Entities.ImportQuarantine>();
    // ADR 0001 — one row = one verified piece of lexical-translation evidence
    public DbSet<Domain.Entities.LexicalEquivalence> LexicalEquivalences => Set<Domain.Entities.LexicalEquivalence>();

    // B10 — AI review audit (non-canonical, never promotes automatically)
    public DbSet<Domain.AiReview.AiTranslationReview> AiTranslationReviews => Set<Domain.AiReview.AiTranslationReview>();
    public DbSet<Domain.AiReview.V2Candidate> V2Candidates => Set<Domain.AiReview.V2Candidate>();

    // IUnitOfWork implementation - Repository properties (explicit interface implementation)
    Domain.Interfaces.ILanguageFamilyRepository Domain.Interfaces.IUnitOfWork.LanguageFamilies => new Repositories.LanguageFamilyRepository(this);
    Domain.Interfaces.ILanguageGroupRepository Domain.Interfaces.IUnitOfWork.LanguageGroups => new Repositories.LanguageGroupRepository(this);
    Domain.Interfaces.ILanguageVariantRepository Domain.Interfaces.IUnitOfWork.LanguageVariants => new Repositories.LanguageVariantRepository(this);
    Domain.Interfaces.ICommunityRepository Domain.Interfaces.IUnitOfWork.Communities => new Repositories.CommunityRepository(this);
    Domain.Interfaces.IRegionRepository Domain.Interfaces.IUnitOfWork.Regions => new Repositories.RegionRepository(this);
    Domain.Interfaces.ISpeakerProfileRepository Domain.Interfaces.IUnitOfWork.Speakers => new Repositories.SpeakerProfileRepository(this);
    Domain.Interfaces.ILexemeRepository Domain.Interfaces.IUnitOfWork.Lexemes => new Repositories.LexemeRepository(this);
    Domain.Interfaces.IMeaningRepository Domain.Interfaces.IUnitOfWork.Meanings => new Repositories.MeaningRepository(this);
    Domain.Interfaces.IPhraseRepository Domain.Interfaces.IUnitOfWork.Phrases => new Repositories.PhraseRepository(this);
    Domain.Interfaces.IExampleRepository Domain.Interfaces.IUnitOfWork.Examples => new Repositories.ExampleRepository(this);
    Domain.Interfaces.IPronunciationRepository Domain.Interfaces.IUnitOfWork.Pronunciations => new Repositories.PronunciationRepository(this);
    Domain.Interfaces.IAudioRecordingRepository Domain.Interfaces.IUnitOfWork.AudioRecordings => new Repositories.AudioRecordingRepository(this);
    Domain.Interfaces.IVideoRecordingRepository Domain.Interfaces.IUnitOfWork.VideoRecordings => new Repositories.VideoRecordingRepository(this);
    Domain.Interfaces.IGrammarRuleRepository Domain.Interfaces.IUnitOfWork.GrammarRules => new Repositories.GrammarRuleRepository(this);
    Domain.Interfaces.IWritingSystemRepository Domain.Interfaces.IUnitOfWork.WritingSystems => new Repositories.WritingSystemRepository(this);
    Domain.Interfaces.IOrthographyRepository Domain.Interfaces.IUnitOfWork.Orthographies => new Repositories.OrthographyRepository(this);
    Domain.Interfaces.ITranslationRepository Domain.Interfaces.IUnitOfWork.Translations => new Repositories.TranslationRepository(this);
    Domain.Interfaces.ISourceRepository Domain.Interfaces.IUnitOfWork.Sources => new Repositories.SourceRepository(this);
    Domain.Interfaces.IEvidenceRepository Domain.Interfaces.IUnitOfWork.Evidence => new Repositories.EvidenceRepository(this);
    Domain.Interfaces.IRiskAssessmentRepository Domain.Interfaces.IUnitOfWork.RiskAssessments => new Repositories.RiskAssessmentRepository(this);
    Domain.Interfaces.IConsentRecordRepository Domain.Interfaces.IUnitOfWork.ConsentRecords => new Repositories.ConsentRecordRepository(this);
    Domain.Interfaces.ICulturalNoteRepository Domain.Interfaces.IUnitOfWork.CulturalNotes => new Repositories.CulturalNoteRepository(this);
    Domain.Interfaces.IDialectRelationshipRepository Domain.Interfaces.IUnitOfWork.DialectRelationships => new Repositories.DialectRelationshipRepository(this);
    Domain.Interfaces.IW1HContextRepository Domain.Interfaces.IUnitOfWork.W1HContexts => new Repositories.W1HContextRepository(this);

    private IDbContextTransaction? _currentTransaction;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AtlasBuhoDbContext).Assembly);

        // Global query filters for soft delete patterns
        modelBuilder.Entity<Domain.Entities.AudioRecording>().HasQueryFilter(e => !e.RemovalRequested);
        modelBuilder.Entity<Domain.Entities.VideoRecording>().HasQueryFilter(e => !e.RemovalRequested);
        modelBuilder.Entity<Domain.Entities.ConsentRecord>().HasQueryFilter(e => !e.IsRevoked);
        modelBuilder.Entity<Domain.Entities.RiskAssessment>().HasQueryFilter(e => e.IsCurrent);
    }

    public new async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _currentTransaction = await Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.CommitAsync(cancellationToken);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public override void Dispose()
    {
        _currentTransaction?.Dispose();
        base.Dispose();
    }

    public override async ValueTask DisposeAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
        await base.DisposeAsync();
    }
}