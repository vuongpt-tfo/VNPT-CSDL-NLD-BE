namespace Dashboard.Domain.Entities.Dimension.Analytical;

public class DimAnalytical
{
    public long AnalyticalId { get; set; }

    public string AnalyticalCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string DimensionType { get; set; } = null!;

    public int Version { get; set; }

    public DateOnly EffectiveFrom { get; set; }

    public DateOnly? EffectiveTo { get; set; }

    public bool IsActive { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public ICollection<DimAnalyticalItem> Items { get; set; }
        = new List<DimAnalyticalItem>();

    public ICollection<DimAnalyticalRule> Rules { get; set; }
        = new List<DimAnalyticalRule>();
}