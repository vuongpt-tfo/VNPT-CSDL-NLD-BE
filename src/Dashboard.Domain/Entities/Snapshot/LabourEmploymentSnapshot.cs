namespace Dashboard.Domain.Entities.Snapshot;

public class LabourEmploymentSnapshot
{
    // ============================================================
    // IDENTITY
    // ============================================================

    /// <summary>
    /// Định danh người lao động từ Labour Master.
    /// Là khóa chính của Employment Snapshot.
    /// </summary>
    public long LabourId { get; set; }

    // ============================================================
    // BASIC
    // ============================================================

    public string? SocialInsuranceNumber { get; set; }

    // ============================================================
    // EDUCATION / TRAINING / SKILL
    // ============================================================

    public string? EducationLevelCode { get; set; }

    public string? TechnicalLevelCode { get; set; }

    public string? MajorCode { get; set; }

    public string? MajorDescription { get; set; }

    public string? SkillIndustryCode { get; set; }

    public string? SkillLevelCode { get; set; }

    // ============================================================
    // ECONOMIC ACTIVITY
    // ============================================================

    public short EconomicActivitiesCode { get; set; }

    // ============================================================
    // EMPLOYED
    // ============================================================

    public string? PositionGroupsCode { get; set; }

    public string? OccupationCode { get; set; }

    public string? ContractCode { get; set; }

    /// <summary>
    /// Ngày hiệu lực/trạng thái việc làm từ dữ liệu nguồn.
    /// Có thể semantic-map tới 03_time.dim_date.
    /// Không tạo physical FK tới Time Dimension.
    /// </summary>
    public DateTime? EmploymentStatusDate { get; set; }

    public string? WorkLocation { get; set; }

    public string? JobTypesCode { get; set; }

    public string? JobTitle { get; set; }

    public int? TotalWorkExperience { get; set; }

    public int? NumberOfJobs { get; set; }

    public string? EmployerCode { get; set; }

    public string[]? IndustryCodes { get; set; }

    public bool? IndustrialZoneStatus { get; set; }

    public string? EmployerSectorsCode { get; set; }

    // ============================================================
    // UNEMPLOYED
    // ============================================================

    public bool? EverWorked { get; set; }

    /// <summary>
    /// Ngày bắt đầu trạng thái thất nghiệp.
    /// Có thể semantic-map tới 03_time.dim_date.
    /// </summary>
    public DateTime? UnemploymentStartDate { get; set; }

    public int? UnemploymentDuration { get; set; }

    public bool? UnemploymentBenefitStatus { get; set; }

    public string? UnemploymentReason { get; set; }

    public bool? JobSearchWanted { get; set; }

    public string? JobSearchStatus { get; set; }

    // ============================================================
    // NOT PARTICIPATING IN ECONOMIC ACTIVITY
    // ============================================================

    public string? UneconomicReasonCode { get; set; }

    // ============================================================
    // SOCIAL INSURANCE
    // ============================================================

    public string? ParticipationFormCode { get; set; }

    /// <summary>
    /// Ngày bắt đầu tham gia bảo hiểm.
    /// Có thể semantic-map tới 03_time.dim_date.
    /// </summary>
    public DateTime? SiStartDate { get; set; }

    /// <summary>
    /// Ngày kết thúc tham gia bảo hiểm.
    /// Có thể semantic-map tới 03_time.dim_date.
    /// </summary>
    public DateTime? SiEndDate { get; set; }

    // ============================================================
    // PRIORITY
    // ============================================================

    public string[]? PriorityCodes { get; set; }

    // ============================================================
    // SNAPSHOT METADATA
    // ============================================================

    /// <summary>
    /// Thời điểm dữ liệu nguồn được cập nhật.
    /// Dùng cho freshness, incremental synchronization và watermark.
    ///
    /// PostgreSQL: TIMESTAMPTZ.
    /// Có thể semantic-map phần date tới 03_time.dim_date
    /// khi cần phân tích freshness.
    /// </summary>
    public DateTimeOffset? SourceUpdatedAt { get; set; }

    /// <summary>
    /// Version của source/projection đã được xử lý.
    ///
    /// Không phải Catalog Version.
    /// Không phải business history version.
    /// Không có quan hệ với Time Dimension.
    /// </summary>
    public long SnapshotVersion { get; set; }

    /// <summary>
    /// Thời điểm technical row được ghi/cập nhật vào Snapshot.
    ///
    /// Không phải business effective date.
    /// Không phải analytical period.
    ///
    /// Semantic mapping:
    ///     DATE(snapshot_at) -> 03_time.dim_date
    ///     TIME(snapshot_at) -> 03_time.dim_time
    ///
    /// Không tạo physical FK tới Time Dimension.
    /// </summary>
    public DateTimeOffset SnapshotAt { get; set; }
}