using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Configurations;

public class LanguageVariantConfiguration : IEntityTypeConfiguration<Domain.Entities.LanguageVariant>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.LanguageVariant> builder)
    {
        builder.ToTable("LanguageVariants");
        
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.LanguageGroupId).HasColumnName("LanguageGroupId").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.Name).HasColumnName("Name").HasMaxLength(500).IsRequired();
        builder.Property(e => e.Autodenomination).HasColumnName("Autodenomination").HasMaxLength(200);
        builder.Property(e => e.Iso639_3Code).HasColumnName("Iso639_3Code").HasMaxLength(10);
        builder.Property(e => e.InaliCode).HasColumnName("InaliCode").HasMaxLength(50);
        builder.Property(e => e.Description).HasColumnName("Description").HasColumnType("TEXT");
        builder.Property(e => e.WritingSystem).HasColumnName("WritingSystem").HasMaxLength(200);
        builder.Property(e => e.Orthography).HasColumnName("Orthography").HasMaxLength(200);
        builder.Property(e => e.Source).HasColumnName("Source").HasMaxLength(500);
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").IsRequired();
        builder.HasIndex(e => new { e.LanguageGroupId, e.Name }).IsUnique().HasDatabaseName("IX_LanguageVariants_Group_Name");
        builder.HasIndex(e => e.LanguageGroupId).HasDatabaseName("IX_LanguageVariants_GroupId");
        builder.HasIndex(e => e.Iso639_3Code).HasDatabaseName("IX_LanguageVariants_IsoCode");
        builder.HasIndex(e => e.InaliCode).HasDatabaseName("IX_LanguageVariants_InaliCode");
        
        builder.HasOne<Domain.Entities.LanguageGroup>()
            .WithMany(e => e.LanguageVariants)
            .HasForeignKey(e => e.LanguageGroupId)
            .OnDelete(DeleteBehavior.Cascade);
          
        builder.HasMany(e => e.Communities)
            .WithOne()
            .HasForeignKey("LanguageVariantId")
            .OnDelete(DeleteBehavior.Cascade);
          
        builder.HasMany(e => e.Lexemes)
            .WithOne()
            .HasForeignKey("LanguageVariantId")
            .OnDelete(DeleteBehavior.Cascade);
          
        builder.HasMany(e => e.Speakers)
            .WithOne()
            .HasForeignKey("LanguageVariantId")
            .OnDelete(DeleteBehavior.Cascade);
          
        builder.HasMany(e => e.RiskAssessments)
            .WithOne()
            .HasForeignKey("LanguageVariantId")
            .OnDelete(DeleteBehavior.Cascade);
          
        builder.HasMany(e => e.DialectRelationships)
            .WithOne()
            .HasForeignKey("SourceLanguageVariantId")
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(e => e.Autodenominations)
            .WithOne(e => e.LanguageVariant)
            .HasForeignKey(e => e.LanguageVariantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
