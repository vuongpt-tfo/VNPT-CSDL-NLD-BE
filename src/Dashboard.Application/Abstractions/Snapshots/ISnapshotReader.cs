namespace Dashboard.Application.Abstractions.Snapshots;

/// <summary>
/// Đọc dữ liệu Snapshot ở mức "business record" - đã join Identity +
/// Employment theo LabourId, phục vụ trực tiếp tầng Aggregate. Khác
/// Domain.Repositories.ISnapshotRepository (đọc entity thô, không
/// join).
/// </summary>
public interface ISnapshotReader
{
    Task<IReadOnlyList<LabourAggregationSourceRecord>> GetLabourRecordsAsync(
        CancellationToken cancellationToken);
}

/// <summary>
/// 1 bản ghi lao động đã join Identity + Employment, đủ field để
/// Domain.Rules.AnalyticalRuleEvaluator phân loại và
/// LabourAggregationService tổng hợp.
/// </summary>
public sealed class LabourAggregationSourceRecord
{
    public required long LabourId { get; init; }

    public required long IdentitySnapshotVersion { get; init; }

    public required long EmploymentSnapshotVersion { get; init; }

    /// <summary>
    /// Field dùng để evaluate DimAnalyticalRuleCondition.SourceField.
    /// Key hiện hỗ trợ: GenderCode, EthnicCode, MaritalStatus,
    /// EducationLevelCode, TechnicalLevelCode, EconomicActivitiesCode,
    /// ContractCode, JobTypesCode, EmployerSectorsCode, EverWorked,
    /// UnemploymentBenefitStatus, JobSearchWanted, UneconomicReasonCode,
    /// ParticipationFormCode, cộng "AgeYears" (tính động từ
    /// DateOfBirth, xem LabourAggregationService.ResolveField). Tên
    /// field trong dữ liệu seed của DimAnalyticalRuleCondition PHẢI
    /// khớp chính xác các key này.
    /// </summary>
    public required IReadOnlyDictionary<string, string?> FieldValues { get; init; }

    // Field cần trực tiếp cho dimension key/metric, không qua rule:
    public required string GenderCode { get; init; }

    public required DateTime DateOfBirth { get; init; }

    public string? PermanentAddressCode { get; init; }

    public string? EducationLevelCode { get; init; }

    public string? TechnicalLevelCode { get; init; }

    public string[]? IndustryCodes { get; init; }

    public string[]? PriorityCodes { get; init; }

    public string? OccupationCode { get; init; }
}
