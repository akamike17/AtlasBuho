using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Configurations;

public class CatalogSourceConfiguration : IEntityTypeConfiguration<Domain.Entities.CatalogSource>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.CatalogSource> builder)
    {
        builder.ToTable("CatalogSources");
        
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.Name).HasColumnName("Name").HasMaxLength(200).IsRequired();
        builder.Property(e => e.Description).HasColumnName("Description").HasColumnType("TEXT");
        builder.Property(e => e.BaseUrl).HasColumnName("BaseUrl").HasMaxLength(2000).IsRequired();
        builder.Property(e => e.IsActive).HasColumnName("IsActive").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").IsRequired();
        builder.HasIndex(e => e.Name).IsUnique().HasDatabaseName("IX_CatalogSources_Name");
        builder.HasIndex(e => e.BaseUrl).HasDatabaseName("IX_CatalogSources_BaseUrl").HasAnnotation("MySql:IndexPrefixLength", new[] { 700 });
        
        builder.HasMany(e => e.Versions)
            .WithOne(e => e.CatalogSource)
            .HasForeignKey(e => e.CatalogSourceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}