using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Configurations;

public class PhraseLexemeConfiguration : IEntityTypeConfiguration<Domain.Entities.PhraseLexeme>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.PhraseLexeme> builder)
    {
        builder.ToTable("PhraseLexemes");
        
        builder.Property(e => e.Id).HasColumnName("Id").IsRequired();
        builder.Property(e => e.PhraseId).HasColumnName("PhraseId").IsRequired();
        builder.Property(e => e.LexemeId).HasColumnName("LexemeId").IsRequired();
        builder.Property(e => e.Position).HasColumnName("Position").IsRequired();
        builder.Property(e => e.GrammarRole).HasColumnName("GrammarRole").HasMaxLength(100);
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.HasIndex(e => e.PhraseId).HasDatabaseName("IX_PhraseLexemes_PhraseId");
        builder.HasIndex(e => e.LexemeId).HasDatabaseName("IX_PhraseLexemes_LexemeId");
        builder.HasIndex(e => new { e.PhraseId, e.Position }).IsUnique().HasDatabaseName("IX_PhraseLexemes_Phrase_Position");
        
        builder.HasOne<Domain.Entities.Phrase>()
            .WithMany(e => e.PhraseLexemes)
            .HasForeignKey(e => e.PhraseId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne<Domain.Entities.Lexeme>()
            .WithMany()
            .HasForeignKey(e => e.LexemeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
