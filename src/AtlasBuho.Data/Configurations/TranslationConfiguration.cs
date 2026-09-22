using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Configurations;

public class TranslationConfiguration : IEntityTypeConfiguration<Domain.Entities.Translation>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Translation> builder)
    {
        builder.ToTable("Translations");
        
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.SourceLanguageVariantId).HasColumnName("SourceLanguageVariantId").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.TargetLanguageVariantId).HasColumnName("TargetLanguageVariantId").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.SourceText).HasColumnName("SourceText").HasColumnType("TEXT").IsRequired();
        builder.Property(e => e.TargetText).HasColumnName("TargetText").HasColumnType("TEXT").IsRequired();
        builder.Property(e => e.Context).HasColumnName("Context").HasColumnType("TEXT");
        builder.Property(e => e.Source).HasColumnName("Source").HasMaxLength(500);
        builder.Property(e => e.Confidence).HasColumnName("Confidence").HasColumnType("decimal(3,2)").IsRequired();
        builder.Property(e => e.VerificationStatus).HasColumnName("VerificationStatus").HasConversion<int>().IsRequired();
        builder.Property(e => e.ModelUsed).HasColumnName("ModelUsed").HasMaxLength(200);
        builder.Property(e => e.ModelVersion).HasColumnName("ModelVersion").HasMaxLength(100);
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").IsRequired();
        builder.HasIndex(e => e.SourceLanguageVariantId).HasDatabaseName("IX_Translations_SourceVariantId");
        builder.HasIndex(e => e.TargetLanguageVariantId).HasDatabaseName("IX_Translations_TargetVariantId");
        // Cannot index TEXT column without key length in MySQL
        // builder.HasIndex(e => new { e.SourceLanguageVariantId, e.TargetLanguageVariantId, e.SourceText }).HasDatabaseName("IX_Translations_Source_Target_Text");
        
        builder.HasOne<Domain.Entities.LanguageVariant>()
            .WithMany()
            .HasForeignKey(e => e.SourceLanguageVariantId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne<Domain.Entities.LanguageVariant>()
            .WithMany()
            .HasForeignKey(e => e.TargetLanguageVariantId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasMany(e => e.Evidence)
            .WithOne()
            .HasForeignKey("EntityId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
