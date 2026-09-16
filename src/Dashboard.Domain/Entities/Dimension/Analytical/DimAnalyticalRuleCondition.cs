namespace Dashboard.Domain.Entities.Dimension.Analytical;

public class DimAnalyticalRuleCondition
{
    public long AnalyticalRuleConditionId { get; set; }

    public long AnalyticalRuleId { get; set; }

    public long AnalyticalItemId { get; set; }

    public int ConditionOrder { get; set; }

    public string SourceField { get; set; } = null!;

    public string Operator { get; set; } = null!;

    public string? ValueFrom { get; set; }

    public string? ValueTo { get; set; }

    public string? Values { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    // Navigation: Analytical Rule chứa condition
    public DimAnalyticalRule AnalyticalRule { get; set; } = null!;

    // Navigation: Analytical Item mà condition phân loại vào
    public DimAnalyticalItem AnalyticalItem { get; set; } = null!;
}