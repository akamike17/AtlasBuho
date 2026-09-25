using AtlasBuho.Domain.Entities;
using AtlasBuho.Data;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Xunit;

namespace AtlasBuho.Tests.Integration;

/// <summary>
/// STEP 3 — Tests físicos contra MySQL real para verificar los triggers
/// instalados por la migración real (20260925033847_LexicalEquivalenceModel).
/// La base de datos se prepara mediante Database.Migrate(), nunca creando triggers in-test.
/// Cada test usa IDs únicos para evitar dependencias entre pruebas.
/// </summary>
public class TriggerImmutabilityTests : IClassFixture<TriggerImmutabilityTests.DatabaseFixture>
{
    private readonly AtlasBuhoDbContext _context;

    public class DatabaseFixture : IDisposable
    {
        public AtlasBuhoDbContext Context { get; }

        public DatabaseFixture()
        {
            var password = Environment.GetEnvironmentVariable("ATLASBUHO_TEST_DB_PASSWORD")
                ?? throw new InvalidOperationException(
                    "Set ATLASBUHO_TEST_DB_PASSWORD environment variable before running integration tests.");

            var dbName = $"atlasbuho_test_triggers_{Guid.NewGuid():N}";
            var connectionString = $"Server=127.0.0.1;Database={dbName};User=Admin;Password={password};";

            var options = new DbContextOptionsBuilder<AtlasBuhoDbContext>()
                .UseMySql(connectionString, ServerVersion.Parse("8.0.46-mysql"))
                .Options;

            Context = new AtlasBuhoDbContext(options);
            Context.Database.Migrate();
        }

        public void Dispose()
        {
            Context.Database.EnsureDeleted();
            Context.Dispose();
        }
    }

    public TriggerImmutabilityTests(DatabaseFixture fixture)
    {
        _context = fixture.Context;
    }

    private Guid CreateCompletedCatalogVersion()
    {
        var catalogSourceId = Guid.NewGuid();
        var catalogVersionId = Guid.NewGuid();

        _context.Database.ExecuteSqlRaw(
            @"INSERT INTO CatalogSources (Id, Name, Description, BaseUrl, IsActive, CreatedAt, UpdatedAt)
              VALUES (@p0, @p1, 'Test', 'http://test.local', 1, UTC_TIMESTAMP(6), UTC_TIMESTAMP(6))",
            catalogSourceId.ToString(),
            $"Test Source {Guid.NewGuid():N}");

        _context.Database.ExecuteSqlRaw(
            @"INSERT INTO CatalogVersions (
                Id, CatalogSourceId, VersionNumber, SourceDocumentHash, RetrievedAt,
                ImportedAt, FamilyCount, GroupCount, VariantCount, AutodenominationCount,
                ParserVersion, ImportStatus, CreatedAt
              ) VALUES (
                @p0, @p1, 'v1.test', 'hash', UTC_TIMESTAMP(6),
                UTC_TIMESTAMP(6), 0, 0, 0, 0,
                '1.0.0', 'Completed', UTC_TIMESTAMP(6)
              )",
            catalogVersionId.ToString(),
            catalogSourceId.ToString());

