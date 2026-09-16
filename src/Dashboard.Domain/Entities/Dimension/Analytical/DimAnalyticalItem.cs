namespace Dashboard.Domain.Entities.Dimension.Analytical;

public class DimAnalyticalItem
{
    public long AnalyticalItemId { get; set; }

    public long AnalyticalId { get; set; }

    public string ItemCode { get; set; } = null!;

    public string? ItemValue { get; set; }

    public long? ParentItemId { get; set; }

    public int SortOrder { get; set; }

    public decimal? RangeFrom { get; set; }

    public decimal? RangeTo { get; set; }

    public bool IsActive { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    // Navigation: Analytical Dimension sở hữu item
    public DimAnalytical Analytical { get; set; } = null!;

    // Navigation: quan hệ phân cấp
    public DimAnalyticalItem? ParentItem { get; set; }

    public ICollection<DimAnalyticalItem> Children { get; set; }
        = new List<DimAnalyticalItem>();

    // Navigation: bản dịch
    public ICollection<DimAnalyticalItemTranslation> Translations { get; set; }
    = new List<DimAnalyticalItemTranslation>();

    public ICollection<DimAnalyticalRuleCondition> RuleConditions { get; set; }
        = new List<DimAnalyticalRuleCondition>();
}