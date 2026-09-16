using Dashboard.Domain.Entities.Dimension.Analytical;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dashboard.Infrastructure.Data.Configurations.Dimension.Analytical;

public sealed class DimAnalyticalConfiguration
    : IEntityTypeConfiguration<DimAnalytical>
{
    public void Configure(EntityTypeBuilder<DimAnalytical> builder)
    {
        // ============================================================
        // TABLE
        // ============================================================

        builder.ToTable("dim_analytical");

        // ============================================================
        // PRIMARY KEY
        // ============================================================

        builder.HasKey(x => x.AnalyticalId)
            .HasName("dim_analytical_pkey");

        builder.Property(x => x.AnalyticalId)
            .HasColumnName("analytical_id")
            .ValueGeneratedOnAdd();

        // ============================================================
        // BUSINESS IDENTITY
        // ============================================================

        builder.Property(x => x.AnalyticalCode)
            .HasColumnName("analytical_code")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description");

        // ============================================================
        // DIMENSION TYPE
        // ============================================================

        builder.Property(x => x.DimensionType)
            .HasColumnName("dimension_type")
            .HasMaxLength(50)
            .IsRequired();

        // ============================================================
        // VERSION
        // ============================================================

        builder.Property(x => x.Version)
            .HasColumnName("version")
            .HasDefaultValue(1)
            .IsRequired();

        // ============================================================
        // EFFECTIVE PERIOD
        // ============================================================

        builder.Property(x => x.EffectiveFrom)
            .HasColumnName("effective_from")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(x => x.EffectiveTo)
            .HasColumnName("effective_to")
            .HasColumnType("date");

        // ============================================================
        // STATUS
        // ============================================================

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true)
            .IsRequired();

        // ============================================================
        // AUDIT
        // ============================================================

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        // ============================================================
        // UNIQUE BUSINESS IDENTITY
        // ============================================================

        builder.HasIndex(x => new
        {
            x.AnalyticalCode,
            x.Version
        })
        .IsUnique()
        .HasDatabaseName(
            "uq_dim_analytical_code_version");

        // ============================================================
        // INDEXES
        // ============================================================

        builder.HasIndex(x => x.AnalyticalCode)
            .HasDatabaseName(
                "ix_dim_analytical_code");

        builder.HasIndex(x => x.IsActive)
            .HasFilter("is_active = TRUE")
            .HasDatabaseName(
                "ix_dim_analytical_active");

        builder.HasIndex(x => x.DimensionType)
            .HasDatabaseName(
                "ix_dim_analytical_dimension_type");

        builder.HasIndex(x => new
        {
            x.AnalyticalCode,
            x.EffectiveFrom,
            x.EffectiveTo
        })
        .HasDatabaseName(
            "ix_dim_analytical_effective_period");

        // ============================================================
        // RELATIONSHIPS
        // ============================================================

        builder.HasMany(x => x.Items)
            .WithOne(x => x.Analytical)
            .HasForeignKey(x => x.AnalyticalId)
            .HasConstraintName(
                "fk_dim_analytical_item_analytical")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Rules)
            .WithOne(x => x.Analytical)
            .HasForeignKey(x => x.AnalyticalId)
            .HasConstraintName(
                "fk_analytical_rule_analytical")
            .OnDelete(DeleteBehavior.Restrict);
    }
}