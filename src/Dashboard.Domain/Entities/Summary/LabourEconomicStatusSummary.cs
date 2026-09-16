namespace Dashboard.Domain.Entities.Summary;

public class LabourEconomicStatusSummary
{
    public long SummaryId { get; set; }

    // ============================================================
    // KY THONG KE
    // ============================================================

    public DateOnly ReferencePeriod { get; set; }

    // ============================================================
    // DIMENSION KEYS
    // ============================================================

    public string AdministrativeUnitCode { get; set; } = null!;

    public string EducationLevelCode { get; set; } = null!;

    public string TechnicalLevelCode { get; set; } = null!;

    public string EconomicStatusCode { get; set; } = null!;

    // ============================================================
    // METRICS
    // ============================================================

    public long LabourCount { get; set; }

    public string ByPriority { get; set; } = "{}";

    // D01-W04: map gender_code -> so luong lao dong (phu tro, khong
    // doi grain). Catalog GENDER (MALE/FEMALE/UNKNOWN) - KHONG suy ra
    // FEMALE = LabourCount - MALE nua, vi gender_code khong dam bao
    // chi co 2 gia tri (xem dimension-init.sql).
    public string ByGender { get; set; } = "{}";

    // D01-W07: map industry_code -> so luong lao dong (phu tro,
    // khong doi grain). Chi co gia tri khi EconomicStatusCode ==
    // "EMP".
    public string ByIndustry { get; set; } = "{}";

    // D01-W05: map age_group_code -> so luong lao dong (phu tro,
    // khong doi grain).
    public string ByAgeGroup { get; set; } = "{}";

    // ============================================================
    // LINEAGE
    // ============================================================

    public long SourceIdentitySnapshotVersion { get; set; }

    public long SourceEmploymentSnapshotVersion { get; set; }

    public int? AnalyticalDefinitionVersion { get; set; }

    public long? ProcessingRunId { get; set; }

    public DateTimeOffset AggregatedAt { get; set; }
}
