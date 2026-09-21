namespace AtlasBuho.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Sqlite;

public class AtlasBuhoDbContextFactory : IDesignTimeDbContextFactory<AtlasBuhoDbContext>
{
    public AtlasBuhoDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AtlasBuhoDbContext>();
        optionsBuilder.UseSqlite("Data Source=atlasbuho_migrations.db");
        return new AtlasBuhoDbContext(optionsBuilder.Options);
    }
}