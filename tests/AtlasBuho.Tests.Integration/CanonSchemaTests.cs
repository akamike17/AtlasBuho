using AtlasBuho.Data;
using AtlasBuho.Domain.Entities;
using AtlasBuho.Domain.Entities.Design;
using EstadoVerificacion = AtlasBuho.Domain.Entities.Design.EstadoVerificacion;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AtlasBuho.Tests.Integration;

[Collection("Database")]
public class CanonSchemaTests : IAsyncLifetime
{
    private readonly string _dbName;
    private AtlasBuhoDbContext _context = null!;

    public CanonSchemaTests()
    {
        _dbName = $"atlasbuho_test_canon_{Guid.NewGuid():N}";
    }

    public async Task InitializeAsync()
    {
        var password = Environment.GetEnvironmentVariable("ATLASBUHO_TEST_DB_PASSWORD")
            ?? throw new InvalidOperationException("Set ATLASBUHO_TEST_DB_PASSWORD");
        var connectionString = $"Server=127.0.0.1;Port=3306;Database={_dbName};User=Admin;Password={password};AllowLoadLocalInfile=true";
        var options = new DbContextOptionsBuilder<AtlasBuhoDbContext>()
            .UseMySql(connectionString, ServerVersion.Parse("8.0.46-mysql"))
            .Options;
        _context = new AtlasBuhoDbContext(options);
        await _context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
    }

    // Helper to seed a minimal parent chain: LanguageFamily -> Group -> Variant -> Source
    private async Task<(Guid famId, Guid grpId, Guid varId, Guid srcId)> SeedParents()
    {
        var fam = new LanguageFamily("FamiliaTestCanon", null, null, null);
        var grp = new LanguageGroup(fam.Id, "GrupoTestCanon", null, null, null);
        var var = new LanguageVariant(grp.Id, "VarianteTestCanon", null, null, null, null, null, null, null);
        var src = new Source("FuenteTestCanon", SourceLevel.Institutional, null, null, null, null, null, null, null, null, null, null, null, null, null);
        _context.LanguageFamilies.Add(fam);
        _context.LanguageGroups.Add(grp);
        _context.LanguageVariants.Add(var);
        _context.Sources.Add(src);
        await _context.SaveChangesAsync();
        return (fam.Id, grp.Id, var.Id, src.Id);
    }

    [Fact]
    public async Task CanonFamilia_PersistsAndLinksToLanguageFamily()
    {
        var (famId, _, _, srcId) = await SeedParents();
        var canon = new FamiliaCanon { LanguageFamilyId = famId, FuenteId = srcId, EstadoVerificacion = EstadoVerificacion.Documented };
        _context.CanonFamilias.Add(canon);
        await _context.SaveChangesAsync();
        var loaded = await _context.CanonFamilias.FindAsync(canon.Id);
        Assert.NotNull(loaded);
        Assert.Equal(famId, loaded.LanguageFamilyId);
        Assert.Equal(srcId, loaded.FuenteId);
        Assert.Equal(EstadoVerificacion.Documented, loaded.EstadoVerificacion);
    }

    [Fact]
    public async Task CanonVariante_PersistsAndLinksToLanguageVariant()
    {
        var (_, _, varId, srcId) = await SeedParents();
        var canon = new VarianteCanon { LanguageVariantId = varId, FuentePrincipalId = srcId, EstadoVerificacion = EstadoVerificacion.Verified, PaginaFuentePrincipal = "p.71" };
        _context.CanonVariantes.Add(canon);
        await _context.SaveChangesAsync();
        var loaded = await _context.CanonVariantes.FindAsync(canon.Id);
        Assert.NotNull(loaded);
        Assert.Equal(varId, loaded.LanguageVariantId);
        Assert.Equal(EstadoVerificacion.Verified, loaded.EstadoVerificacion);
    }

    [Fact]
    public async Task CanonGeografiaChain_PaisEstadoMunicipioLocalidad()
    {
        var pais = new PaisCanon { Nombre = "México", Codigo = "MEX", EstadoVerificacion = EstadoVerificacion.Verified };
        _context.CanonPaises.Add(pais);
        await _context.SaveChangesAsync();
        var estado = new EstadoCanon { PaisId = pais.Id, Nombre = "Oaxaca", Codigo = "OAX" };
        _context.CanonEstados.Add(estado);
        await _context.SaveChangesAsync();
        var muni = new MunicipioCanon { EstadoId = estado.Id, Nombre = "Miahuatlán de Porfirio Díaz", Codigo = "" };
        _context.CanonMunicipios.Add(muni);
        await _context.SaveChangesAsync();
        var loc = new LocalidadCanon { MunicipioId = muni.Id, Nombre = "Miahuatlán", Tipo = LocalidadTipo.LOCALIDAD };
        _context.CanonLocalidades.Add(loc);
        await _context.SaveChangesAsync();
        var loaded = await _context.CanonLocalidades.Include(l => l.Municipio).ThenInclude(m => m!.Estado).FirstAsync(l => l.Id == loc.Id);
        Assert.Equal("México", loaded.Municipio?.Estado?.Pais?.Nombre);
    }

