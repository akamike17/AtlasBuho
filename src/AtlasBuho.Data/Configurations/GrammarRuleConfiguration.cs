using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Configurations;

public class GrammarRuleConfiguration : IEntityTypeConfiguration<Domain.Entities.GrammarRule>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.GrammarRule> builder)
    {
        builder.ToTable("GrammarRules");
        
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.LanguageVariantId).HasColumnName("LanguageVariantId").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.Category).HasColumnName("Category").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Subcategory).HasColumnName("Subcategory").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Name).HasColumnName("Name").HasMaxLength(200).IsRequired();
        builder.Property(e => e.Description).HasColumnName("Description").HasColumnType("TEXT").IsRequired();
        builder.Property(e => e.Pattern).HasColumnName("Pattern").HasColumnType("TEXT");
        builder.Property(e => e.Examples).HasColumnName("Examples").HasColumnType("TEXT");
        builder.Property(e => e.Notes).HasColumnName("Notes").HasColumnType("TEXT");
        builder.Property(e => e.Source).HasColumnName("Source").HasMaxLength(500);
        builder.Property(e => e.VerificationStatus).HasColumnName("VerificationStatus").HasConversion<int>().IsRequired();
        builder.Property(e => e.Confidence).HasColumnName("Confidence").HasColumnType("decimal(3,2)").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").IsRequired();
        builder.HasIndex(e => e.LanguageVariantId).HasDatabaseName("IX_GrammarRules_VariantId");
        builder.HasIndex(e => new { e.LanguageVariantId, e.Category }).HasDatabaseName("IX_GrammarRules_Variant_Category");
        
        builder.HasOne<Domain.Entities.LanguageVariant>()
            .WithMany()
            .HasForeignKey(e => e.LanguageVariantId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(e => e.Evidence)
            .WithOne()
            .HasForeignKey("EntityId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
