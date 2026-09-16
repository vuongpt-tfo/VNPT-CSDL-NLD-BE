using Dashboard.Domain.Entities.Dimension;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dashboard.Infrastructure.Data.Configurations.Dimension;

public sealed class DimAgeRangeConfiguration
    : IEntityTypeConfiguration<DimAgeRange>
{
    public void Configure(EntityTypeBuilder<DimAgeRange> builder)
    {
        builder.ToTable("dim_age_range");

        builder.HasKey(x => x.AgeRangeId);

        builder.Property(x => x.AgeRangeId)
            .HasColumnName("age_range_id")
            .ValueGeneratedNever();

        builder.Property(x => x.AgeFrom)
            .HasColumnName("age_from")
            .IsRequired();

        builder.Property(x => x.AgeTo)
            .HasColumnName("age_to")
            .IsRequired();

        builder.Property(x => x.Label)
            .HasColumnName("label")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Colour)
            .HasColumnName("colour")
            .HasMaxLength(50);

        builder.Property(x => x.Description)
            .HasColumnName("description");

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(x => new
        {
            x.AgeFrom,
            x.AgeTo
        })
        .IsUnique()
        .HasDatabaseName("uq_dim_age_range_from_to");
    }
}