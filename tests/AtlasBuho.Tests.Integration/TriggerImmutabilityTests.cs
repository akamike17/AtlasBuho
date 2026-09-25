using AtlasBuho.Domain.Entities;
using AtlasBuho.Data;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Xunit;

namespace AtlasBuho.Tests.Integration;

/// <summary>
/// STEP 3 — Tests físicos contra MySQL real para verificar el comportamiento de los triggers.
/// Estos tests usan una base de datos temporal 'atlasbuho_test_triggers' creada manualmente.
/// </summary>
public class TriggerImmutabilityTests : IDisposable
{
    private readonly AtlasBuhoDbContext _context;
    private readonly string _connectionString = "Server=127.0.0.1;Database=atlasbuho_test_triggers;User=Admin;Password=RenacerGood17;";
    private readonly Guid _testCatalogVersionId = Guid.Parse("a1b2c3d4-e5f6-7890-1234-567890abcdef");
    private readonly Guid _testLexicalEquivalenceId = Guid.Parse("c3d4e5f6-a7b8-9012-3456-7890abcdef12");

    public TriggerImmutabilityTests()
    {
        var options = new DbContextOptionsBuilder<AtlasBuhoDbContext>()
            .UseMySql(_connectionString, ServerVersion.Parse("8.0.46-mysql"))
            .Options;

        _context = new AtlasBuhoDbContext(options);

        // Asegurar que la base de datos de prueba esté limpia y tenga datos de prueba
        InitializeTestDatabase();
    }

