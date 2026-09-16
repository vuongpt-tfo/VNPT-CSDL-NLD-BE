namespace Dashboard.Domain.Entities.Dimension;

public sealed class DimAdministrativeUnit
{
    public long AdministrativeUnitId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? ParentCode { get; set; }

    public short? UnitTypeId { get; set; }

    public bool IsActive { get; set; }

    public int EffectiveFrom { get; set; }

    public int? EffectiveTo { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}