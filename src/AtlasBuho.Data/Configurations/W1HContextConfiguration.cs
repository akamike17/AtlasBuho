namespace AtlasBuho.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

public class W1HContextConfiguration : IEntityTypeConfiguration<W1HContext>
{
    public void Configure(EntityTypeBuilder<W1HContext> builder)
    {
        builder.ToTable("W1HContexts");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("Id")
            .HasColumnType("CHAR(36)")
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.Property(e => e.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
            .ValueGeneratedOnAddOrUpdate();

        // WHO
        builder.Property(e => e.SpeakerId)
            .HasColumnName("SpeakerId")
            .HasColumnType("CHAR(36)")
            .IsRequired(false);

        builder.Property(e => e.CommunityId)
            .HasColumnName("CommunityId")
            .HasColumnType("CHAR(36)")
            .IsRequired(false);

        builder.Property(e => e.ResearcherId)
            .HasColumnName("ResearcherId")
            .HasColumnType("CHAR(36)")
            .IsRequired(false);

        builder.Property(e => e.Role)
            .HasColumnName("Role")
            .HasMaxLength(100)
            .IsRequired(false);

        // WHAT
        builder.Property(e => e.LexemeId)
            .HasColumnName("LexemeId")
            .HasColumnType("CHAR(36)")
            .IsRequired(false);

        builder.Property(e => e.PhraseId)
            .HasColumnName("PhraseId")
            .HasColumnType("CHAR(36)")
            .IsRequired(false);

        builder.Property(e => e.GrammarRuleId)
            .HasColumnName("GrammarRuleId")
            .HasColumnType("CHAR(36)")
            .IsRequired(false);

        builder.Property(e => e.CulturalNoteId)
            .HasColumnName("CulturalNoteId")
            .HasColumnType("CHAR(36)")
            .IsRequired(false);

        builder.Property(e => e.EntityType)
            .HasColumnName("EntityType")
            .HasMaxLength(100)
            .IsRequired(false);

        // WHERE
        builder.Property(e => e.RegionId)
            .HasColumnName("RegionId")
            .HasColumnType("CHAR(36)")
            .IsRequired(false);

        builder.Property(e => e.CommunityLocationId)
            .HasColumnName("CommunityLocationId")
            .HasColumnType("CHAR(36)")
            .IsRequired(false);

        builder.Property(e => e.Latitude)
            .HasColumnName("Latitude")
            .HasPrecision(9, 6)
            .IsRequired(false);

        builder.Property(e => e.Longitude)
            .HasColumnName("Longitude")
            .HasPrecision(9, 6)
            .IsRequired(false);

        builder.Property(e => e.LocationDescription)
            .HasColumnName("LocationDescription")
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(e => e.LocationType)
            .HasColumnName("LocationType")
            .HasMaxLength(100)
            .IsRequired(false);

        // WHEN - DocumentedAt is now non-nullable
        builder.Property(e => e.DocumentedAt)
            .HasColumnName("DocumentedAt")
            .IsRequired(true);

        builder.Property(e => e.PeriodStart)
            .HasColumnName("PeriodStart")
            .IsRequired(false);

        builder.Property(e => e.PeriodEnd)
            .HasColumnName("PeriodEnd")
            .IsRequired(false);

        builder.Property(e => e.Era)
            .HasColumnName("Era")
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(e => e.Season)
            .HasColumnName("Season")
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(e => e.TimeOfDay)
            .HasColumnName("TimeOfDay")
            .HasMaxLength(100)
            .IsRequired(false);

        // WHY
        builder.Property(e => e.Purpose)
            .HasColumnName("Purpose")
            .HasConversion<int>()
            .IsRequired(true);

        builder.Property(e => e.ResearchGoal)
            .HasColumnName("ResearchGoal")
            .HasColumnType("TEXT")
            .IsRequired(false);

        builder.Property(e => e.PreservationAction)
            .HasColumnName("PreservationAction")
            .HasColumnType("TEXT")
            .IsRequired(false);

        builder.Property(e => e.CommunityRequest)
            .HasColumnName("CommunityRequest")
            .HasColumnType("TEXT")
            .IsRequired(false);

        // HOW
        builder.Property(e => e.Methodology)
            .HasColumnName("Methodology")
            .HasConversion<int>()
            .IsRequired(true);

        builder.Property(e => e.Technique)
            .HasColumnName("Technique")
            .HasMaxLength(200)
            .IsRequired(false);

        builder.Property(e => e.Protocol)
            .HasColumnName("Protocol")
            .HasMaxLength(200)
            .IsRequired(false);

        builder.Property(e => e.Equipment)
            .HasColumnName("Equipment")
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(e => e.Software)
            .HasColumnName("Software")
            .HasMaxLength(200)
            .IsRequired(false);

        builder.Property(e => e.AudioRecordingId)
            .HasColumnName("AudioRecordingId")
            .HasColumnType("CHAR(36)")
            .IsRequired(false);

        builder.Property(e => e.VideoRecordingId)
            .HasColumnName("VideoRecordingId")
            .HasColumnType("CHAR(36)")
            .IsRequired(false);

        builder.Property(e => e.SourceId)
            .HasColumnName("SourceId")
            .HasColumnType("CHAR(36)")
            .IsRequired(false);

        // Metadata
        builder.Property(e => e.Notes)
            .HasColumnName("Notes")
            .HasColumnType("TEXT")
            .IsRequired(false);

        builder.Property(e => e.IsComplete)
            .HasColumnName("IsComplete")
            .IsRequired(true);

        // Indexes for common queries
        builder.HasIndex(e => e.SpeakerId)
            .HasDatabaseName("IX_W1HContexts_SpeakerId");

        builder.HasIndex(e => e.CommunityId)
            .HasDatabaseName("IX_W1HContexts_CommunityId");

        builder.HasIndex(e => e.LexemeId)
            .HasDatabaseName("IX_W1HContexts_LexemeId");

        builder.HasIndex(e => e.PhraseId)
            .HasDatabaseName("IX_W1HContexts_PhraseId");

        builder.HasIndex(e => e.GrammarRuleId)
            .HasDatabaseName("IX_W1HContexts_GrammarRuleId");

        builder.HasIndex(e => e.CulturalNoteId)
            .HasDatabaseName("IX_W1HContexts_CulturalNoteId");

        builder.HasIndex(e => e.RegionId)
            .HasDatabaseName("IX_W1HContexts_RegionId");

        builder.HasIndex(e => e.DocumentedAt)
            .HasDatabaseName("IX_W1HContexts_DocumentedAt");

        builder.HasIndex(e => e.Purpose)
            .HasDatabaseName("IX_W1HContexts_Purpose");

        builder.HasIndex(e => e.Methodology)
            .HasDatabaseName("IX_W1HContexts_Methodology");

        builder.HasIndex(e => e.IsComplete)
            .HasDatabaseName("IX_W1HContexts_IsComplete");

        builder.HasIndex(e => new { e.EntityType, e.LexemeId, e.PhraseId, e.GrammarRuleId, e.CulturalNoteId })
            .HasDatabaseName("IX_W1HContexts_EntityReference");

        builder.HasIndex(e => new { e.SpeakerId, e.CommunityId, e.DocumentedAt })
            .HasDatabaseName("IX_W1HContexts_WhoWhen");

        // Foreign key relationships
        builder.HasOne<SpeakerProfile>()
            .WithMany()
            .HasForeignKey(e => e.SpeakerId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("FK_W1HContexts_Speakers");

        builder.HasOne<Community>()
            .WithMany()
            .HasForeignKey(e => e.CommunityId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("FK_W1HContexts_Communities");

        builder.HasOne<Region>()
            .WithMany()
            .HasForeignKey(e => e.RegionId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("FK_W1HContexts_Regions");

        builder.HasOne<Lexeme>()
            .WithMany()
            .HasForeignKey(e => e.LexemeId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("FK_W1HContexts_Lexemes");

        builder.HasOne<Phrase>()
            .WithMany()
            .HasForeignKey(e => e.PhraseId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("FK_W1HContexts_Phrases");

        builder.HasOne<GrammarRule>()
            .WithMany()
            .HasForeignKey(e => e.GrammarRuleId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("FK_W1HContexts_GrammarRules");

        builder.HasOne<CulturalNote>()
            .WithMany()
            .HasForeignKey(e => e.CulturalNoteId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("FK_W1HContexts_CulturalNotes");

        builder.HasOne<AudioRecording>()
            .WithMany()
            .HasForeignKey(e => e.AudioRecordingId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("FK_W1HContexts_AudioRecordings");

        builder.HasOne<VideoRecording>()
            .WithMany()
            .HasForeignKey(e => e.VideoRecordingId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("FK_W1HContexts_VideoRecordings");

        builder.HasOne<Source>()
            .WithMany()
            .HasForeignKey(e => e.SourceId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("FK_W1HContexts_Sources");
    }
}