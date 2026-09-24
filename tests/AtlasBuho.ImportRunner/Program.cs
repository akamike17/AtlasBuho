using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AtlasBuho.Data;
using AtlasBuho.Data.Seeding;
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