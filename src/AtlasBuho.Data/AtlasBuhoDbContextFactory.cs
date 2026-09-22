namespace AtlasBuho.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;

public class AtlasBuhoDbContextFactory : IDesignTimeDbContextFactory<AtlasBuhoDbContext>
{
    public AtlasBuhoDbContext CreateDbContext(string[] args)
    {
        // Use current directory for design-time operations (where .csproj is)
        var projectDir = Directory.GetCurrentDirectory();
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(projectDir)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddUserSecrets<AtlasBuhoDbContextFactory>()
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("MySQL")
            ?? throw new InvalidOperationException("Connection string 'MySQL' not found. Set it via user-secrets or environment variables.");

        var optionsBuilder = new DbContextOptionsBuilder<AtlasBuhoDbContext>();
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        return new AtlasBuhoDbContext(optionsBuilder.Options);
    }
}