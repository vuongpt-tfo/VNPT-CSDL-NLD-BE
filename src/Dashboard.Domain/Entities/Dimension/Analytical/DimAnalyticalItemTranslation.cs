namespace Dashboard.Domain.Entities.Dimension.Analytical;

public class DimAnalyticalItemTranslation
{
    public long TranslationId { get; set; }

    public long AnalyticalItemId { get; set; }

    public string LanguageCode { get; set; } = null!;

    public string ItemName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    // Navigation
    public DimAnalyticalItem AnalyticalItem { get; set; } = null!;
}