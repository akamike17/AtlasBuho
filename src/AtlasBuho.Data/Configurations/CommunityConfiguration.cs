using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Configurations;

public class CommunityConfiguration : IEntityTypeConfiguration<Domain.Entities.Community>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Community> builder)
    {
        builder.ToTable("Communities");
        
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.LanguageVariantId).HasColumnName("LanguageVariantId").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.RegionId).HasColumnName("RegionId");
        builder.Property(e => e.Name).HasColumnName("Name").HasMaxLength(200).IsRequired();
        builder.Property(e => e.Autodenomination).HasColumnName("Autodenomination").HasMaxLength(200);
        builder.Property(e => e.State).HasColumnName("State").HasMaxLength(100);
        builder.Property(e => e.Municipality).HasColumnName("Municipality").HasMaxLength(100);
        builder.Property(e => e.Locality).HasColumnName("Locality").HasMaxLength(100);
        builder.Property(e => e.Latitude).HasColumnName("Latitude").HasColumnType("decimal(10,8)");
        builder.Property(e => e.Longitude).HasColumnName("Longitude").HasColumnType("decimal(11,8)");
        builder.Property(e => e.Population).HasColumnName("Population");
        builder.Property(e => e.SpeakerCount).HasColumnName("SpeakerCount");
        builder.Property(e => e.Source).HasColumnName("Source").HasMaxLength(500);
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").IsRequired();
        builder.HasIndex(e => e.LanguageVariantId).HasDatabaseName("IX_Communities_VariantId");
        builder.HasIndex(e => e.RegionId).HasDatabaseName("IX_Communities_RegionId");
        builder.HasIndex(e => e.State).HasDatabaseName("IX_Communities_State");
        
        builder.HasOne<Domain.Entities.LanguageVariant>()
            .WithMany(e => e.Communities)
            .HasForeignKey(e => e.LanguageVariantId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne<Domain.Entities.Region>()
            .WithMany(e => e.Communities)
            .HasForeignKey(e => e.RegionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
