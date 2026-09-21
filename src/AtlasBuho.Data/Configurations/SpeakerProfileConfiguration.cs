using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Configurations;

public class SpeakerProfileConfiguration : IEntityTypeConfiguration<Domain.Entities.SpeakerProfile>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.SpeakerProfile> builder)
    {
        builder.ToTable("SpeakerProfiles");
        
        builder.Property(e => e.Id).HasColumnName("Id").IsRequired();
        builder.Property(e => e.LanguageVariantId).HasColumnName("LanguageVariantId").IsRequired();
        builder.Property(e => e.CommunityId).HasColumnName("CommunityId");
        builder.Property(e => e.Name).HasColumnName("Name").HasMaxLength(200).IsRequired();
        builder.Property(e => e.Pseudonym).HasColumnName("Pseudonym").HasMaxLength(200);
        builder.Property(e => e.BirthDate).HasColumnName("BirthDate");
        builder.Property(e => e.Gender).HasColumnName("Gender").HasMaxLength(50);
        builder.Property(e => e.EducationLevel).HasColumnName("EducationLevel").HasMaxLength(200);
        builder.Property(e => e.Occupation).HasColumnName("Occupation").HasMaxLength(200);
        builder.Property(e => e.Source).HasColumnName("Source").HasMaxLength(500);
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").IsRequired();
        builder.HasIndex(e => e.LanguageVariantId).HasDatabaseName("IX_Speakers_VariantId");
        builder.HasIndex(e => e.CommunityId).HasDatabaseName("IX_Speakers_CommunityId");
        
        builder.HasOne<Domain.Entities.LanguageVariant>()
            .WithMany(e => e.Speakers)
            .HasForeignKey(e => e.LanguageVariantId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne<Domain.Entities.Community>()
            .WithMany()
            .HasForeignKey(e => e.CommunityId)
            .OnDelete(DeleteBehavior.SetNull);
            
        builder.HasMany(e => e.Recordings)
            .WithOne()
            .HasForeignKey("SpeakerId")
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(e => e.Consents)
            .WithOne()
            .HasForeignKey("SpeakerId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
