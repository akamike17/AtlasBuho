using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Configurations;

public class VideoRecordingConfiguration : IEntityTypeConfiguration<Domain.Entities.VideoRecording>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.VideoRecording> builder)
    {
        builder.ToTable("VideoRecordings");
        
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.SpeakerId).HasColumnName("SpeakerId").HasColumnType("CHAR(36)");
        builder.Property(e => e.LanguageVariantId).HasColumnName("LanguageVariantId").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.CommunityId).HasColumnName("CommunityId").HasColumnType("CHAR(36)");
        builder.Property(e => e.FilePath).HasColumnName("FilePath").HasMaxLength(1000).IsRequired();
        builder.Property(e => e.FileName).HasColumnName("FileName").HasMaxLength(500);
        builder.Property(e => e.FileHash).HasColumnName("FileHash").HasMaxLength(100);
        builder.Property(e => e.FileSizeBytes).HasColumnName("FileSizeBytes");
        builder.Property(e => e.MimeType).HasColumnName("MimeType").HasMaxLength(100);
        builder.Property(e => e.DurationSeconds).HasColumnName("DurationSeconds").HasColumnType("decimal(10,2)");
        builder.Property(e => e.Width).HasColumnName("Width");
        builder.Property(e => e.Height).HasColumnName("Height");
        builder.Property(e => e.FrameRate).HasColumnName("FrameRate").HasMaxLength(20);
        builder.Property(e => e.RecordedAt).HasColumnName("RecordedAt");
        builder.Property(e => e.Context).HasColumnName("Context").HasColumnType("TEXT");
        builder.Property(e => e.Transcription).HasColumnName("Transcription").HasColumnType("TEXT");
        builder.Property(e => e.Translation).HasColumnName("Translation").HasColumnType("TEXT");
        builder.Property(e => e.Annotation).HasColumnName("Annotation").HasColumnType("TEXT");
        builder.Property(e => e.Source).HasColumnName("Source").HasMaxLength(500);
        builder.Property(e => e.ConsentStatus).HasColumnName("ConsentStatus").HasConversion<int>().IsRequired();
        builder.Property(e => e.ConsentSource).HasColumnName("ConsentSource").HasMaxLength(500);
        builder.Property(e => e.UsagePermission).HasColumnName("UsagePermission").HasColumnType("TEXT");
        builder.Property(e => e.AttributionRequirement).HasColumnName("AttributionRequirement").HasColumnType("TEXT");
        builder.Property(e => e.RemovalRequested).HasColumnName("RemovalRequested").IsRequired();
        builder.Property(e => e.CommunityRestriction).HasColumnName("CommunityRestriction").HasColumnType("TEXT");
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").IsRequired();
        builder.HasIndex(e => e.LanguageVariantId).HasDatabaseName("IX_VideoRecordings_VariantId");
        builder.HasIndex(e => e.SpeakerId).HasDatabaseName("IX_VideoRecordings_SpeakerId");
        builder.HasIndex(e => e.CommunityId).HasDatabaseName("IX_VideoRecordings_CommunityId");
        builder.HasIndex(e => e.ConsentStatus).HasDatabaseName("IX_VideoRecordings_ConsentStatus");
        builder.HasIndex(e => e.RemovalRequested).HasDatabaseName("IX_VideoRecordings_RemovalRequested");
        builder.HasIndex(e => e.FilePath).HasDatabaseName("IX_VideoRecordings_FilePath").HasAnnotation("MySql:IndexPrefixLength", new[] { 700 });
        
        builder.HasOne<Domain.Entities.LanguageVariant>()
            .WithMany()
            .HasForeignKey(e => e.LanguageVariantId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne<Domain.Entities.Community>()
            .WithMany()
            .HasForeignKey(e => e.CommunityId)
            .OnDelete(DeleteBehavior.SetNull);
            
        builder.HasOne<Domain.Entities.SpeakerProfile>()
            .WithMany()
            .HasForeignKey(e => e.SpeakerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
