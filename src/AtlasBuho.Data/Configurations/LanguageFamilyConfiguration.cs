using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Configurations;

public class LanguageFamilyConfiguration : IEntityTypeConfiguration<Domain.Entities.LanguageFamily>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.LanguageFamily> builder)
    {
        builder.ToTable("LanguageFamilies");
        
        builder.Property(e => e.Id).HasColumnName("Id").IsRequired();
        builder.Property(e => e.Name).HasColumnName("Name").HasMaxLength(200).IsRequired();
        builder.Property(e => e.NameEnglish).HasColumnName("NameEnglish").HasMaxLength(200);
        builder.Property(e => e.Description).HasColumnName("Description").HasMaxLength(2000);
        builder.Property(e => e.Source).HasColumnName("Source").HasMaxLength(500);
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").IsRequired();
        builder.HasIndex(e => e.Name).IsUnique().HasDatabaseName("IX_LanguageFamilies_Name");
        
        builder.HasMany(e => e.LanguageGroups)
            .WithOne()
            .HasForeignKey("LanguageFamilyId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
