using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Configurations;

public class ExampleConfiguration : IEntityTypeConfiguration<Domain.Entities.Example>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Example> builder)
    {
        builder.ToTable("Examples");
        
        builder.Property(e => e.Id).HasColumnName("Id").IsRequired();
        builder.Property(e => e.LexemeId).HasColumnName("LexemeId").IsRequired();
        builder.Property(e => e.Text).HasColumnName("Text").HasMaxLength(5000).IsRequired();
        builder.Property(e => e.SpanishTranslation).HasColumnName("SpanishTranslation").HasMaxLength(5000);
        builder.Property(e => e.EnglishTranslation).HasColumnName("EnglishTranslation").HasMaxLength(5000);
        builder.Property(e => e.Context).HasColumnName("Context").HasMaxLength(2000);
        builder.Property(e => e.Source).HasColumnName("Source").HasMaxLength(500);
        builder.Property(e => e.VerificationStatus).HasColumnName("VerificationStatus").HasConversion<int>().IsRequired();
        builder.Property(e => e.Confidence).HasColumnName("Confidence").HasColumnType("decimal(3,2)").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").IsRequired();
        builder.HasIndex(e => e.LexemeId).HasDatabaseName("IX_Examples_LexemeId");
        
        builder.HasOne<Domain.Entities.Lexeme>()
            .WithMany(e => e.Examples)
            .HasForeignKey(e => e.LexemeId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(e => e.AudioRecordings)
            .WithOne()
            .HasForeignKey("ExampleId")
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(e => e.Evidence)
            .WithOne()
            .HasForeignKey("EntityId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
