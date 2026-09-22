using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Configurations;

public class CatalogVersionConfiguration : IEntityTypeConfiguration<Domain.Entities.CatalogVersion>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.CatalogVersion> builder)
    {
        builder.ToTable("CatalogVersions");
        
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.CatalogSourceId).HasColumnName("CatalogSourceId").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.VersionNumber).HasColumnName("VersionNumber").HasMaxLength(100).IsRequired();
        builder.Property(e => e.SourceDocumentHash).HasColumnName("SourceDocumentHash").HasMaxLength(64).IsRequired();
        builder.Property(e => e.RetrievedAt).HasColumnName("RetrievedAt").IsRequired();
        builder.Property(e => e.ImportedAt).HasColumnName("ImportedAt").IsRequired();
        builder.Property(e => e.FamilyCount).HasColumnName("FamilyCount").IsRequired();
        builder.Property(e => e.GroupCount).HasColumnName("GroupCount").IsRequired();
        builder.Property(e => e.VariantCount).HasColumnName("VariantCount").IsRequired();
        builder.Property(e => e.AutodenominationCount).HasColumnName("AutodenominationCount").IsRequired();
        builder.Property(e => e.ParserVersion).HasColumnName("ParserVersion").HasMaxLength(50).IsRequired();
        builder.Property(e => e.ImportStatus).HasColumnName("ImportStatus").HasMaxLength(50).IsRequired();
        builder.Property(e => e.ErrorMessage).HasColumnName("ErrorMessage").HasColumnType("TEXT");
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.HasIndex(e => new { e.CatalogSourceId, e.VersionNumber }).IsUnique().HasDatabaseName("IX_CatalogVersions_Source_Version");
        builder.HasIndex(e => e.SourceDocumentHash).HasDatabaseName("IX_CatalogVersions_SourceHash");
        
        builder.HasOne(e => e.CatalogSource)
            .WithMany(e => e.Versions)
            .HasForeignKey(e => e.CatalogSourceId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(e => e.Records)
            .WithOne(e => e.CatalogVersion)
            .HasForeignKey(e => e.CatalogVersionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}