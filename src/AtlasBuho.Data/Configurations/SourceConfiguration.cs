using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Configurations;

public class SourceConfiguration : IEntityTypeConfiguration<Domain.Entities.Source>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Source> builder)
    {
        builder.ToTable("Sources");
        
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.Name).HasColumnName("Name").HasMaxLength(500).IsRequired();
        builder.Property(e => e.Description).HasColumnName("Description").HasColumnType("TEXT");
        builder.Property(e => e.Level).HasColumnName("Level").HasConversion<int>().IsRequired();
        builder.Property(e => e.Institution).HasColumnName("Institution").HasMaxLength(500);
        builder.Property(e => e.Url).HasColumnName("Url").HasMaxLength(1000);
        builder.Property(e => e.Doi).HasColumnName("Doi").HasMaxLength(200);
        builder.Property(e => e.Isbn).HasColumnName("Isbn").HasMaxLength(50);
        builder.Property(e => e.Issn).HasColumnName("Issn").HasMaxLength(50);
        builder.Property(e => e.PublicationDate).HasColumnName("PublicationDate");
        builder.Property(e => e.Authors).HasColumnName("Authors").HasColumnType("TEXT");
        builder.Property(e => e.Editors).HasColumnName("Editors").HasColumnType("TEXT");
        builder.Property(e => e.Publisher).HasColumnName("Publisher").HasMaxLength(500);
        builder.Property(e => e.Location).HasColumnName("Location").HasMaxLength(500);
        builder.Property(e => e.Language).HasColumnName("Language").HasMaxLength(100);
        builder.Property(e => e.Notes).HasColumnName("Notes").HasColumnType("TEXT");
        builder.Property(e => e.IsActive).HasColumnName("IsActive").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").IsRequired();
        builder.HasIndex(e => e.Name).IsUnique().HasDatabaseName("IX_Sources_Name");
        builder.HasIndex(e => e.Level).HasDatabaseName("IX_Sources_Level");
        builder.HasIndex(e => e.Institution).HasDatabaseName("IX_Sources_Institution");
        builder.HasIndex(e => e.Url).HasDatabaseName("IX_Sources_Url").HasAnnotation("MySql:IndexPrefixLength", new[] { 700 });
        
        builder.HasMany(e => e.Evidence)
            .WithOne(e => e.Source)
            .HasForeignKey(e => e.SourceId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.Metadata.FindNavigation(nameof(Source.Evidence))?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
