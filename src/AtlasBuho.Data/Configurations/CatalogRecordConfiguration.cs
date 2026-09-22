using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Configurations;

public class CatalogRecordConfiguration : IEntityTypeConfiguration<Domain.Entities.CatalogRecord>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.CatalogRecord> builder)
    {
        builder.ToTable("CatalogRecords");
        
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.CatalogVersionId).HasColumnName("CatalogVersionId").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.Status).HasColumnName("Status").HasConversion<int>().IsRequired();
        builder.Property(e => e.EntityType).HasColumnName("EntityType").HasMaxLength(100).IsRequired();
        builder.Property(e => e.IdentityKey).HasColumnName("IdentityKey").HasMaxLength(500).IsRequired();
        builder.Property(e => e.InaliCode).HasColumnName("InaliCode").HasMaxLength(50);
        builder.Property(e => e.Iso639_3Code).HasColumnName("Iso639_3Code").HasMaxLength(10);
        builder.Property(e => e.Name).HasColumnName("Name").HasMaxLength(500).IsRequired();
        builder.Property(e => e.Autodenomination).HasColumnName("Autodenomination").HasMaxLength(200);
        builder.Property(e => e.Description).HasColumnName("Description").HasColumnType("TEXT");
        builder.Property(e => e.GeoReference).HasColumnName("GeoReference").HasMaxLength(500);
        builder.Property(e => e.SourceDocumentId).HasColumnName("SourceDocumentId").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.SourceHash).HasColumnName("SourceHash").HasMaxLength(64).IsRequired();
        builder.Property(e => e.SourceUrl).HasColumnName("SourceUrl").HasMaxLength(2000).IsRequired();
        builder.Property(e => e.SourcePage).HasColumnName("SourcePage").IsRequired();
        builder.Property(e => e.SourceSection).HasColumnName("SourceSection").HasMaxLength(500);
        builder.Property(e => e.ExtractionDate).HasColumnName("ExtractionDate").IsRequired();
        builder.Property(e => e.ParserVersion).HasColumnName("ParserVersion").HasMaxLength(50).IsRequired();
        builder.Property(e => e.ParentRecordId).HasColumnName("ParentRecordId").HasColumnType("CHAR(36)");
        builder.HasIndex(e => new { e.CatalogVersionId, e.EntityType, e.IdentityKey }).IsUnique().HasDatabaseName("IX_CatalogRecords_Version_Type_Identity");
        builder.HasIndex(e => e.IdentityKey).HasDatabaseName("IX_CatalogRecords_IdentityKey");
        builder.HasIndex(e => e.SourceDocumentId).HasDatabaseName("IX_CatalogRecords_SourceDocument");
        builder.HasIndex(e => e.SourcePage).HasDatabaseName("IX_CatalogRecords_SourcePage");
        builder.HasIndex(e => e.SourceUrl).HasDatabaseName("IX_CatalogRecords_SourceUrl").HasAnnotation("MySql:IndexPrefixLength", new[] { 700 });
        
        builder.HasOne(e => e.CatalogVersion)
            .WithMany(e => e.Records)
            .HasForeignKey(e => e.CatalogVersionId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(e => e.ParentRecord)
            .WithMany()
            .HasForeignKey(e => e.ParentRecordId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}