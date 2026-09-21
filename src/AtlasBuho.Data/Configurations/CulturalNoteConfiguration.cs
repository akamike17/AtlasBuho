using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Configurations;

public class CulturalNoteConfiguration : IEntityTypeConfiguration<Domain.Entities.CulturalNote>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.CulturalNote> builder)
    {
        builder.ToTable("CulturalNotes");
        
        builder.Property(e => e.Id).HasColumnName("Id").IsRequired();
        builder.Property(e => e.LanguageVariantId).HasColumnName("LanguageVariantId").IsRequired();
        builder.Property(e => e.CommunityId).HasColumnName("CommunityId");
        builder.Property(e => e.Type).HasColumnName("Type").HasConversion<int>().IsRequired();
        builder.Property(e => e.Title).HasColumnName("Title").HasMaxLength(500).IsRequired();
        builder.Property(e => e.Content).HasColumnName("Content").HasMaxLength(10000).IsRequired();
        builder.Property(e => e.Source).HasColumnName("Source").HasMaxLength(500);
        builder.Property(e => e.Author).HasColumnName("Author").HasMaxLength(200);
        builder.Property(e => e.DateRecorded).HasColumnName("DateRecorded");
        builder.Property(e => e.VerificationStatus).HasColumnName("VerificationStatus").HasConversion<int>().IsRequired();
        builder.Property(e => e.Confidence).HasColumnName("Confidence").HasColumnType("decimal(3,2)").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").IsRequired();
        builder.HasIndex(e => e.LanguageVariantId).HasDatabaseName("IX_CulturalNotes_VariantId");
        builder.HasIndex(e => e.CommunityId).HasDatabaseName("IX_CulturalNotes_CommunityId");
        builder.HasIndex(e => e.Type).HasDatabaseName("IX_CulturalNotes_Type");
        
        builder.HasOne<Domain.Entities.LanguageVariant>()
            .WithMany()
            .HasForeignKey(e => e.LanguageVariantId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne<Domain.Entities.Community>()
            .WithMany()
            .HasForeignKey(e => e.CommunityId)
            .OnDelete(DeleteBehavior.SetNull);
            
        builder.HasMany(e => e.Evidence)
            .WithOne()
            .HasForeignKey("EntityId")
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(e => e.AudioRecordings)
            .WithOne()
            .HasForeignKey("CulturalNoteId")
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(e => e.VideoRecordings)
            .WithOne()
            .HasForeignKey("CulturalNoteId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
