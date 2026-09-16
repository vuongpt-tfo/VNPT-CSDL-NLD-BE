namespace Dashboard.Domain.Entities.Dimension;

public sealed class DimAgeRange
{
    public int AgeRangeId { get; set; }

    public short AgeFrom { get; set; }

    public short AgeTo { get; set; }

    public string Label { get; set; } = string.Empty;

    public string? Colour { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}