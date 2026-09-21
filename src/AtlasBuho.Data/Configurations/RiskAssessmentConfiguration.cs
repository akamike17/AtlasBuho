using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Configurations;

public class RiskAssessmentConfiguration : IEntityTypeConfiguration<Domain.Entities.RiskAssessment>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.RiskAssessment> builder)
    {
        builder.ToTable("RiskAssessments");
        
        builder.Property(e => e.Id).HasColumnName("Id").IsRequired();
        builder.Property(e => e.LanguageVariantId).HasColumnName("LanguageVariantId").IsRequired();
        builder.Property(e => e.RiskLevel).HasColumnName("RiskLevel").HasConversion<int>().IsRequired();
        builder.Property(e => e.Source).HasColumnName("Source").HasMaxLength(200);
        builder.Property(e => e.Year).HasColumnName("Year");
        builder.Property(e => e.Methodology).HasColumnName("Methodology").HasMaxLength(2000);
        builder.Property(e => e.Population).HasColumnName("Population");
        builder.Property(e => e.Criterion).HasColumnName("Criterion").HasMaxLength(2000);
        builder.Property(e => e.Notes).HasColumnName("Notes").HasMaxLength(2000);
        builder.Property(e => e.IsCurrent).HasColumnName("IsCurrent").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").IsRequired();
        builder.HasIndex(e => e.LanguageVariantId).HasDatabaseName("IX_RiskAssessments_VariantId");
        builder.HasIndex(e => e.RiskLevel).HasDatabaseName("IX_RiskAssessments_Level");
        builder.HasIndex(e => new { e.LanguageVariantId, e.IsCurrent }).HasDatabaseName("IX_RiskAssessments_Variant_Current");
        
        builder.HasOne<Domain.Entities.LanguageVariant>()
            .WithMany(e => e.RiskAssessments)
            .HasForeignKey(e => e.LanguageVariantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
