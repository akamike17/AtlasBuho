using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Configurations;

public class PhraseLexemeConfiguration : IEntityTypeConfiguration<Domain.Entities.PhraseLexeme>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.PhraseLexeme> builder)
    {
        builder.ToTable("PhraseLexemes");
        
        builder.HasKey(e => new { e.PhraseId, e.LexemeId, e.Position });
        
        builder.Property(e => e.PhraseId).HasColumnName("PhraseId").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.LexemeId).HasColumnName("LexemeId").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.Position).HasColumnName("Position").IsRequired();
        builder.Property(e => e.GrammarRole).HasColumnName("GrammarRole").HasMaxLength(100);
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.HasIndex(e => e.PhraseId).HasDatabaseName("IX_PhraseLexemes_PhraseId");
        builder.HasIndex(e => e.LexemeId).HasDatabaseName("IX_PhraseLexemes_LexemeId");
        
        builder.HasOne<Domain.Entities.Phrase>()
            .WithMany(e => e.PhraseLexemes)
            .HasForeignKey(e => e.PhraseId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne<Domain.Entities.Lexeme>()
            .WithMany(e => e.PhraseLexemes)
            .HasForeignKey(e => e.LexemeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