    private void InitializeTestDatabase()
    {
        // Paso 1: Eliminar triggers existentes para permitir limpieza
        _context.Database.ExecuteSqlRaw("DROP TRIGGER IF EXISTS trg_catalogversions_prevent_update_completed");
        _context.Database.ExecuteSqlRaw("DROP TRIGGER IF EXISTS trg_catalogversions_prevent_delete_completed");
        _context.Database.ExecuteSqlRaw("DROP TRIGGER IF EXISTS trg_lexeq_block_mutation_completed");
        _context.Database.ExecuteSqlRaw("DROP TRIGGER IF EXISTS trg_lexeq_block_delete_completed");

        // Paso 2: Limpiar datos de ejecuciones previas (sin triggers activos)
        _context.Database.ExecuteSqlRaw("DELETE FROM LexicalEquivalences WHERE Id = @p0", _testLexicalEquivalenceId.ToString());
        _context.Database.ExecuteSqlRaw("DELETE FROM CatalogVersions WHERE Id = @p0", _testCatalogVersionId.ToString());

        // Paso 3: Crear tablas si no existen
        _context.Database.ExecuteSqlRaw(@"
            CREATE TABLE IF NOT EXISTS CatalogVersions (
                Id CHAR(36) NOT NULL,
                ImportStatus VARCHAR(50) NOT NULL,
                PRIMARY KEY (Id)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
        ");

        _context.Database.ExecuteSqlRaw(@"
            CREATE TABLE IF NOT EXISTS LexicalEquivalences (
                Id CHAR(36) NOT NULL,
                SourceLexemeId CHAR(36) NOT NULL,
                TargetLanguage VARCHAR(8) NOT NULL,
                TargetText VARCHAR(500) NOT NULL,
                IsCanonical TINYINT(1) NOT NULL,
                VerificationStatus INT NOT NULL,
                SourceId CHAR(36) NOT NULL,
                CatalogVersionId CHAR(36) NOT NULL,
                RowHash VARCHAR(64) NOT NULL,
                CreatedAt DATETIME(6) NOT NULL,
                PRIMARY KEY (Id),
                CONSTRAINT FK_TestLexicalEquivalence_CatalogVersionId FOREIGN KEY (CatalogVersionId) REFERENCES CatalogVersions(Id) ON DELETE RESTRICT
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
        ");

        // Crear triggers si no existen (simulando el comportamiento real)
        _context.Database.ExecuteSqlRaw(@"
            DROP TRIGGER IF EXISTS trg_catalogversions_prevent_update_completed;
            CREATE TRIGGER trg_catalogversions_prevent_update_completed
            BEFORE UPDATE ON CatalogVersions FOR EACH ROW
            BEGIN
                IF OLD.ImportStatus = 'Completed' THEN
                    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'CatalogVersion is immutable once Completed';
                END IF;
            END;
        ");

        _context.Database.ExecuteSqlRaw(@"
            DROP TRIGGER IF EXISTS trg_catalogversions_prevent_delete_completed;
            CREATE TRIGGER trg_catalogversions_prevent_delete_completed
            BEFORE DELETE ON CatalogVersions FOR EACH ROW
            BEGIN
                IF OLD.ImportStatus = 'Completed' THEN
                    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'CatalogVersion is immutable once Completed';
                END IF;
            END;
        ");

        _context.Database.ExecuteSqlRaw(@"
            DROP TRIGGER IF EXISTS trg_lexeq_block_mutation_completed;
            CREATE TRIGGER trg_lexeq_block_mutation_completed
            BEFORE UPDATE ON LexicalEquivalences FOR EACH ROW
            BEGIN
                IF EXISTS (SELECT 1 FROM CatalogVersions cv WHERE cv.Id = OLD.CatalogVersionId AND cv.ImportStatus = 'Completed') THEN
                    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'LexicalEquivalence evidence is immutable once its CatalogVersion is Completed';
                END IF;
            END;
        ");

        _context.Database.ExecuteSqlRaw(@"
            DROP TRIGGER IF EXISTS trg_lexeq_block_delete_completed;
            CREATE TRIGGER trg_lexeq_block_delete_completed
            BEFORE DELETE ON LexicalEquivalences FOR EACH ROW
            BEGIN
                IF EXISTS (SELECT 1 FROM CatalogVersions cv WHERE cv.Id = OLD.CatalogVersionId AND cv.ImportStatus = 'Completed') THEN
                    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'LexicalEquivalence evidence is immutable once its CatalogVersion is Completed';
                END IF;
            END;
        ");

        // Insertar datos de prueba via SQL directo (evitar ctors de dominio)
        _context.Database.ExecuteSqlRaw(
            "INSERT IGNORE INTO CatalogVersions (Id, ImportStatus) VALUES (@p0, 'Completed')",
            _testCatalogVersionId.ToString());

        _context.Database.ExecuteSqlRaw(@"
            INSERT IGNORE INTO LexicalEquivalences (Id, SourceLexemeId, TargetLanguage, TargetText, IsCanonical, VerificationStatus, SourceId, CatalogVersionId, RowHash, CreatedAt)
            VALUES (@p0, @p1, 'es', 'test', 1, 1, @p2, @p3, 'hash123', UTC_TIMESTAMP(6))",
            _testLexicalEquivalenceId.ToString(),
            Guid.Parse("d4e5f6a7-b8c9-0123-4567-890abcdef123").ToString(),
            Guid.Parse("d4e5f6a7-b8c9-0123-4567-890abcdef321").ToString(),
            _testCatalogVersionId.ToString());
    }

    [Fact]
    public void CompletedCatalogVersion_Update_IsRejectedByTrigger()
    {
        // STEP 3 Test 1: Intentar actualizar una Completed CatalogVersion
        var ex = Assert.Throws<MySqlException>(() =>
            _context.Database.ExecuteSqlRaw(
                "UPDATE CatalogVersions SET ImportStatus = 'Archived' WHERE Id = @p0",
                _testCatalogVersionId.ToString()));

        Assert.Equal("45000", ex.SqlState);
    }

    [Fact]
    public void CompletedCatalogVersion_Delete_IsRejectedByTrigger()
    {
        // STEP 3 Test 2: Intentar eliminar una Completed CatalogVersion
        var ex = Assert.Throws<MySqlException>(() =>
            _context.Database.ExecuteSqlRaw(
                "DELETE FROM CatalogVersions WHERE Id = @p0",
                _testCatalogVersionId.ToString()));

        Assert.Equal("45000", ex.SqlState);
    }

    [Fact]
    public void LexicalEquivalence_Update_WhenCatalogCompleted_IsRejectedByTrigger()
    {
        // STEP 3 Test 3: Intentar actualizar una LexicalEquivalence ligada a un Completed CatalogVersion
        var ex = Assert.Throws<MySqlException>(() =>
            _context.Database.ExecuteSqlRaw(
                "UPDATE LexicalEquivalences SET TargetText = 'updated' WHERE Id = @p0",
                _testLexicalEquivalenceId.ToString()));

        Assert.Equal("45000", ex.SqlState);
    }

    [Fact]
    public void LexicalEquivalence_Delete_WhenCatalogCompleted_IsRejectedByTrigger()
    {
        // STEP 3 Test 4: Intentar eliminar una LexicalEquivalence ligada a un Completed CatalogVersion
        var ex = Assert.Throws<MySqlException>(() =>
            _context.Database.ExecuteSqlRaw(
                "DELETE FROM LexicalEquivalences WHERE Id = @p0",
                _testLexicalEquivalenceId.ToString()));

        Assert.Equal("45000", ex.SqlState);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
