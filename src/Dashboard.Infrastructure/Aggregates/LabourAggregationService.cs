using Dashboard.Application.Abstractions.Aggregation;
using Dashboard.Application.Abstractions.Dimension;
using Dashboard.Application.Abstractions.Snapshots;
using Dashboard.Domain.Entities.Dimension.Analytical;
using Dashboard.Domain.Entities.Summary;
using Dashboard.Domain.Repositories;
using Dashboard.Domain.Rules;
using System.Text.Json;

namespace Dashboard.Infrastructure.Aggregates;

/// <summary>
/// Implementation của IAggregationService: Snapshot -> phân loại (rule
/// engine) -> Summary. Xem docs/decisions/decision-log.md cho bối cảnh
/// kiến trúc; các điểm còn phụ thuộc số liệu/quy tắc chính thức từ
/// VNPT được đánh dấu TODO rõ trong file này - KHÔNG dùng số liệu do
/// TODO sinh ra cho báo cáo chính thức trước khi xác nhận lại.
/// </summary>
public sealed class LabourAggregationService : IAggregationService
{
    // Ma DimAnalytical dung de phan loai - phai khop du lieu seed trong
    // dim_analytical.analytical_code. "AGE_GROUP" da duoc dung o tang
    // doc (xem LabourDashboardReadRepository.GetByAgeAsync).
    // "ECONOMIC_STATUS" la ten suy doan hop ly, CAN xac nhan lai dung
    // ma seed thuc te.
    private const string EconomicStatusAnalyticalCode = "ECONOMIC_STATUS";
    private const string AgeGroupAnalyticalCode = "AGE_GROUP";

    // Gia tri EconomicStatusCode da duoc dung truc tiep o tang doc (xem
    // LabourDashboardReadRepository.GetEconomicStatusCountsAsync) - tang
    // Aggregate PHAI sinh dung 3 gia tri nay de tang doc hoat dong dung.
    private const string EmployedCode = "EMP";
    private const string UnemployedCode = "UNE";
    private const string NotInLabourForceCode = "INA";

    // TODO (can VNPT xac nhan - xem "official KPI formulas" trong
    // blocker cua du an): do tuoi lao dong dung de tinh
    // LabourTimeSummary.WorkingAgeFrom/To. Gia tri duoi day chi la mac
    // dinh tham khao theo dinh nghia "luc luong lao dong" pho bien cua
    // GSO (>= 15 tuoi); WorkingAgeTo thuc te phu thuoc gioi tinh va lo
    // trinh tuoi nghi huu (dang thay doi theo Bo luat Lao dong) nen
    // KHONG the co 1 gia tri co dinh dung cho ca 2 gioi.
    private const short DefaultWorkingAgeFrom = 15;
    private const short DefaultWorkingAgeTo = 60;

    private readonly ISnapshotReader _snapshotReader;
    private readonly IDimensionReader _dimensionReader;
    private readonly ISummaryRepository _summaryRepository;

    public LabourAggregationService(
        ISnapshotReader snapshotReader,
        IDimensionReader dimensionReader,
        ISummaryRepository summaryRepository)
    {
        _snapshotReader = snapshotReader;
        _dimensionReader = dimensionReader;
        _summaryRepository = summaryRepository;
    }