        return catalogVersionId;
    }

    private Guid CreateLexicalEquivalence(Guid catalogVersionId)
    {
        var lexicalEquivalenceId = Guid.NewGuid();
        var sourceLexemeId = Guid.NewGuid();
        var sourceId = Guid.NewGuid();
        var languageVariantId = Guid.NewGuid();
        var languageGroupId = Guid.NewGuid();
        var languageFamilyId = Guid.NewGuid();

        _context.Database.ExecuteSqlRaw(
            @"INSERT INTO LanguageFamilies (Id, Name, CreatedAt, UpdatedAt)
              VALUES (@p0, @p1, UTC_TIMESTAMP(6), UTC_TIMESTAMP(6))",
            languageFamilyId.ToString(),
            $"Test Family {Guid.NewGuid():N}");

        _context.Database.ExecuteSqlRaw(
            @"INSERT INTO LanguageGroups (Id, LanguageFamilyId, Name, CreatedAt, UpdatedAt)
              VALUES (@p0, @p1, @p2, UTC_TIMESTAMP(6), UTC_TIMESTAMP(6))",
            languageGroupId.ToString(),
            languageFamilyId.ToString(),
            $"Test Group {Guid.NewGuid():N}");

        _context.Database.ExecuteSqlRaw(
            @"INSERT INTO LanguageVariants (Id, LanguageGroupId, Name, CreatedAt, UpdatedAt)
              VALUES (@p0, @p1, @p2, UTC_TIMESTAMP(6), UTC_TIMESTAMP(6))",
            languageVariantId.ToString(),
            languageGroupId.ToString(),
            $"Test Variant {Guid.NewGuid():N}");

        _context.Database.ExecuteSqlRaw(
            @"INSERT INTO Lexemes (Id, LanguageVariantId, CanonicalForm, VerificationStatus, Confidence, CreatedAt, UpdatedAt)
              VALUES (@p0, @p1, 'test', 1, 1.0, UTC_TIMESTAMP(6), UTC_TIMESTAMP(6))",
            sourceLexemeId.ToString(),
            languageVariantId.ToString());

        _context.Database.ExecuteSqlRaw(
            @"INSERT INTO Sources (Id, Name, Level, IsActive, CreatedAt, UpdatedAt)
              VALUES (@p0, @p1, 1, 1, UTC_TIMESTAMP(6), UTC_TIMESTAMP(6))",
            sourceId.ToString(),
            $"Test Source {Guid.NewGuid():N}");

        _context.Database.ExecuteSqlRaw(
            @"INSERT INTO LexicalEquivalences (
                Id, SourceLexemeId, TargetLanguage, TargetText, IsCanonical,
                VerificationStatus, SourceId, CatalogVersionId, RowHash, CreatedAt
              ) VALUES (
                @p0, @p1, 'es', 'test', 1,
                1, @p2, @p3, 'hash123', UTC_TIMESTAMP(6)
              )",
            lexicalEquivalenceId.ToString(),
            sourceLexemeId.ToString(),
            sourceId.ToString(),
            catalogVersionId.ToString());

        return lexicalEquivalenceId;
    }

    [Fact]
    public void CompletedCatalogVersion_Update_IsRejectedByTrigger()
    {
        var catalogVersionId = CreateCompletedCatalogVersion();

        var ex = Assert.Throws<MySqlException>(() =>
            _context.Database.ExecuteSqlRaw(
                "UPDATE CatalogVersions SET ImportStatus = 'Archived' WHERE Id = @p0",
                catalogVersionId.ToString()));

        Assert.Equal("45000", ex.SqlState);
    }

    [Fact]
    public void CompletedCatalogVersion_Delete_IsRejectedByTrigger()
    {
        var catalogVersionId = CreateCompletedCatalogVersion();

        var ex = Assert.Throws<MySqlException>(() =>
            _context.Database.ExecuteSqlRaw(
                "DELETE FROM CatalogVersions WHERE Id = @p0",
                catalogVersionId.ToString()));

        Assert.Equal("45000", ex.SqlState);
    }

    [Fact]
    public void LexicalEquivalence_Update_WhenCatalogCompleted_IsRejectedByTrigger()
    {
        var catalogVersionId = CreateCompletedCatalogVersion();
        var lexicalEquivalenceId = CreateLexicalEquivalence(catalogVersionId);

        var ex = Assert.Throws<MySqlException>(() =>
            _context.Database.ExecuteSqlRaw(
                "UPDATE LexicalEquivalences SET TargetText = 'updated' WHERE Id = @p0",
                lexicalEquivalenceId.ToString()));

        Assert.Equal("45000", ex.SqlState);
    }

    [Fact]
    public void LexicalEquivalence_Delete_WhenCatalogCompleted_IsRejectedByTrigger()
    {
        var catalogVersionId = CreateCompletedCatalogVersion();
        var lexicalEquivalenceId = CreateLexicalEquivalence(catalogVersionId);

        var ex = Assert.Throws<MySqlException>(() =>
            _context.Database.ExecuteSqlRaw(
                "DELETE FROM LexicalEquivalences WHERE Id = @p0",
                lexicalEquivalenceId.ToString()));

        Assert.Equal("45000", ex.SqlState);
    }
}
