using Dashboard.Domain.Entities.Dimension.Analytical;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dashboard.Infrastructure.Data.Configurations.Dimension.Analytical;

public sealed class DimAnalyticalItemTranslationConfiguration
    : IEntityTypeConfiguration<DimAnalyticalItemTranslation>
{
    public void Configure(
        EntityTypeBuilder<DimAnalyticalItemTranslation> builder)
    {
        // ============================================================
        // TABLE
        // ============================================================

        builder.ToTable("dim_analytical_item_translation");

        // ============================================================
        // PRIMARY KEY
        // ============================================================

        builder.HasKey(x => x.TranslationId)
            .HasName("dim_analytical_item_translation_pkey");

        builder.Property(x => x.TranslationId)
            .HasColumnName("translation_id")
            .ValueGeneratedOnAdd();

        // ============================================================
        // ANALYTICAL ITEM
        // ============================================================

        builder.Property(x => x.AnalyticalItemId)
            .HasColumnName("analytical_item_id")
            .IsRequired();

        // ============================================================
        // LANGUAGE
        // ============================================================

        builder.Property(x => x.LanguageCode)
            .HasColumnName("language_code")
            .HasMaxLength(10)
            .IsRequired();

        // ============================================================
        // DISPLAY NAME
        // ============================================================

        builder.Property(x => x.ItemName)
            .HasColumnName("item_name")
            .HasMaxLength(255)
            .IsRequired();

        // ============================================================
        // DESCRIPTION
        // ============================================================

        builder.Property(x => x.Description)
            .HasColumnName("description");

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
        // FK: ANALYTICAL ITEM
        // ============================================================

        builder.HasOne(x => x.AnalyticalItem)
            .WithMany(x => x.Translations)
            .HasForeignKey(x => x.AnalyticalItemId)
            .HasConstraintName(
                "fk_analytical_item_translation_item")
            .OnDelete(DeleteBehavior.NoAction);

        // ============================================================
        // UNIQUE CONSTRAINT
        // UNIQUE (analytical_item_id, language_code)
        // ============================================================

        builder.HasIndex(x => new
        {
            x.AnalyticalItemId,
            x.LanguageCode
        })
        .IsUnique()
        .HasDatabaseName(
            "uq_analytical_item_translation_language");

        // ============================================================
        // INDEX: ANALYTICAL ITEM
        // ============================================================

        builder.HasIndex(x => x.AnalyticalItemId)
            .HasDatabaseName(
                "ix_analytical_item_translation_item");

        // ============================================================
        // INDEX: LANGUAGE
        // ============================================================

        builder.HasIndex(x => x.LanguageCode)
            .HasDatabaseName(
                "ix_analytical_item_translation_language");
    }
}