using Dashboard.Domain.Entities.Summary;

namespace Dashboard.Domain.Repositories;

/// <summary>
/// Ghi kết quả Aggregate vào 3 bảng Summary. "1 lần ghi" tương ứng 1
/// lượt chạy IAggregationService.RunAsync cho 1 ReferencePeriod -
/// repository không tự quản lý ProcessingRun/transaction cấp cao hơn,
/// chỉ thực hiện thao tác dữ liệu.
/// </summary>
public interface ISummaryRepository
{
    /// <summary>
    /// Thay toàn bộ Summary của 1 ReferencePeriod bằng tập kết quả mới
    /// (xóa row cũ cùng ReferencePeriod rồi insert lại) - đơn giản và
    /// đúng cho batch T+1 hằng ngày.
    /// </summary>
    Task ReplaceEconomicStatusSummaryAsync(
        DateOnly referencePeriod,
        IReadOnlyList<LabourEconomicStatusSummary> rows,
        CancellationToken cancellationToken);

    /// <summary>
    /// LabourIdentitySummary hiện KHÔNG có cột ReferencePeriod (xem
    /// Domain.Entities.Summary.LabourIdentitySummary) nên implementation
    /// hiện thay toàn bộ bảng mỗi lần chạy - không lọc theo period như
    /// 2 hàm còn lại. Cần rà soát lại nếu Identity Summary sau này thêm
    /// ReferencePeriod.
    /// </summary>
    Task ReplaceIdentitySummaryAsync(
        DateOnly referencePeriod,
        IReadOnlyList<LabourIdentitySummary> rows,
        CancellationToken cancellationToken);

    /// <summary>
    /// LabourTimeSummary có cơ chế revision/backfill riêng (IsRevised,
    /// Previous*, xem D01-W08): nếu grain đã tồn tại cho
    /// ReferencePeriod, dịch giá trị cũ sang cột Previous* và đánh dấu
    /// IsRevised thay vì xóa - khác 2 hàm Replace* ở trên.
    /// </summary>
    Task UpsertTimeSummaryAsync(
        DateOnly referencePeriod,
        IReadOnlyList<LabourTimeSummary> rows,
        CancellationToken cancellationToken);
}
