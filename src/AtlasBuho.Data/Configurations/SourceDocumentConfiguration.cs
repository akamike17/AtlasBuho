using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Configurations;

public class SourceDocumentConfiguration : IEntityTypeConfiguration<Domain.Entities.SourceDocument>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.SourceDocument> builder)
    {
        builder.ToTable("SourceDocuments");
        
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.Title).HasColumnName("Title").HasMaxLength(500).IsRequired();
        builder.Property(e => e.Url).HasColumnName("Url").HasMaxLength(2000).IsRequired();
        builder.Property(e => e.HashSha256).HasColumnName("HashSha256").HasMaxLength(64).IsRequired();
        builder.Property(e => e.ContentType).HasColumnName("ContentType").HasMaxLength(100).IsRequired();
        builder.Property(e => e.SizeBytes).HasColumnName("SizeBytes").IsRequired();
        builder.Property(e => e.RetrievedAt).HasColumnName("RetrievedAt").IsRequired();
        builder.Property(e => e.TotalPages).HasColumnName("TotalPages").IsRequired();
        builder.Property(e => e.CatalogRange).HasColumnName("CatalogRange").HasMaxLength(100);
        builder.Property(e => e.ParserVersion).HasColumnName("ParserVersion").HasMaxLength(50).IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").IsRequired();
        builder.HasIndex(e => e.HashSha256).IsUnique().HasDatabaseName("IX_SourceDocuments_Hash");
        // URL index requires length prefix in MySQL
        builder.HasIndex(e => e.Url).HasDatabaseName("IX_SourceDocuments_Url").HasAnnotation("MySql:IndexPrefixLength", new[] { 700 });
        
        builder.HasMany(e => e.Pages)
            .WithOne(e => e.SourceDocument)
            .HasForeignKey(e => e.SourceDocumentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}