using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Configurations;

public class ConsentRecordConfiguration : IEntityTypeConfiguration<Domain.Entities.ConsentRecord>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.ConsentRecord> builder)
    {
        builder.ToTable("ConsentRecords");
        
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.SpeakerId).HasColumnName("SpeakerId").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.AudioRecordingId).HasColumnName("AudioRecordingId");
        builder.Property(e => e.VideoRecordingId).HasColumnName("VideoRecordingId");
        builder.Property(e => e.LexemeId).HasColumnName("LexemeId");
        builder.Property(e => e.PhraseId).HasColumnName("PhraseId");
        builder.Property(e => e.Status).HasColumnName("Status").HasConversion<int>().IsRequired();
        builder.Property(e => e.GrantedBy).HasColumnName("GrantedBy").HasMaxLength(200);
        builder.Property(e => e.GrantedAt).HasColumnName("GrantedAt");
        builder.Property(e => e.ExpiresAt).HasColumnName("ExpiresAt");
        builder.Property(e => e.Scope).HasColumnName("Scope").HasColumnType("TEXT");
        builder.Property(e => e.Restrictions).HasColumnName("Restrictions").HasColumnType("TEXT");
        builder.Property(e => e.Notes).HasColumnName("Notes").HasColumnType("TEXT");
        builder.Property(e => e.IsRevoked).HasColumnName("IsRevoked").IsRequired();
        builder.Property(e => e.RevokedAt).HasColumnName("RevokedAt");
        builder.Property(e => e.RevokedBy).HasColumnName("RevokedBy").HasMaxLength(200);
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").IsRequired();
        builder.HasIndex(e => e.SpeakerId).HasDatabaseName("IX_ConsentRecords_SpeakerId");
        builder.HasIndex(e => e.AudioRecordingId).HasDatabaseName("IX_ConsentRecords_AudioRecordingId");
        builder.HasIndex(e => e.VideoRecordingId).HasDatabaseName("IX_ConsentRecords_VideoRecordingId");
        builder.HasIndex(e => e.LexemeId).HasDatabaseName("IX_ConsentRecords_LexemeId");
        builder.HasIndex(e => e.PhraseId).HasDatabaseName("IX_ConsentRecords_PhraseId");
        builder.HasIndex(e => e.IsRevoked).HasDatabaseName("IX_ConsentRecords_IsRevoked");
        
        builder.HasOne<Domain.Entities.SpeakerProfile>()
            .WithMany(e => e.Consents)
            .HasForeignKey(e => e.SpeakerId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne<Domain.Entities.AudioRecording>()
            .WithMany()
            .HasForeignKey(e => e.AudioRecordingId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne<Domain.Entities.VideoRecording>()
            .WithMany()
            .HasForeignKey(e => e.VideoRecordingId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne<Domain.Entities.Lexeme>()
            .WithMany()
            .HasForeignKey(e => e.LexemeId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne<Domain.Entities.Phrase>()
            .WithMany()
            .HasForeignKey(e => e.PhraseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
