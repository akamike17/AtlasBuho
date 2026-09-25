using AtlasBuho.Domain.AiReview;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtlasBuho.Data.Configurations;

/// <summary>
/// EF Core configuration for AiTranslationReview (B10).
/// AI reviews are audit records — never canonical evidence.
/// </summary>
public sealed class AiTranslationReviewConfiguration : IEntityTypeConfiguration<AiTranslationReview>
{
    public void Configure(EntityTypeBuilder<AiTranslationReview> builder)
    {
        builder.ToTable("AiTranslationReviews");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime(6)");

        builder.Property(e => e.InputText)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(e => e.SourceLanguageRequested)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.TargetLanguageRequested)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.DetectedLanguage)
            .HasMaxLength(100);

        builder.Property(e => e.DetectedLanguageConfidence)
            .HasPrecision(5, 4);

        builder.Property(e => e.AtlasBuhoV1Result)
            .HasMaxLength(2000);

        builder.Property(e => e.ReviewStatus)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(e => e.ProposedCorrection)
            .HasMaxLength(2000);

        builder.Property(e => e.Explanation)
            .HasColumnType("text");

        builder.Property(e => e.EvidenceNotes)
            .HasColumnType("text");

        builder.Property(e => e.ProviderName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.ModelName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.PromptVersion)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.ResponseSchemaVersion)
            .HasMaxLength(20)
            .HasDefaultValue("v1");

        builder.Property(e => e.LatencyMs);

        builder.Property(e => e.Success)
            .IsRequired();

        builder.Property(e => e.ErrorCode)
            .HasMaxLength(100);

        builder.Property(e => e.CorrelationId);

        builder.Property(e => e.Lifecycle)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(e => e.ReviewedAt)
            .HasColumnType("datetime(6)");

        builder.Property(e => e.HumanReviewNotes)
            .HasColumnType("text");

        builder.Property(e => e.PromotedAt)
            .HasColumnType("datetime(6)");

        builder.Property(e => e.V2CandidateId);

        // Indexes for audit queries
        builder.HasIndex(e => e.CreatedAt);
        builder.HasIndex(e => e.ReviewStatus);
        builder.HasIndex(e => e.Lifecycle);
        builder.HasIndex(e => e.ProviderName);
        builder.HasIndex(e => e.CorrelationId);

        // CRITICAL: No FKs to canonical tables — AI reviews must not reference Lexemes/Meanings directly
        // V2CandidateId is a soft reference only (no FK constraint to prevent canonical coupling)
    }
}

/// <summary>
/// EF Core configuration for V2Candidate (B10 §14).
/// Candidates are extracted from accepted AI reviews.
/// </summary>
public sealed class V2CandidateConfiguration : IEntityTypeConfiguration<V2Candidate>
{
    public void Configure(EntityTypeBuilder<V2Candidate> builder)
    {
        builder.ToTable("V2Candidates");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime(6)");

        builder.Property(e => e.AiReviewId)
            .IsRequired();

        builder.Property(e => e.CanonicalForm)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.SpanishMeaning)
            .HasMaxLength(2000);

        builder.Property(e => e.TargetLanguage)
            .HasMaxLength(100);

        builder.Property(e => e.TargetText)
            .HasMaxLength(2000);

        builder.Property(e => e.SourceVariant)
            .HasMaxLength(100);

        builder.Property(e => e.OriginalConfidence)
            .HasPrecision(5, 4);

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(100);

        builder.Property(e => e.PromotionEvidenceId);

        builder.Property(e => e.IsPromoted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.PromotedAt)
            .HasColumnType("datetime(6)");

        // Indexes
        builder.HasIndex(e => e.AiReviewId);
        builder.HasIndex(e => e.IsPromoted);
        builder.HasIndex(e => e.CreatedAt);

        // Relationship to AiTranslationReview (one review can spawn one candidate)
        builder.HasOne(e => e.AiReview)
            .WithMany()
            .HasForeignKey(e => e.AiReviewId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
