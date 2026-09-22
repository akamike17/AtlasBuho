using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Configurations;

public class OrthographyConfiguration : IEntityTypeConfiguration<Domain.Entities.Orthography>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Orthography> builder)
    {
        builder.ToTable("Orthographies");
        
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.LanguageVariantId).HasColumnName("LanguageVariantId").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.WritingSystemId).HasColumnName("WritingSystemId");
        builder.Property(e => e.Grapheme).HasColumnName("Grapheme").HasMaxLength(50).IsRequired();
        builder.Property(e => e.IpaEquivalent).HasColumnName("IpaEquivalent").HasMaxLength(100);
        builder.Property(e => e.Description).HasColumnName("Description").HasColumnType("TEXT");
        builder.Property(e => e.PositionalRules).HasColumnName("PositionalRules").HasColumnType("TEXT");
        builder.Property(e => e.Allophones).HasColumnName("Allophones").HasColumnType("TEXT");
        builder.Property(e => e.Source).HasColumnName("Source").HasMaxLength(500);
        builder.Property(e => e.VerificationStatus).HasColumnName("VerificationStatus").HasConversion<int>().IsRequired();
        builder.Property(e => e.Confidence).HasColumnName("Confidence").HasColumnType("decimal(3,2)").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").IsRequired();
        builder.HasIndex(e => e.LanguageVariantId).HasDatabaseName("IX_Orthographies_VariantId");
        builder.HasIndex(e => e.WritingSystemId).HasDatabaseName("IX_Orthographies_WritingSystemId");
        builder.HasIndex(e => new { e.LanguageVariantId, e.Grapheme }).IsUnique().HasDatabaseName("IX_Orthographies_Variant_Grapheme");
        
        builder.HasOne<Domain.Entities.LanguageVariant>()
            .WithMany()
            .HasForeignKey(e => e.LanguageVariantId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne<Domain.Entities.WritingSystem>()
            .WithMany()
            .HasForeignKey(e => e.WritingSystemId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
