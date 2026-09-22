using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Configurations;

public class LexemeConfiguration : IEntityTypeConfiguration<Domain.Entities.Lexeme>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Lexeme> builder)
    {
        builder.ToTable("Lexemes");
        
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.LanguageVariantId).HasColumnName("LanguageVariantId").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.CanonicalForm).HasColumnName("CanonicalForm").HasMaxLength(500).IsRequired();
        builder.Property(e => e.AlternativeForms).HasColumnName("AlternativeForms").HasColumnType("TEXT");
        builder.Property(e => e.Autodenomination).HasColumnName("Autodenomination").HasMaxLength(500);
        builder.Property(e => e.SpanishMeaning).HasColumnName("SpanishMeaning").HasColumnType("TEXT");
        builder.Property(e => e.PartOfSpeech).HasColumnName("PartOfSpeech").HasMaxLength(100);
        builder.Property(e => e.PronunciationIpa).HasColumnName("PronunciationIpa").HasMaxLength(500);
        builder.Property(e => e.PronunciationReadable).HasColumnName("PronunciationReadable").HasMaxLength(500);
        builder.Property(e => e.SemanticDomain).HasColumnName("SemanticDomain").HasMaxLength(200);
        builder.Property(e => e.Register).HasColumnName("Register").HasMaxLength(100);
        builder.Property(e => e.RegionalNotes).HasColumnName("RegionalNotes").HasColumnType("TEXT");
        builder.Property(e => e.Etymology).HasColumnName("Etymology").HasColumnType("TEXT");
        builder.Property(e => e.Source).HasColumnName("Source").HasMaxLength(500);
        builder.Property(e => e.VerificationStatus).HasColumnName("VerificationStatus").HasConversion<int>().IsRequired();
        builder.Property(e => e.Confidence).HasColumnName("Confidence").HasColumnType("decimal(3,2)").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").IsRequired();
        builder.HasIndex(e => new { e.LanguageVariantId, e.CanonicalForm }).IsUnique().HasDatabaseName("IX_Lexemes_Variant_CanonicalForm");
        builder.HasIndex(e => e.LanguageVariantId).HasDatabaseName("IX_Lexemes_VariantId");
        builder.HasIndex(e => e.SemanticDomain).HasDatabaseName("IX_Lexemes_SemanticDomain");
        builder.HasIndex(e => e.VerificationStatus).HasDatabaseName("IX_Lexemes_VerificationStatus");
        
        builder.HasOne<Domain.Entities.LanguageVariant>()
            .WithMany(e => e.Lexemes)
            .HasForeignKey(e => e.LanguageVariantId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(e => e.Pronunciations)
            .WithOne()
            .HasForeignKey("LexemeId")
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(e => e.Examples)
            .WithOne()
            .HasForeignKey("LexemeId")
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(e => e.Meanings)
            .WithOne()
            .HasForeignKey("LexemeId")
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(e => e.Evidence)
            .WithOne()
            .HasForeignKey("EntityId")
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(e => e.AudioRecordings)
            .WithOne()
            .HasForeignKey("LexemeId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
