using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Configurations;

public class DialectRelationshipConfiguration : IEntityTypeConfiguration<Domain.Entities.DialectRelationship>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.DialectRelationship> builder)
    {
        builder.ToTable("DialectRelationships");
        
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.SourceLanguageVariantId).HasColumnName("SourceLanguageVariantId").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.TargetLanguageVariantId).HasColumnName("TargetLanguageVariantId").HasColumnType("CHAR(36)").IsRequired();
        builder.Property(e => e.RelationshipType).HasColumnName("RelationshipType").HasConversion<int>().IsRequired();
        builder.Property(e => e.IntelligibilityScore).HasColumnName("IntelligibilityScore").HasColumnType("decimal(3,2)");
        builder.Property(e => e.Description).HasColumnName("Description").HasColumnType("TEXT");
        builder.Property(e => e.Source).HasColumnName("Source").HasMaxLength(500);
        builder.Property(e => e.VerificationStatus).HasColumnName("VerificationStatus").HasConversion<int>().IsRequired();
        builder.Property(e => e.Confidence).HasColumnName("Confidence").HasColumnType("decimal(3,2)").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").IsRequired();
        builder.HasIndex(e => e.SourceLanguageVariantId).HasDatabaseName("IX_DialectRelationships_SourceVariantId");
        builder.HasIndex(e => e.TargetLanguageVariantId).HasDatabaseName("IX_DialectRelationships_TargetVariantId");
        builder.HasIndex(e => new { e.SourceLanguageVariantId, e.TargetLanguageVariantId }).IsUnique().HasDatabaseName("IX_DialectRelationships_Source_Target");
        
        builder.HasOne<Domain.Entities.LanguageVariant>()
            .WithMany(e => e.DialectRelationships)
            .HasForeignKey(e => e.SourceLanguageVariantId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne<Domain.Entities.LanguageVariant>()
            .WithMany()
            .HasForeignKey(e => e.TargetLanguageVariantId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasMany(e => e.Evidence)
            .WithOne()
            .HasForeignKey("EntityId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