    public async Task<AggregationRunResult> RunAsync(
        DateOnly referencePeriod,
        CancellationToken cancellationToken)
    {
        var startedAt = DateTimeOffset.UtcNow;

        var records = await _snapshotReader.GetLabourRecordsAsync(cancellationToken);

        var economicStatusRules = await _dimensionReader.GetActiveRulesAsync(
            EconomicStatusAnalyticalCode, cancellationToken);

        var ageGroupRules = await _dimensionReader.GetActiveRulesAsync(
            AgeGroupAnalyticalCode, cancellationToken);

        var analyticalDefinitionVersion = await _dimensionReader.GetActiveVersionAsync(
            EconomicStatusAnalyticalCode, cancellationToken);

        var classified = records
            .Select(record => Classify(record, economicStatusRules, ageGroupRules, referencePeriod))
            .ToList();

        var aggregatedAt = DateTimeOffset.UtcNow;

        var economicStatusRows = BuildEconomicStatusSummary(
            classified, referencePeriod, analyticalDefinitionVersion, aggregatedAt);

        var identityRows = BuildIdentitySummary(
            classified, analyticalDefinitionVersion, aggregatedAt);

        var timeRows = BuildTimeSummary(
            classified, referencePeriod, analyticalDefinitionVersion, aggregatedAt);

        await _summaryRepository.ReplaceEconomicStatusSummaryAsync(
            referencePeriod, economicStatusRows, cancellationToken);

        await _summaryRepository.ReplaceIdentitySummaryAsync(
            referencePeriod, identityRows, cancellationToken);

        await _summaryRepository.UpsertTimeSummaryAsync(
            referencePeriod, timeRows, cancellationToken);

        var finishedAt = DateTimeOffset.UtcNow;

        // TODO: chua co bang/entity "ProcessingRun" (Summary.ProcessingRunId
        // hien luon duoc ghi null o day) du cot nay da ton tai san trong
        // schema Summary. Can thiet ke ProcessingRun (Id, StartedAt,
        // FinishedAt, Status, TriggerSource, ...) khi chot co che trigger
        // (xem IAggregationService), roi cap nhat ProcessingRunId that
        // trong 3 ham Build*Summary ben duoi thay vi null.

        return new AggregationRunResult(
            referencePeriod,
            records.Count,
            economicStatusRows.Count,
            identityRows.Count,
            timeRows.Count,
            startedAt,
            finishedAt);
    }

    // ============================================================
    // PHAN LOAI TUNG BAN GHI
    // ============================================================

    private sealed record ClassifiedRecord(
        LabourAggregationSourceRecord Source,
        string EconomicStatusCode,
        string? AgeGroupCode,
        string AdministrativeUnitCode,
        int AgeYears);

    private static ClassifiedRecord Classify(
        LabourAggregationSourceRecord record,
        IReadOnlyList<DimAnalyticalRule> economicStatusRules,
        IReadOnlyList<DimAnalyticalRule> ageGroupRules,
        DateOnly asOf)
    {
        var ageYears = CalculateAge(record.DateOfBirth, asOf);

        string? ResolveField(string fieldName) =>
            fieldName == "AgeYears"
                ? ageYears.ToString()
                : record.FieldValues.GetValueOrDefault(fieldName);

        var economicStatusItem = AnalyticalRuleEvaluator.Classify(
            economicStatusRules, ResolveField, asOf);

        var ageGroupItem = AnalyticalRuleEvaluator.Classify(
            ageGroupRules, ResolveField, asOf);

        return new ClassifiedRecord(
            record,
            economicStatusItem?.ItemCode ?? NotInLabourForceCode,
            ageGroupItem?.ItemCode,
            // TODO (blocker du an - "data field standards A02/A03/A06/
            // A19/B05/A08" dang cho VNPT): chua co bang mapping tu
            // PermanentAddressCode sang AdministrativeUnitCode chinh
            // thuc (vd qua DimAdministrativeUnit). Tam thoi dung truc
            // tiep PermanentAddressCode lam AdministrativeUnitCode -
            // PHAI xac nhan/thay the truoc khi dung so lieu that.
            record.PermanentAddressCode ?? "UNKNOWN",
            ageYears);
    }

    private static int CalculateAge(DateTime dateOfBirth, DateOnly asOf)
    {
        var asOfDate = asOf.ToDateTime(TimeOnly.MinValue);
        var age = asOfDate.Year - dateOfBirth.Year;

        if (dateOfBirth.Date > asOfDate.AddYears(-age))
        {
            age--;
        }

        return age;
    }

    // ============================================================
    // LABOUR ECONOMIC STATUS SUMMARY
    // ============================================================
    //
    // Grain: (ReferencePeriod, AdministrativeUnitCode, EducationLevelCode,
    // TechnicalLevelCode, EconomicStatusCode).

