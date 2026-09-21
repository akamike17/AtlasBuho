using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Configurations;

public class AudioRecordingConfiguration : IEntityTypeConfiguration<Domain.Entities.AudioRecording>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.AudioRecording> builder)
    {
        builder.ToTable("AudioRecordings");
        
        builder.Property(e => e.Id).HasColumnName("Id").IsRequired();
        builder.Property(e => e.SpeakerId).HasColumnName("SpeakerId");
        builder.Property(e => e.LanguageVariantId).HasColumnName("LanguageVariantId").IsRequired();
        builder.Property(e => e.CommunityId).HasColumnName("CommunityId");
        builder.Property(e => e.FilePath).HasColumnName("FilePath").HasMaxLength(1000).IsRequired();
        builder.Property(e => e.FileName).HasColumnName("FileName").HasMaxLength(500);
        builder.Property(e => e.FileHash).HasColumnName("FileHash").HasMaxLength(100);
        builder.Property(e => e.FileSizeBytes).HasColumnName("FileSizeBytes");
        builder.Property(e => e.MimeType).HasColumnName("MimeType").HasMaxLength(100);
        builder.Property(e => e.DurationSeconds).HasColumnName("DurationSeconds").HasColumnType("decimal(10,2)");
        builder.Property(e => e.SampleRate).HasColumnName("SampleRate");
        builder.Property(e => e.Channels).HasColumnName("Channels");
        builder.Property(e => e.RecordedAt).HasColumnName("RecordedAt");
        builder.Property(e => e.Context).HasColumnName("Context").HasMaxLength(2000);
        builder.Property(e => e.Transcription).HasColumnName("Transcription").HasMaxLength(5000);
        builder.Property(e => e.Translation).HasColumnName("Translation").HasMaxLength(5000);
        builder.Property(e => e.Annotation).HasColumnName("Annotation").HasMaxLength(2000);
        builder.Property(e => e.Source).HasColumnName("Source").HasMaxLength(500);
        builder.Property(e => e.ConsentStatus).HasColumnName("ConsentStatus").HasConversion<int>().IsRequired();
        builder.Property(e => e.ConsentSource).HasColumnName("ConsentSource").HasMaxLength(500);
        builder.Property(e => e.UsagePermission).HasColumnName("UsagePermission").HasMaxLength(2000);
        builder.Property(e => e.AttributionRequirement).HasColumnName("AttributionRequirement").HasMaxLength(2000);
        builder.Property(e => e.RemovalRequested).HasColumnName("RemovalRequested").IsRequired();
        builder.Property(e => e.CommunityRestriction).HasColumnName("CommunityRestriction").HasMaxLength(2000);
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").IsRequired();
        builder.HasIndex(e => e.LanguageVariantId).HasDatabaseName("IX_AudioRecordings_VariantId");
        builder.HasIndex(e => e.SpeakerId).HasDatabaseName("IX_AudioRecordings_SpeakerId");
        builder.HasIndex(e => e.CommunityId).HasDatabaseName("IX_AudioRecordings_CommunityId");
        builder.HasIndex(e => e.ConsentStatus).HasDatabaseName("IX_AudioRecordings_ConsentStatus");
        builder.HasIndex(e => e.RemovalRequested).HasDatabaseName("IX_AudioRecordings_RemovalRequested");
        
        builder.HasOne<Domain.Entities.LanguageVariant>()
            .WithMany()
            .HasForeignKey(e => e.LanguageVariantId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne<Domain.Entities.Community>()
            .WithMany()
            .HasForeignKey(e => e.CommunityId)
            .OnDelete(DeleteBehavior.SetNull);
            
        builder.HasOne<Domain.Entities.SpeakerProfile>()
            .WithMany(e => e.Recordings)
            .HasForeignKey(e => e.SpeakerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
