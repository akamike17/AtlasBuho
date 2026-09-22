using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Configurations;

public class RegionConfiguration : IEntityTypeConfiguration<Domain.Entities.Region>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Region> builder)
    {
        builder.ToTable("Regions");
        
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.Name).HasColumnName("Name").HasMaxLength(200).IsRequired();
        builder.Property(e => e.Description).HasColumnName("Description").HasColumnType("TEXT");
        builder.Property(e => e.Country).HasColumnName("Country").HasMaxLength(100);
        builder.Property(e => e.Source).HasColumnName("Source").HasMaxLength(500);
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").IsRequired();
        builder.HasIndex(e => e.Name).IsUnique().HasDatabaseName("IX_Regions_Name");
        builder.HasIndex(e => e.Country).HasDatabaseName("IX_Regions_Country");
        
        builder.HasMany(e => e.Communities)
            .WithOne()
            .HasForeignKey("RegionId")
            .OnDelete(DeleteBehavior.SetNull);
    }
}