    private static List<LabourEconomicStatusSummary> BuildEconomicStatusSummary(
        IReadOnlyList<ClassifiedRecord> classified,
        DateOnly referencePeriod,
        int? analyticalDefinitionVersion,
        DateTimeOffset aggregatedAt)
    {
        return classified
            .GroupBy(c => new
            {
                c.AdministrativeUnitCode,
                EducationLevelCode = c.Source.EducationLevelCode ?? "UNKNOWN",
                TechnicalLevelCode = c.Source.TechnicalLevelCode ?? "UNKNOWN",
                c.EconomicStatusCode,
            })
            .Select(group =>
            {
                var items = group.ToList();

                return new LabourEconomicStatusSummary
                {
                    ReferencePeriod = referencePeriod,
                    AdministrativeUnitCode = group.Key.AdministrativeUnitCode,
                    EducationLevelCode = group.Key.EducationLevelCode,
                    TechnicalLevelCode = group.Key.TechnicalLevelCode,
                    EconomicStatusCode = group.Key.EconomicStatusCode,
                    LabourCount = items.Count,
                    ByGender = ToJsonCounts(items, x => x.Source.GenderCode),
                    ByAgeGroup = ToJsonCounts(
                        items.Where(x => x.AgeGroupCode is not null),
                        x => x.AgeGroupCode!),
                    ByIndustry = group.Key.EconomicStatusCode == EmployedCode
                        ? ToJsonMultiCounts(items, x => x.Source.IndustryCodes ?? [])
                        : "{}",
                    ByPriority = ToJsonMultiCounts(items, x => x.Source.PriorityCodes ?? []),
                    SourceIdentitySnapshotVersion = items.Max(x => x.Source.IdentitySnapshotVersion),
                    SourceEmploymentSnapshotVersion = items.Max(x => x.Source.EmploymentSnapshotVersion),
                    AnalyticalDefinitionVersion = analyticalDefinitionVersion,
                    ProcessingRunId = null,
                    AggregatedAt = aggregatedAt,
                };
            })
            .ToList();
    }

    // ============================================================
    // LABOUR IDENTITY SUMMARY
    // ============================================================
    //
    // Grain: (AdministrativeUnitCode, EducationLevelCode,
    // TechnicalLevelCode, TopLevelOccupationCode, AgeGroupCode,
    // EconomicStatusCode). Khong co ReferencePeriod trong entity nay
    // (xem ghi chu tren ISummaryRepository) nen 1 lan chay thay TOAN BO
    // bang - dung cho snapshot "hien trang" thay vi chuoi thoi gian.

    private static List<LabourIdentitySummary> BuildIdentitySummary(
        IReadOnlyList<ClassifiedRecord> classified,
        int? analyticalDefinitionVersion,
        DateTimeOffset aggregatedAt)
    {
        return classified
            .Where(c => c.AgeGroupCode is not null)
            .GroupBy(c => new
            {
                c.AdministrativeUnitCode,
                EducationLevelCode = c.Source.EducationLevelCode ?? "UNKNOWN",
                TechnicalLevelCode = c.Source.TechnicalLevelCode ?? "UNKNOWN",
                // TODO (can VNPT xac nhan): OccupationCode la ma nghe
                // nghiep chi tiet (leaf), chua duoc quy ve nhom nghe
                // nghiep cap cao nhat (top-level) qua phan cap
                // DimOccupation.ParentCode - IDimensionReader hien chua
                // co ham doc DimOccupation nen dung tam OccupationCode
                // truc tiep.
                TopLevelOccupationCode = c.Source.OccupationCode ?? "UNKNOWN",
                AgeGroupCode = c.AgeGroupCode!,
                c.EconomicStatusCode,
            })
            .Select(group =>
            {
                var items = group.ToList();

                var maleCount = items.Count(
                    x => string.Equals(x.Source.GenderCode, "MALE", StringComparison.OrdinalIgnoreCase));

                return new LabourIdentitySummary
                {
                    AdministrativeUnitCode = group.Key.AdministrativeUnitCode,
                    EducationLevelCode = group.Key.EducationLevelCode,
                    TechnicalLevelCode = group.Key.TechnicalLevelCode,
                    TopLevelOccupationCode = group.Key.TopLevelOccupationCode,
                    AgeGroupCode = group.Key.AgeGroupCode,
                    EconomicStatusCode = group.Key.EconomicStatusCode,
                    LabourCount = items.Count,
                    MaleCount = maleCount,
                    ByIndustry = group.Key.EconomicStatusCode == EmployedCode
                        ? ToJsonMultiCounts(items, x => x.Source.IndustryCodes ?? [])
                        : "{}",
                    ByPriority = ToJsonMultiCounts(items, x => x.Source.PriorityCodes ?? []),
                    SourceIdentitySnapshotVersion = items.Max(x => x.Source.IdentitySnapshotVersion),
                    SourceEmploymentSnapshotVersion = items.Max(x => x.Source.EmploymentSnapshotVersion),
                    AnalyticalDefinitionVersion = analyticalDefinitionVersion,
                    ProcessingRunId = null,
                    AggregatedAt = aggregatedAt,
                };
            })
            .ToList();
    }

