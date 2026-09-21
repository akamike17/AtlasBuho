using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Configurations;

public class WritingSystemConfiguration : IEntityTypeConfiguration<Domain.Entities.WritingSystem>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.WritingSystem> builder)
    {
        builder.ToTable("WritingSystems");
        
        builder.Property(e => e.Id).HasColumnName("Id").IsRequired();
        builder.Property(e => e.LanguageVariantId).HasColumnName("LanguageVariantId").IsRequired();
        builder.Property(e => e.Name).HasColumnName("Name").HasMaxLength(200).IsRequired();
        builder.Property(e => e.Script).HasColumnName("Script").HasMaxLength(100);
        builder.Property(e => e.Description).HasColumnName("Description").HasMaxLength(2000);
        builder.Property(e => e.OrthographyRules).HasColumnName("OrthographyRules").HasMaxLength(5000);
        builder.Property(e => e.Source).HasColumnName("Source").HasMaxLength(500);
        builder.Property(e => e.IsOfficial).HasColumnName("IsOfficial").IsRequired();
        builder.Property(e => e.IsActive).HasColumnName("IsActive").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").IsRequired();
        builder.HasIndex(e => e.LanguageVariantId).HasDatabaseName("IX_WritingSystems_VariantId");
        builder.HasIndex(e => new { e.LanguageVariantId, e.IsOfficial }).HasDatabaseName("IX_WritingSystems_Variant_Official");
        
        builder.HasOne<Domain.Entities.LanguageVariant>()
            .WithMany()
            .HasForeignKey(e => e.LanguageVariantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
