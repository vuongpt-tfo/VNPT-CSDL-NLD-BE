namespace Dashboard.Application.Abstractions.Aggregation;

/// <summary>
/// Orchestrator của tầng Aggregate: đọc Snapshot (qua ISnapshotReader),
/// phân loại bằng Domain.Rules.AnalyticalRuleEvaluator (dùng rule từ
/// IDimensionReader), tổng hợp thành 3 Summary rồi ghi qua
/// ISummaryRepository.
///
/// KHÔNG tự quyết định khi nào chạy - cơ chế trigger (scheduled
/// worker / Kafka consumer / endpoint nội bộ, chưa chốt - xem
/// docs/decisions/decision-log.md) sẽ gọi RunAsync khi cần.
/// </summary>
public interface IAggregationService
{
    /// <summary>
    /// Chạy 1 lượt Aggregate cho <paramref name="referencePeriod"/>,
    /// tính cả 3 Summary (EconomicStatus, Identity, Time). Idempotent:
    /// gọi lại cho cùng referencePeriod sẽ thay thế/ghi đè kết quả cũ
    /// (xem ISummaryRepository).
    /// </summary>
    Task<AggregationRunResult> RunAsync(
        DateOnly referencePeriod,
        CancellationToken cancellationToken);
}

/// <summary>
/// Kết quả 1 lượt Aggregate - dùng để log/audit. Chưa gắn với bảng
/// ProcessingRun cụ thể (bảng này chưa được thiết kế dù Summary đã có
/// sẵn cột ProcessingRunId - xem TODO trong LabourAggregationService).
/// </summary>
public sealed record AggregationRunResult(
    DateOnly ReferencePeriod,
    int SourceRecordCount,
    int EconomicStatusSummaryRowCount,
    int IdentitySummaryRowCount,
    int TimeSummaryRowCount,
    DateTimeOffset StartedAt,
    DateTimeOffset FinishedAt);
