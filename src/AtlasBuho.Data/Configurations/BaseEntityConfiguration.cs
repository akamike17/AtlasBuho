using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities;

namespace AtlasBuho.Data.Configurations;

public abstract class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : class
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        // All entities have Id property
        builder.HasKey("Id");
        
        builder.Property("CreatedAt")
            .IsRequired()
            .HasDefaultValueSql("UTC_TIMESTAMP()");
            
        builder.Property("UpdatedAt")
            .IsRequired()
            .HasDefaultValueSql("UTC_TIMESTAMP()")
            .ValueGeneratedOnAddOrUpdate();
    }
}
