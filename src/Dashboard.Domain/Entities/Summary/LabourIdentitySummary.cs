namespace Dashboard.Domain.Entities.Summary;

public class LabourIdentitySummary
{
    public long SummaryId { get; set; }

    // ============================================================
    // DIMENSION KEYS
    // ============================================================

    public string AdministrativeUnitCode { get; set; } = null!;

    public string EducationLevelCode { get; set; } = null!;

    public string TechnicalLevelCode { get; set; } = null!;

    public string TopLevelOccupationCode { get; set; } = null!;

    public string AgeGroupCode { get; set; } = null!;

    public string EconomicStatusCode { get; set; } = null!;

    // ============================================================
    // METRICS
    // ============================================================

    public long LabourCount { get; set; }

    public string ByPriority { get; set; } = "{}";

    // D01-W04: so luong lao dong nam trong cung grain (phu tro,
    // khong doi grain). Nu suy ra = LabourCount - MaleCount.
    public long? MaleCount { get; set; }

    // D01-W07: map industry_code -> so luong lao dong (phu tro,
    // khong doi grain). Chi co gia tri khi EconomicStatusCode ==
    // "EMP".
    public string ByIndustry { get; set; } = "{}";

    // ============================================================
    // LINEAGE
    // ============================================================

    public long SourceIdentitySnapshotVersion { get; set; }

    public long SourceEmploymentSnapshotVersion { get; set; }

    public int? AnalyticalDefinitionVersion { get; set; }

    public long? ProcessingRunId { get; set; }

    public DateTimeOffset AggregatedAt { get; set; }
}