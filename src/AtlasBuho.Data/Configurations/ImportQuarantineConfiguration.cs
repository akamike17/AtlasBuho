using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Configurations;

/// <summary>
/// 5B.md FASE 9 / 4B.md §9 — quarantine identity.
///
/// The quarantine identity is the tuple
///   (CatalogVersionId, SourceDocumentId, EntityType, SourcePage, ResolutionMethod, RawDataHash)
/// and it is enforced BY THE DATABASE, not only by an application-level AnyAsync-then-Add check.
/// <see cref="ImportQuarantine.RawDataHash"/> is a SHA256 over the canonicalized source row, so
/// the identity distinguishes source ROWS (two identical rows on different ordinals/pages do not
/// collapse) and re-imports cannot accumulate duplicate quarantine records.
/// </summary>
public class ImportQuarantineConfiguration : IEntityTypeConfiguration<ImportQuarantine>
{
    public void Configure(EntityTypeBuilder<ImportQuarantine> builder)
    {
        builder.ToTable("ImportQuarantines");

        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.CatalogVersionId).HasColumnName("CatalogVersionId").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.SourceDocumentId).HasColumnName("SourceDocumentId").HasColumnType("CHAR(36)").IsRequired();

        // Bounded so the columns can participate in the composite unique index.
        builder.Property(e => e.EntityType).HasColumnName("EntityType").HasMaxLength(100).IsRequired();
        builder.Property(e => e.RawDataHash).HasColumnName("RawDataHash").HasMaxLength(64).IsRequired();
        builder.Property(e => e.ResolutionMethod).HasColumnName("ResolutionMethod").HasMaxLength(100).IsRequired();
        builder.Property(e => e.SourceSection).HasColumnName("SourceSection").HasMaxLength(100).IsRequired();
        builder.Property(e => e.ComparisonKey).HasColumnName("ComparisonKey").HasMaxLength(500);

        builder.Property(e => e.RawDataJson).HasColumnName("RawDataJson").HasColumnType("longtext").IsRequired();
        builder.Property(e => e.Reason).HasColumnName("Reason").HasColumnType("longtext").IsRequired();
        builder.Property(e => e.ResolvedBy).HasColumnName("ResolvedBy").HasMaxLength(200);
        builder.Property(e => e.ResolutionNotes).HasColumnName("ResolutionNotes").HasColumnType("longtext");

        builder.Property(e => e.SourcePage).HasColumnName("SourcePage").IsRequired();
        builder.Property(e => e.Status).HasColumnName("Status").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.Property(e => e.ResolvedAt).HasColumnName("ResolvedAt");

        builder.HasKey(e => e.Id);

        // The database must protect the invariant even under concurrent execution.
        builder.HasIndex(e => new
            {
                e.CatalogVersionId,
                e.SourceDocumentId,
                e.EntityType,
                e.SourcePage,
                e.ResolutionMethod,
                e.RawDataHash
            })
            .IsUnique()
            .HasDatabaseName("IX_ImportQuarantines_Identity");

        builder.HasIndex(e => e.CatalogVersionId);
        builder.HasIndex(e => e.SourceDocumentId);

        builder.HasOne(e => e.CatalogVersion)
            .WithMany()
            .HasForeignKey(e => e.CatalogVersionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.SourceDocument)
            .WithMany()
            .HasForeignKey(e => e.SourceDocumentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
