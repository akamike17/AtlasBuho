using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Configurations;

public class PronunciationConfiguration : IEntityTypeConfiguration<Domain.Entities.Pronunciation>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Pronunciation> builder)
    {
        builder.ToTable("Pronunciations");
        
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.LexemeId).HasColumnName("LexemeId").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.Ipa).HasColumnName("Ipa").HasMaxLength(500);
        builder.Property(e => e.Readable).HasColumnName("Readable").HasMaxLength(500);
        builder.Property(e => e.AudioUrl).HasColumnName("AudioUrl").HasMaxLength(1000);
        builder.Property(e => e.AudioRecordingId).HasColumnName("AudioRecordingId").HasColumnType("CHAR(36)");
        builder.Property(e => e.SpeakerId).HasColumnName("SpeakerId").HasColumnType("CHAR(36)");
        builder.Property(e => e.Source).HasColumnName("Source").HasMaxLength(500);
        builder.Property(e => e.VerificationStatus).HasColumnName("VerificationStatus").HasConversion<int>().IsRequired();
        builder.Property(e => e.Confidence).HasColumnName("Confidence").HasColumnType("decimal(3,2)").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").IsRequired();
        builder.HasIndex(e => e.LexemeId).HasDatabaseName("IX_Pronunciations_LexemeId");
        builder.HasIndex(e => e.AudioRecordingId).HasDatabaseName("IX_Pronunciations_AudioRecordingId");
        builder.HasIndex(e => e.AudioUrl).HasDatabaseName("IX_Pronunciations_AudioUrl").HasAnnotation("MySql:IndexPrefixLength", new[] { 700 });
        
        builder.HasOne<Domain.Entities.Lexeme>()
            .WithMany(e => e.Pronunciations)
            .HasForeignKey(e => e.LexemeId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne<Domain.Entities.AudioRecording>()
            .WithMany()
            .HasForeignKey(e => e.AudioRecordingId)
            .OnDelete(DeleteBehavior.SetNull);
            
        builder.HasOne<Domain.Entities.SpeakerProfile>()
            .WithMany()
            .HasForeignKey(e => e.SpeakerId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
