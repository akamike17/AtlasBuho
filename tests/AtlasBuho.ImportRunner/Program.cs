using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AtlasBuho.Data;
using AtlasBuho.Data.Seeding;
using AtlasBuho.Data.Translation;
using AtlasBuho.Application.Translation;
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
    Console.WriteLine("ERROR: Connection string 'MySQL' not found in User Secrets.");
    Console.WriteLine("Run: dotnet user-secrets set ConnectionStrings:MySQL \"Server=localhost;Database=atlasbuho;User=Admin;Password=YOUR_PASSWORD\"");
    Environment.Exit(1);
}

services.AddDbContext<AtlasBuhoDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

services.AddScoped<ICatalogImporter, InaliCatalogImporter>();

var provider = services.BuildServiceProvider();

using var scope = provider.CreateScope();
var importer = scope.ServiceProvider.GetRequiredService<ICatalogImporter>();

// 6B.md dogfood mode: "--translate <source> <target> <text...>" runs the dictionary engine
// against the seeded database and prints the structured result (no AI involved).
if (args.Length >= 4 && args[0] == "--translate")
{
    var sp = provider.GetRequiredService<AtlasBuhoDbContext>();
    var engine = new DictionaryTranslationEngine(sp);
    var tr = await engine.TranslateAsync(
        new TranslationRequest(args[1], args[2], string.Join(" ", args.Skip(3))));
    Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(tr,
        new System.Text.Json.JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping }));
    return;
}

Console.WriteLine("Starting INALI catalog import...");

var result = await importer.ImportInaliCatalogAsync();
Console.WriteLine($"Success: {result.Success}");
Console.WriteLine($"Families: {result.FamiliesImported}");
Console.WriteLine($"Groups: {result.GroupsImported}");
Console.WriteLine($"Variants: {result.VariantsImported}");
Console.WriteLine($"Autodenominations: {result.AutodenominationsImported}");

if (!result.Success)
{
    Console.WriteLine($"Error: {result.ErrorMessage}");
    foreach (var err in result.ValidationErrors)
    {
        Console.WriteLine($"Validation: {err}");
    }
    Environment.Exit(1);
}

Console.WriteLine("Import completed successfully!");