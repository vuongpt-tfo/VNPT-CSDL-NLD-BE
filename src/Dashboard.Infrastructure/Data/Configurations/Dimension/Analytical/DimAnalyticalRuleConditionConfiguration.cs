using Dashboard.Domain.Entities.Dimension.Analytical;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dashboard.Infrastructure.Data.Configurations.Dimension.Analytical;

public sealed class DimAnalyticalRuleConditionConfiguration
    : IEntityTypeConfiguration<DimAnalyticalRuleCondition>
{
    public void Configure(
        EntityTypeBuilder<DimAnalyticalRuleCondition> builder)
    {
        // ============================================================
        // TABLE
        // ============================================================

        builder.ToTable("dim_analytical_rule_condition");

        // ============================================================
        // PRIMARY KEY
        // ============================================================

        builder.HasKey(x => x.AnalyticalRuleConditionId)
            .HasName("dim_analytical_rule_condition_pkey");

        builder.Property(x => x.AnalyticalRuleConditionId)
            .HasColumnName("analytical_rule_condition_id")
            .ValueGeneratedOnAdd();

        // ============================================================
        // ANALYTICAL RULE
        // ============================================================

        builder.Property(x => x.AnalyticalRuleId)
            .HasColumnName("analytical_rule_id")
            .IsRequired();

        // ============================================================
        // ANALYTICAL ITEM
        // ============================================================

        builder.Property(x => x.AnalyticalItemId)
            .HasColumnName("analytical_item_id")
            .IsRequired();

        // ============================================================
        // CONDITION ORDER
        // ============================================================

        builder.Property(x => x.ConditionOrder)
            .HasColumnName("condition_order")
            .HasDefaultValue(0)
            .IsRequired();

        // ============================================================
        // SOURCE FIELD
        // ============================================================

        builder.Property(x => x.SourceField)
            .HasColumnName("source_field")
            .HasMaxLength(255)
            .IsRequired();

        // ============================================================
        // OPERATOR
        // ============================================================

        builder.Property(x => x.Operator)
            .HasColumnName("operator")
            .HasMaxLength(30)
            .IsRequired();

        // ============================================================
        // VALUE FROM
        // ============================================================

        builder.Property(x => x.ValueFrom)
            .HasColumnName("value_from")
            .HasMaxLength(255);

        // ============================================================
        // VALUE TO
        // ============================================================

        builder.Property(x => x.ValueTo)
            .HasColumnName("value_to")
            .HasMaxLength(255);

        // ============================================================
        // VALUES
        // ============================================================

        builder.Property(x => x.Values)
            .HasColumnName("values")
            .HasColumnType("jsonb");

        // ============================================================
        // AUDIT
        // ============================================================

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        // ============================================================
        // FK: ANALYTICAL RULE
        // ============================================================

        builder.HasOne(x => x.AnalyticalRule)
            .WithMany(x => x.Conditions)
            .HasForeignKey(x => x.AnalyticalRuleId)
            .HasConstraintName(
                "fk_analytical_rule_condition_rule")
            .OnDelete(DeleteBehavior.NoAction);

        // ============================================================
        // FK: ANALYTICAL ITEM
        // ============================================================

        builder.HasOne(x => x.AnalyticalItem)
            .WithMany(x => x.RuleConditions)
            .HasForeignKey(x => x.AnalyticalItemId)
            .HasConstraintName(
                "fk_analytical_rule_condition_item")
            .OnDelete(DeleteBehavior.NoAction);

        // ============================================================
        // INDEX: ANALYTICAL RULE
        // ============================================================

        builder.HasIndex(x => x.AnalyticalRuleId)
            .HasDatabaseName(
                "ix_analytical_rule_condition_rule");

        // ============================================================
        // INDEX: ANALYTICAL ITEM
        // ============================================================

        builder.HasIndex(x => x.AnalyticalItemId)
            .HasDatabaseName(
                "ix_analytical_rule_condition_item");

        // ============================================================
        // INDEX: RULE + CONDITION ORDER
        // ============================================================

        builder.HasIndex(x => new
        {
            x.AnalyticalRuleId,
            x.ConditionOrder
        })
        .HasDatabaseName(
            "ix_analytical_rule_condition_order");

        // ============================================================
        // INDEX: SOURCE FIELD
        // ============================================================

        builder.HasIndex(x => x.SourceField)
            .HasDatabaseName(
                "ix_analytical_rule_condition_source_field");
    }
}