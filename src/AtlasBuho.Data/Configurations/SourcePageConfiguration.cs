using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Configurations;

public class SourcePageConfiguration : IEntityTypeConfiguration<Domain.Entities.SourcePage>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.SourcePage> builder)
    {
        builder.ToTable("SourcePages");
        
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.SourceDocumentId).HasColumnName("SourceDocumentId").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.PageNumber).HasColumnName("PageNumber").IsRequired();
        builder.Property(e => e.SectionReference).HasColumnName("SectionReference").HasMaxLength(500);
        builder.Property(e => e.RawTextHash).HasColumnName("RawTextHash").HasMaxLength(64);
        builder.Property(e => e.ExtractedAt).HasColumnName("ExtractedAt").IsRequired();
        builder.HasIndex(e => new { e.SourceDocumentId, e.PageNumber }).IsUnique().HasDatabaseName("IX_SourcePages_Document_Page");
        
        builder.HasOne(e => e.SourceDocument)
            .WithMany(e => e.Pages)
            .HasForeignKey(e => e.SourceDocumentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}