    [Fact]
    public async Task CanonPoblacion_VariantePoblacion_PoblacionLocalidad()
    {
        var (_, _, varId, srcId) = await SeedParents();
        var canon = new VarianteCanon { LanguageVariantId = varId, FuentePrincipalId = srcId, EstadoVerificacion = EstadoVerificacion.Verified };
        _context.CanonVariantes.Add(canon);
        await _context.SaveChangesAsync();
        var pob = new PoblacionCanon { Nombre = "Zapoteco de San Vicente", Tipo = PoblacionTipo.PUEBLO, EstadoVerificacion = EstadoVerificacion.Documented };
        _context.CanonPoblaciones.Add(pob);
        await _context.SaveChangesAsync();
        var vp = new VariantePoblacionCanon { VarianteLinguisticaId = canon.Id, PoblacionId = pob.Id, Relacion = RelacionTipo.HABLA, FuenteId = srcId };
        _context.CanonVariantePoblacion.Add(vp);
        await _context.SaveChangesAsync();
        var loaded = await _context.CanonVariantePoblacion.FindAsync(vp.Id);
        Assert.NotNull(loaded);
        Assert.Equal(canon.Id, loaded.VarianteLinguisticaId);
    }

    [Fact]
    public async Task CanonCodigoLinguistico_UniqueConstraint()
    {
        var (_, _, varId, srcId) = await SeedParents();
        var canon = new VarianteCanon { LanguageVariantId = varId, FuentePrincipalId = srcId, EstadoVerificacion = EstadoVerificacion.Verified };
        _context.CanonVariantes.Add(canon);
        await _context.SaveChangesAsync();
        var c1 = new CodigoLinguisticoCanon { VarianteLinguisticaId = canon.Id, Sistema = CodigoSistema.ISO639_3, Codigo = "zap", FuenteId = srcId };
        _context.CanonCodigosLinguisticos.Add(c1);
        await _context.SaveChangesAsync();
        var c2 = new CodigoLinguisticoCanon { VarianteLinguisticaId = canon.Id, Sistema = CodigoSistema.ISO639_3, Codigo = "zap", FuenteId = srcId };
        _context.CanonCodigosLinguisticos.Add(c2);
        await Assert.ThrowsAsync<DbUpdateException>(() => _context.SaveChangesAsync());
    }

    [Fact]
    public async Task CanonEvidenciaVariante_FkReal_NoOrphan()
    {
        var (_, _, varId, srcId) = await SeedParents();
        var canon = new VarianteCanon { LanguageVariantId = varId, FuentePrincipalId = srcId, EstadoVerificacion = EstadoVerificacion.Verified };
        _context.CanonVariantes.Add(canon);
        await _context.SaveChangesAsync();
        var ev = new EvidenciaVarianteCanon { VarianteLinguisticaId = canon.Id, FuenteId = srcId, TextoEvidencia = "Kickapoo", Pagina = "71", EstadoVerificacion = EstadoVerificacion.Verified };
        _context.CanonEvidenciaVariante.Add(ev);
        await _context.SaveChangesAsync();
        var loaded = await _context.CanonEvidenciaVariante.FindAsync(ev.Id);
        Assert.NotNull(loaded);
        Assert.Equal(canon.Id, loaded.VarianteLinguisticaId);
        Assert.Equal(srcId, loaded.FuenteId);
    }

    [Fact]
    public async Task CanonReconciliationResult_CatalogRecordFk()
    {
        var (_, _, varId, srcId) = await SeedParents();
        var canon = new VarianteCanon { LanguageVariantId = varId, FuentePrincipalId = srcId, EstadoVerificacion = EstadoVerificacion.Verified };
        _context.CanonVariantes.Add(canon);
        await _context.SaveChangesAsync();
        var catSrc = new CatalogSource("SrcCatTest", null, null);
        var catVer = new CatalogVersion(catSrc.Id, "1.0", "hash", DateTime.UtcNow, 1, 1, 1, 1, "1.0.0", "Completed");
        var catRec = new CatalogRecord(catVer.Id, "LanguageVariant", "VarianteTestCanon", "", "", "VarianteTestCanon", null, null, null, catSrc.Id, "hash", "url", 1, "sec", DateTime.UtcNow, "1.0.0");
        _context.CatalogSources.Add(catSrc);
        _context.CatalogVersions.Add(catVer);
        _context.CatalogRecords.Add(catRec);
        await _context.SaveChangesAsync();
        var rr = new ReconciliationResultCanon { CatalogRecordId = catRec.Id, FuenteId = srcId, Resultado = ReconciliacionResultado.MATCH, VarianteLinguisticaId = canon.Id };
        _context.CanonReconciliationResults.Add(rr);
        await _context.SaveChangesAsync();
        var loaded = await _context.CanonReconciliationResults.FindAsync(rr.Id);
        Assert.NotNull(loaded);
        Assert.Equal(catRec.Id, loaded.CatalogRecordId);
        Assert.Equal(ReconciliacionResultado.MATCH, loaded.Resultado);
    }
}
