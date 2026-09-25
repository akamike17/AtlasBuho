using AtlasBuho.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtlasBuho.Data.Configurations;

/// <summary>
/// ADR 0001 — persistence configuration for <see cref="LexicalEquivalence"/>.
///
/// I1 — at most one canonical row per (SourceLexemeId, TargetLanguage, CatalogVersionId).
/// MySQL/MarialDB do not support filtered unique indexes, so the invariant is enforced with a
/// unique index over the tuple *plus a generated column* that is 1 only when IsCanonical = 1
/// (computed at the database level). Column generated in the migration; index defined here so
/// EF also guards it in the model.
///
/// I2 — Directionality is represented in the row itself (`TargetLanguage`), never inferred.
/// I3 — Evidence append-only: `RowHash` + CatalogVersionId FK protect against silent mutation;
///       the engine never updates rows pointing at a Completed CatalogVersion.
/// </summary>
public class LexicalEquivalenceConfiguration : IEntityTypeConfiguration<LexicalEquivalence>
{
    public void Configure(EntityTypeBuilder<LexicalEquivalence> builder)
    {
        builder.ToTable("LexicalEquivalences");

        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.SourceLexemeId).HasColumnName("SourceLexemeId").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.SourceId).HasColumnName("SourceId").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.CatalogVersionId).HasColumnName("CatalogVersionId").HasColumnType("CHAR(36)").IsRequired();

        builder.Property(e => e.TargetLanguage).HasColumnName("TargetLanguage").HasMaxLength(8).IsRequired();
        builder.Property(e => e.TargetText).HasColumnName("TargetText").HasMaxLength(500).IsRequired();
        builder.Property(e => e.IsCanonical).HasColumnName("IsCanonical").IsRequired();
        builder.Property(e => e.VerificationStatus).HasColumnName("VerificationStatus").IsRequired();
        builder.Property(e => e.RowHash).HasColumnName("RowHash").HasMaxLength(64).IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();

        builder.HasKey(e => e.Id);

        // Fast lookup: evidence for one lexeme + target language + corpus version.
        builder.HasIndex(e => new { e.SourceLexemeId, e.TargetLanguage, e.CatalogVersionId })
            .HasDatabaseName("IX_LexicalEquivalences_Lookup");

        // Row integrity: detect tampering even outside EF.
        builder.HasIndex(e => e.RowHash).HasDatabaseName("IX_LexicalEquivalences_RowHash");

        builder.HasOne(e => e.SourceLexeme)
            .WithMany()
            .HasForeignKey(e => e.SourceLexemeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.EvidenceSource)
            .WithMany()
            .HasForeignKey(e => e.SourceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.CatalogVersion)
            .WithMany()
            .HasForeignKey(e => e.CatalogVersionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
