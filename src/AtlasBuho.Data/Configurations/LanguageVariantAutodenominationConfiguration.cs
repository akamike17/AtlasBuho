using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Configurations;

public class LanguageVariantAutodenominationConfiguration : IEntityTypeConfiguration<Domain.Entities.LanguageVariantAutodenomination>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.LanguageVariantAutodenomination> builder)
    {
        builder.ToTable("LanguageVariantAutodenominations");
        
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.LanguageVariantId).HasColumnName("LanguageVariantId").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.Autodenomination).HasColumnName("Autodenomination").HasMaxLength(500).IsRequired();
        builder.Property(e => e.SpanishName).HasColumnName("SpanishName").HasMaxLength(500);
        builder.Property(e => e.Agrupacion).HasColumnName("Agrupacion").HasMaxLength(500);
        builder.Property(e => e.Familia).HasColumnName("Familia").HasMaxLength(500);
        builder.Property(e => e.SourcePage).HasColumnName("SourcePage").IsRequired();
        builder.Property(e => e.SourceSection).HasColumnName("SourceSection").HasMaxLength(500);
        builder.Property(e => e.SourceDocumentId).HasColumnName("SourceDocumentId").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.SourceHash).HasColumnName("SourceHash").HasMaxLength(64).IsRequired();
        builder.Property(e => e.ExtractedAt).HasColumnName("ExtractedAt").IsRequired();
        builder.Property(e => e.ParserVersion).HasColumnName("ParserVersion").HasMaxLength(50).IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").IsRequired();
        
        // Unique index includes SourcePage to allow same autodenomination for same variant on different pages
        builder.HasIndex(e => new { e.LanguageVariantId, e.Autodenomination, e.SourcePage }).IsUnique()
            .HasDatabaseName("IX_LanguageVariantAutodenominations_Variant_Autodenom_Page");
        builder.HasIndex(e => e.SourceDocumentId).HasDatabaseName("IX_LanguageVariantAutodenominations_SourceDocument");
        builder.HasIndex(e => e.SourcePage).HasDatabaseName("IX_LanguageVariantAutodenominations_SourcePage");
        
        builder.HasOne(e => e.LanguageVariant)
            .WithMany(e => e.Autodenominations)
            .HasForeignKey(e => e.LanguageVariantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}