    // ============================================================
    // LABOUR TIME SUMMARY
    // ============================================================
    //
    // Grain: (ReferencePeriod, AdministrativeUnitCode, EducationLevelCode,
    // TechnicalLevelCode). Revision/backfill (Previous*, IsRevised) do
    // ISummaryRepository.UpsertTimeSummaryAsync xu ly khi ghi, khong
    // phai o day.

    private static List<LabourTimeSummary> BuildTimeSummary(
        IReadOnlyList<ClassifiedRecord> classified,
        DateOnly referencePeriod,
        int? analyticalDefinitionVersion,
        DateTimeOffset aggregatedAt)
    {
        return classified
            .GroupBy(c => new
            {
                c.AdministrativeUnitCode,
                EducationLevelCode = c.Source.EducationLevelCode ?? "UNKNOWN",
                TechnicalLevelCode = c.Source.TechnicalLevelCode ?? "UNKNOWN",
            })
            .Select(group =>
            {
                var items = group.ToList();

                var workingAge = items.Count(
                    x => x.AgeYears >= DefaultWorkingAgeFrom && x.AgeYears <= DefaultWorkingAgeTo);

                var outWorkingAge = items.Count - workingAge;

                return new LabourTimeSummary
                {
                    ReferencePeriod = referencePeriod,
                    AdministrativeUnitCode = group.Key.AdministrativeUnitCode,
                    EducationLevelCode = group.Key.EducationLevelCode,
                    TechnicalLevelCode = group.Key.TechnicalLevelCode,
                    WorkingAgeLabourCount = workingAge,
                    OutWorkingAgeLabourCount = outWorkingAge,
                    TotalLabourCount = items.Count,
                    IsRevised = false,
                    AnalyticalDefinitionVersion = analyticalDefinitionVersion,
                    WorkingAgeFrom = DefaultWorkingAgeFrom,
                    WorkingAgeTo = DefaultWorkingAgeTo,
                    SourceIdentitySnapshotVersion = items.Max(x => x.Source.IdentitySnapshotVersion),
                    SourceEmploymentSnapshotVersion = items.Max(x => x.Source.EmploymentSnapshotVersion),
                    ProcessingRunId = null,
                    AggregatedAt = aggregatedAt,
                };
            })
            .ToList();
    }

    // ============================================================
    // HELPER - JSONB {code: count}
    // ============================================================

    private static string ToJsonCounts<T>(
        IEnumerable<T> items, Func<T, string> keySelector)
    {
        var counts = items
            .GroupBy(keySelector, StringComparer.Ordinal)
            .ToDictionary(g => g.Key, g => (long)g.Count());

        return JsonSerializer.Serialize(counts);
    }

    private static string ToJsonMultiCounts<T>(
        IEnumerable<T> items, Func<T, IEnumerable<string>> keysSelector)
    {
        var counts = new Dictionary<string, long>(StringComparer.Ordinal);

        foreach (var item in items)
        {
            foreach (var key in keysSelector(item))
            {
                counts[key] = counts.TryGetValue(key, out var existing) ? existing + 1 : 1;
            }
        }

        return JsonSerializer.Serialize(counts);
    }
}
