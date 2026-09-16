using Dashboard.Domain.Entities.Dimension;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dashboard.Infrastructure.Data.Configurations.Dimension;

public sealed class DimOccupationConfiguration
    : IEntityTypeConfiguration<DimOccupation>
{
    public void Configure(EntityTypeBuilder<DimOccupation> builder)
    {
        builder.ToTable("dim_occupation");

        builder.HasKey(x => x.OccupationId);

        builder.Property(x => x.OccupationId)
            .HasColumnName("occupation_id")
            .ValueGeneratedNever();

        builder.Property(x => x.Code)
            .HasColumnName("code")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description");

        builder.Property(x => x.ParentCode)
            .HasColumnName("parent_code")
            .HasMaxLength(50);

        builder.Property(x => x.Level)
            .HasColumnName("level")
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Property(x => x.EffectiveFrom)
            .HasColumnName("effective_from")
            .IsRequired();

        builder.Property(x => x.EffectiveTo)
            .HasColumnName("effective_to");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(x => new
        {
            x.Code,
            x.EffectiveFrom
        })
        .IsUnique()
        .HasDatabaseName("uq_dim_occupation_code_effective");

        builder.HasIndex(x => x.ParentCode)
            .HasDatabaseName("ix_dim_occupation_parent");

        builder.HasIndex(x => x.Level)
            .HasDatabaseName("ix_dim_occupation_level");

        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("ix_dim_occupation_active");
    }
}