using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Configurations;

public class LanguageGroupConfiguration : IEntityTypeConfiguration<Domain.Entities.LanguageGroup>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.LanguageGroup> builder)
    {
        builder.ToTable("LanguageGroups");
        
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.LanguageFamilyId).HasColumnName("LanguageFamilyId").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.Name).HasColumnName("Name").HasMaxLength(200).IsRequired();
        builder.Property(e => e.NameEnglish).HasColumnName("NameEnglish").HasMaxLength(200);
        builder.Property(e => e.Description).HasColumnName("Description").HasColumnType("TEXT");
        builder.Property(e => e.Source).HasColumnName("Source").HasMaxLength(500);
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").IsRequired();
        builder.HasIndex(e => new { e.LanguageFamilyId, e.Name }).IsUnique().HasDatabaseName("IX_LanguageGroups_Family_Name");
        builder.HasIndex(e => e.LanguageFamilyId).HasDatabaseName("IX_LanguageGroups_FamilyId");
        
        builder.HasOne<Domain.Entities.LanguageFamily>()
            .WithMany(e => e.LanguageGroups)
            .HasForeignKey(e => e.LanguageFamilyId)
            .OnDelete(DeleteBehavior.Cascade);
           
        builder.HasMany(e => e.LanguageVariants)
            .WithOne()
            .HasForeignKey("LanguageGroupId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
