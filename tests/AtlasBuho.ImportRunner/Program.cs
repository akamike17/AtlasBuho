using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AtlasBuho.Data;
using AtlasBuho.Data.Seeding;
using AtlasBuho.Data.Translation;
using AtlasBuho.Application.Translation;
using AtlasBuho.Domain.Entities;
using AtlasBuho.Domain.Entities.Design;
using EstadoVerificacion = AtlasBuho.Domain.Entities.Design.EstadoVerificacion;
using Microsoft.EntityFrameworkCore;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .Build();

var services = new ServiceCollection();
services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));

var connectionString = config.GetConnectionString("MySQL");
if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("ERROR: Connection string 'MySQL' not found.");
    Environment.Exit(1);
}

services.AddDbContext<AtlasBuhoDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

services.AddScoped<ICatalogImporter, InaliCatalogImporter>();

var provider = services.BuildServiceProvider();

// --load-canon
if (args.Length > 0 && args[0] == "--load-canon")
{
    await LoadCanon(provider.GetRequiredService<AtlasBuhoDbContext>());
    return;
}
// --verify-canon
if (args.Length > 0 && args[0] == "--verify-canon")
{
    await VerifyCanon(provider.GetRequiredService<AtlasBuhoDbContext>());
    return;
}
// --translate (existing dogfood)
if (args.Length >= 4 && args[0] == "--translate")
{
    var sp = provider.GetRequiredService<AtlasBuhoDbContext>();
    var engine = new DictionaryTranslationEngine(sp);
    var tr = await engine.TranslateAsync(
        new TranslationRequest(args[1], args[2], string.Join(" ", args.Skip(3))));
    Console.WriteLine(JsonSerializer.Serialize(tr,
        new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping }));
    return;
}

Console.WriteLine("Usage: --load-canon | --verify-canon | --translate <src> <tgt> <text>");

static async Task LoadCanon(AtlasBuhoDbContext ctx)
{
    var jsonPath = Environment.GetEnvironmentVariable("CANON_JSON_PATH")
        ?? "C:/Users/Admin/source/repos/AtlasBuho/catalogs/extracted/inali_pdf_canon_full.json";
    if (!File.Exists(jsonPath)) throw new FileNotFoundException(jsonPath);

    var json = await File.ReadAllTextAsync(jsonPath);
    var doc = JsonSerializer.Deserialize<CanonJson>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
        ?? throw new InvalidOperationException("JSON parse failed");

    Console.WriteLine($"Loaded {doc.Rows.Count} rows; expected {doc.Totals.Rows} rows, {doc.Totals.UniqueVariants} unique.");

    var catalogSource = new CatalogSource("INALI Catálogo Lenguas Indígenas 2008", "DOF 14-01-2008", "https://site.inali.gob.mx/pdf/catalogo_lenguas_indigenas.pdf");
    ctx.CatalogSources.Add(catalogSource);
    await ctx.SaveChangesAsync();

    // Usar reflexión para invocar UpdateStatus(387 lines in Domain) o bien assign via property
    var catalogVersion = new CatalogVersion(catalogSource.Id, "2008-01-14", "e38e667d024784bc84cc350f44918c1d22d998284d02e38dcf92858257947311",
        DateTime.UtcNow, doc.Totals.Familias, doc.Totals.Agrupaciones, doc.Totals.UniqueVariants, doc.Totals.UniqueVariants, "pdfplumber-v1");
    ctx.CatalogVersions.Add(catalogVersion);
    await ctx.SaveChangesAsync();

    var famCache = new Dictionary<string, LanguageFamily>();
    var grpCache = new Dictionary<string, LanguageGroup>();
    var varCache = new Dictionary<string, LanguageVariant>();
    var srcCache = new Dictionary<string, Source>();
    var canonCache = new Dictionary<string, VarianteCanon>();
    var count = 0;

    foreach (var row in doc.Rows)
    {
        if (!famCache.TryGetValue(row.Familia, out var fam))
        {
            fam = new LanguageFamily(row.Familia, null, null, null);
            ctx.LanguageFamilies.Add(fam);
            famCache[row.Familia] = fam;
        }
        if (!grpCache.TryGetValue(row.Agrupacion, out var grp))
        {
            grp = new LanguageGroup(fam.Id, row.Agrupacion, null, null, null);
            ctx.LanguageGroups.Add(grp);
            grpCache[row.Agrupacion] = grp;
        }
        if (!varCache.TryGetValue(row.Variante, out var var))
        {
            var = new LanguageVariant(grp.Id, row.Variante, row.Autodenominacion, null, null, null, null, null, null);
            ctx.LanguageVariants.Add(var);
            varCache[row.Variante] = var;
        }
        var srcKey = $"{row.Familia}|{row.Agrupacion}|{row.Variante}";
        if (!srcCache.TryGetValue(srcKey, out var src))
        {
            src = new Source($"INALI 2008 {row.Variante}", SourceLevel.Institutional, $"Página {row.Page}", "INALI",
                "https://site.inali.gob.mx/pdf/catalogo_lenguas_indigenas.pdf", null, null, null, new DateTime(2008, 1, 14),
                null, null, "INALI", null, "es", null);
            ctx.Sources.Add(src);
            srcCache[srcKey] = src;
        }
        var canon = new VarianteCanon
        {
            LanguageVariantId = var.Id,
            FuentePrincipalId = src.Id,
            PaginaFuentePrincipal = $"p.{row.Page}",
            EstadoVerificacion = EstadoVerificacion.Documented,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };
        ctx.CanonVariantes.Add(canon);
        canonCache[row.Variante] = canon;
        count++;
        if (count % 100 == 0) { await ctx.SaveChangesAsync(); Console.WriteLine($"  ...{count}"); }
    }
    await ctx.SaveChangesAsync();
    Console.WriteLine($"Inserted {count} canon rows. Familias={famCache.Count}, Grupos={grpCache.Count}, Variantes={varCache.Count}, Fuentes={srcCache.Count}");
}

static async Task VerifyCanon(AtlasBuhoDbContext ctx)
{
    var fam = await ctx.LanguageFamilies.CountAsync();
    var grp = await ctx.LanguageGroups.CountAsync();
    var var = await ctx.LanguageVariants.CountAsync();
    var canon = await ctx.CanonVariantes.CountAsync();
    var src = await ctx.Sources.CountAsync();
    Console.WriteLine($"LanguageFamilies={fam}, LanguageGroups={grp}, LanguageVariants={var}, CanonVariantes={canon}, Sources={src}");
    var vIds = (await ctx.LanguageVariants.Select(v => v.Id).ToListAsync()).ToHashSet();
    var cIds = (await ctx.CanonVariantes.Select(c => c.LanguageVariantId).ToListAsync()).ToHashSet();
    var vNoC = vIds.Except(cIds).ToList().Count;
    var cNoV = cIds.Except(vIds).ToList().Count;
    Console.WriteLine($"VariantsWithoutCanon={vNoC}, CanonWithoutVariant={cNoV}");
    if (vNoC == 0 && cNoV == 0 && var == 364 && canon == 364)
        Console.WriteLine("364/364 VERIFIED — one row per variant.");
    else
        Console.WriteLine("MISMATCH — reconciliation required.");
}

record CanonJson(string Source, string ExtractedAtUtc, List<CanonRow> Rows, CanonTotals Totals);
record CanonRow(string Familia, string Agrupacion, string Variante, string? Autodenominacion, string? GeoReference, int Page);
record CanonTotals(int Rows, int UniqueVariants, int DuplicatesByName, int Familias, int Agrupaciones, List<string> MissingAgrupaciones, int UnresolvedAgrupacion);
