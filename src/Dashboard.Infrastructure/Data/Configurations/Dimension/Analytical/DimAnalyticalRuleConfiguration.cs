using Dashboard.Domain.Entities.Dimension.Analytical;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dashboard.Infrastructure.Data.Configurations.Dimension.Analytical;

public sealed class DimAnalyticalRuleConfiguration
    : IEntityTypeConfiguration<DimAnalyticalRule>
{
    public void Configure(
        EntityTypeBuilder<DimAnalyticalRule> builder)
    {
        // ============================================================
        // TABLE
        // ============================================================

        builder.ToTable("dim_analytical_rule");

        // ============================================================
        // PRIMARY KEY
        // ============================================================

        builder.HasKey(x => x.AnalyticalRuleId)
            .HasName("dim_analytical_rule_pkey");

        builder.Property(x => x.AnalyticalRuleId)
            .HasColumnName("analytical_rule_id")
            .ValueGeneratedOnAdd();

        // ============================================================
        // ANALYTICAL DIMENSION
        // ============================================================

        builder.Property(x => x.AnalyticalId)
            .HasColumnName("analytical_id")
            .IsRequired();

        // ============================================================
        // BUSINESS IDENTITY
        // ============================================================

        builder.Property(x => x.RuleCode)
            .HasColumnName("rule_code")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description");

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
        // CONFIGURATION
        // ============================================================

        builder.Property(x => x.Configuration)
            .HasColumnName("configuration")
            .HasColumnType("jsonb");

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
        // FK: ANALYTICAL DIMENSION
        // ============================================================

        builder.HasOne(x => x.Analytical)
            .WithMany(x => x.Rules)
            .HasForeignKey(x => x.AnalyticalId)
            .HasConstraintName(
                "fk_analytical_rule_analytical")
            .OnDelete(DeleteBehavior.NoAction);

        // ============================================================
        // UNIQUE CONSTRAINT
        // UNIQUE (analytical_id, rule_code, version)
        // ============================================================

        builder.HasIndex(x => new
        {
            x.AnalyticalId,
            x.RuleCode,
            x.Version
        })
        .IsUnique()
        .HasDatabaseName(
            "uq_analytical_rule_code_version");

        // ============================================================
        // INDEX: ANALYTICAL
        // ============================================================

        builder.HasIndex(x => x.AnalyticalId)
            .HasDatabaseName(
                "ix_analytical_rule_analytical");

        // ============================================================
        // INDEX: RULE CODE
        // ============================================================

        builder.HasIndex(x => x.RuleCode)
            .HasDatabaseName(
                "ix_analytical_rule_code");

        // ============================================================
        // INDEX: ACTIVE RULES
        // ============================================================

        builder.HasIndex(x => new
        {
            x.AnalyticalId,
            x.IsActive
        })
        .HasFilter("is_active = TRUE")
        .HasDatabaseName(
            "ix_analytical_rule_active");

        // ============================================================
        // INDEX: EFFECTIVE PERIOD
        // ============================================================

        builder.HasIndex(x => new
        {
            x.AnalyticalId,
            x.EffectiveFrom,
            x.EffectiveTo
        })
        .HasDatabaseName(
            "ix_analytical_rule_effective_period");

        // ============================================================
        // CONDITIONS
        // ============================================================

        builder.HasMany(x => x.Conditions)
            .WithOne(x => x.AnalyticalRule)
            .HasForeignKey(x => x.AnalyticalRuleId)
            .HasConstraintName(
                "fk_analytical_rule_condition_rule")
            .OnDelete(DeleteBehavior.NoAction);
    }
}