using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Configurations;

public class MeaningConfiguration : IEntityTypeConfiguration<Domain.Entities.Meaning>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Meaning> builder)
    {
        builder.ToTable("Meanings");
        
        builder.Property(e => e.Id).HasColumnName("Id").IsRequired();
        builder.Property(e => e.LexemeId).HasColumnName("LexemeId").IsRequired();
        builder.Property(e => e.SpanishMeaning).HasColumnName("SpanishMeaning").HasMaxLength(2000).IsRequired();
        builder.Property(e => e.EnglishMeaning).HasColumnName("EnglishMeaning").HasMaxLength(2000);
        builder.Property(e => e.PartOfSpeech).HasColumnName("PartOfSpeech").HasMaxLength(100);
        builder.Property(e => e.SemanticDomain).HasColumnName("SemanticDomain").HasMaxLength(200);
        builder.Property(e => e.Register).HasColumnName("Register").HasMaxLength(100);
        builder.Property(e => e.RegionalNotes).HasColumnName("RegionalNotes").HasMaxLength(2000);
        builder.Property(e => e.Source).HasColumnName("Source").HasMaxLength(500);
        builder.Property(e => e.VerificationStatus).HasColumnName("VerificationStatus").HasConversion<int>().IsRequired();
        builder.Property(e => e.Confidence).HasColumnName("Confidence").HasColumnType("decimal(3,2)").IsRequired();
        builder.Property(e => e.Order).HasColumnName("Order").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").IsRequired();
        builder.HasIndex(e => e.LexemeId).HasDatabaseName("IX_Meanings_LexemeId");
        builder.HasIndex(e => new { e.LexemeId, e.Order }).HasDatabaseName("IX_Meanings_Lexeme_Order");
        
        builder.HasOne<Domain.Entities.Lexeme>()
            .WithMany(e => e.Meanings)
            .HasForeignKey(e => e.LexemeId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(e => e.Evidence)
            .WithOne()
            .HasForeignKey("EntityId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
