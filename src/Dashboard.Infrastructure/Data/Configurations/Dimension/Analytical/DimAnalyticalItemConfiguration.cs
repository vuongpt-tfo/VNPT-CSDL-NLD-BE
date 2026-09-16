using Dashboard.Domain.Entities.Dimension.Analytical;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dashboard.Infrastructure.Data.Configurations.Dimension.Analytical;

public sealed class DimAnalyticalItemConfiguration
    : IEntityTypeConfiguration<DimAnalyticalItem>
{
    public void Configure(EntityTypeBuilder<DimAnalyticalItem> builder)
    {
        // ============================================================
        // TABLE
        // ============================================================

        builder.ToTable("dim_analytical_item");

        // ============================================================
        // PRIMARY KEY
        // ============================================================

        builder.HasKey(x => x.AnalyticalItemId)
            .HasName("dim_analytical_item_pkey");

        builder.Property(x => x.AnalyticalItemId)
            .HasColumnName("analytical_item_id")
            .ValueGeneratedOnAdd();

        // ============================================================
        // ANALYTICAL DIMENSION
        // ============================================================

        builder.Property(x => x.AnalyticalId)
            .HasColumnName("analytical_id")
            .IsRequired();

        // ============================================================
        // BUSINESS ITEM
        // ============================================================

        builder.Property(x => x.ItemCode)
            .HasColumnName("item_code")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.ItemValue)
            .HasColumnName("item_value")
            .HasMaxLength(255);

        // ============================================================
        // HIERARCHY
        // ============================================================

        builder.Property(x => x.ParentItemId)
            .HasColumnName("parent_item_id");

        builder.Property(x => x.SortOrder)
            .HasColumnName("sort_order")
            .HasDefaultValue(0)
            .IsRequired();

        // ============================================================
        // RANGE
        // ============================================================

        builder.Property(x => x.RangeFrom)
            .HasColumnName("range_from")
            .HasColumnType("numeric");

        builder.Property(x => x.RangeTo)
            .HasColumnName("range_to")
            .HasColumnType("numeric");

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
        // FK: ANALYTICAL DIMENSION
        // ============================================================

        builder.HasOne(x => x.Analytical)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.AnalyticalId)
            .HasConstraintName(
                "fk_dim_analytical_item_analytical")
            .OnDelete(DeleteBehavior.NoAction);

        // ============================================================
        // FK: PARENT ITEM
        // ============================================================

        builder.HasOne(x => x.ParentItem)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentItemId)
            .HasConstraintName(
                "fk_dim_analytical_item_parent")
            .OnDelete(DeleteBehavior.NoAction);

        // ============================================================
        // UNIQUE CONSTRAINT
        // UNIQUE (analytical_id, item_code)
        // ============================================================

        builder.HasIndex(x => new
        {
            x.AnalyticalId,
            x.ItemCode
        })
        .IsUnique()
        .HasDatabaseName(
            "uq_dim_analytical_item_code");

        // ============================================================
        // INDEX: ANALYTICAL
        // ============================================================

        builder.HasIndex(x => x.AnalyticalId)
            .HasDatabaseName(
                "ix_dim_analytical_item_analytical");

        // ============================================================
        // INDEX: ITEM CODE
        // ============================================================

        builder.HasIndex(x => x.ItemCode)
            .HasDatabaseName(
                "ix_dim_analytical_item_code");

        // ============================================================
        // INDEX: PARENT
        // ============================================================

        builder.HasIndex(x => x.ParentItemId)
            .HasDatabaseName(
                "ix_dim_analytical_item_parent");

        // ============================================================
        // INDEX: ACTIVE ITEMS
        // ============================================================

        builder.HasIndex(x => new
        {
            x.AnalyticalId,
            x.IsActive
        })
        .HasFilter("is_active = TRUE")
        .HasDatabaseName(
            "ix_dim_analytical_item_active");

        // ============================================================
        // INDEX: SORT ORDER
        // ============================================================

        builder.HasIndex(x => new
        {
            x.AnalyticalId,
            x.SortOrder
        })
        .HasDatabaseName(
            "ix_dim_analytical_item_sort");
    }
}