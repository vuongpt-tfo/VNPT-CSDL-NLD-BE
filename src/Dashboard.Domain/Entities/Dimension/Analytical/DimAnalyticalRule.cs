namespace Dashboard.Domain.Entities.Dimension.Analytical;

public class DimAnalyticalRule
{
    public long AnalyticalRuleId { get; set; }

    public long AnalyticalId { get; set; }

    public string RuleCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int Version { get; set; }

    public DateOnly EffectiveFrom { get; set; }

    public DateOnly? EffectiveTo { get; set; }

    public bool IsActive { get; set; }

    public string? Configuration { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    // Navigation: Analytical Dimension sở hữu rule
    public DimAnalytical Analytical { get; set; } = null!;

    // Navigation: các điều kiện của rule
    public ICollection<DimAnalyticalRuleCondition> Conditions { get; set; }
        = new List<DimAnalyticalRuleCondition>();
}