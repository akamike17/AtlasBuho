using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Configurations;

public class EvidenceConfiguration : IEntityTypeConfiguration<Domain.Entities.Evidence>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Evidence> builder)
    {
        builder.ToTable("Evidence");
        
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.SourceId).HasColumnName("SourceId").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.EntityType).HasColumnName("EntityType").HasMaxLength(100);
        builder.Property(e => e.EntityId).HasColumnName("EntityId").HasColumnType("CHAR(36)");
        builder.Property(e => e.Quote).HasColumnName("Quote").HasColumnType("TEXT");
        builder.Property(e => e.PageReference).HasColumnName("PageReference").HasMaxLength(200);
        builder.Property(e => e.SectionReference).HasColumnName("SectionReference").HasMaxLength(500);
        builder.Property(e => e.Url).HasColumnName("Url").HasMaxLength(1000);
        builder.Property(e => e.Notes).HasColumnName("Notes").HasColumnType("TEXT");
        builder.Property(e => e.Confidence).HasColumnName("Confidence").HasColumnType("decimal(3,2)").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").IsRequired();
        builder.HasIndex(e => e.SourceId).HasDatabaseName("IX_Evidence_SourceId");
        builder.HasIndex(e => new { e.EntityType, e.EntityId }).HasDatabaseName("IX_Evidence_Entity");
        builder.HasIndex(e => e.Url).HasDatabaseName("IX_Evidence_Url").HasAnnotation("MySql:IndexPrefixLength", new[] { 700 });
        
        builder.HasOne<Domain.Entities.Source>()
            .WithMany(e => e.Evidence)
            .HasForeignKey(e => e.SourceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
