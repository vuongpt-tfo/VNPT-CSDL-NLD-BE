namespace Dashboard.Domain.Entities.Dimension;

public sealed class DimCatalogItem
{
    public string CatalogName { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public int EffectiveFrom { get; set; }

    public int? EffectiveTo { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}