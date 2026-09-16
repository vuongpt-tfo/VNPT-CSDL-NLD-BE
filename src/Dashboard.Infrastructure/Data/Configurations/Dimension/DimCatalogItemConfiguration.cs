using Dashboard.Domain.Entities.Dimension;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dashboard.Infrastructure.Data.Configurations.Dimension;

public sealed class DimCatalogItemConfiguration
    : IEntityTypeConfiguration<DimCatalogItem>
{
    public void Configure(EntityTypeBuilder<DimCatalogItem> builder)
    {
        builder.ToTable("dim_catalog_items");

        builder.HasKey(x => new
        {
            x.CatalogName,
            x.Code,
            x.EffectiveFrom
        });

        builder.Property(x => x.CatalogName)
            .HasColumnName("catalog_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Code)
            .HasColumnName("code")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description");

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
            x.CatalogName,
            x.IsActive
        })
        .HasDatabaseName("ix_dim_catalog_items_active");
    }
}