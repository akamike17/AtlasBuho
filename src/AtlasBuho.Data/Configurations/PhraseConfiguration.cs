using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Configurations;

public class PhraseConfiguration : IEntityTypeConfiguration<Domain.Entities.Phrase>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Phrase> builder)
    {
        builder.ToTable("Phrases");
        
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.LanguageVariantId).HasColumnName("LanguageVariantId").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.Text).HasColumnName("Text").HasColumnType("TEXT").IsRequired();
        builder.Property(e => e.SpanishTranslation).HasColumnName("SpanishTranslation").HasColumnType("TEXT");
        builder.Property(e => e.EnglishTranslation).HasColumnName("EnglishTranslation").HasColumnType("TEXT");
        builder.Property(e => e.Context).HasColumnName("Context").HasColumnType("TEXT");
        builder.Property(e => e.GrammarNotes).HasColumnName("GrammarNotes").HasColumnType("TEXT");
        builder.Property(e => e.Source).HasColumnName("Source").HasMaxLength(500);
        builder.Property(e => e.VerificationStatus).HasColumnName("VerificationStatus").HasConversion<int>().IsRequired();
        builder.Property(e => e.Confidence).HasColumnName("Confidence").HasColumnType("decimal(3,2)").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").IsRequired();
        builder.HasIndex(e => e.LanguageVariantId).HasDatabaseName("IX_Phrases_VariantId");
        builder.HasIndex(e => e.VerificationStatus).HasDatabaseName("IX_Phrases_VerificationStatus");
        
        builder.HasOne<Domain.Entities.LanguageVariant>()
            .WithMany()
            .HasForeignKey(e => e.LanguageVariantId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(e => e.PhraseLexemes)
            .WithOne()
            .HasForeignKey("PhraseId")
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(e => e.AudioRecordings)
            .WithOne()
            .HasForeignKey("PhraseId")
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(e => e.Evidence)
            .WithOne()
            .HasForeignKey("EntityId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
