namespace Dashboard.Domain.Entities.Summary;

public class LabourTimeSummary
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

    // ============================================================
    // METRICS
    // ============================================================

    public long WorkingAgeLabourCount { get; set; }

    public long OutWorkingAgeLabourCount { get; set; }

    public long TotalLabourCount { get; set; }

    // ============================================================
    // REVISION / BACKFILL (D01-W08)
    // ============================================================

    public long? PreviousWorkingAgeLabourCount { get; set; }

    public long? PreviousOutWorkingAgeLabourCount { get; set; }

    public long? PreviousTotalLabourCount { get; set; }

    public bool IsRevised { get; set; }

    public DateTimeOffset? RevisedAt { get; set; }

    // ============================================================
    // QUY TAC PHAN LOAI
    // ============================================================

    public int? AnalyticalDefinitionVersion { get; set; }

    public short WorkingAgeFrom { get; set; }

    public short WorkingAgeTo { get; set; }

    // ============================================================
    // LINEAGE
    // ============================================================

    public long SourceIdentitySnapshotVersion { get; set; }

    public long SourceEmploymentSnapshotVersion { get; set; }

    public long? ProcessingRunId { get; set; }

    public DateTimeOffset AggregatedAt { get; set; }
}
