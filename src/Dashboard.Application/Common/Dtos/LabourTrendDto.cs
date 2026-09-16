namespace Dashboard.Application.Common.Dtos;

public sealed class LabourTrendDto
{
    /// <summary>
    /// Kỳ thống kê (ngày đầu tháng), theo labour_time_summary.reference_period.
    /// </summary>
    public DateOnly ReferencePeriod { get; set; }

    /// <summary>
    /// Số lao động trong độ tuổi lao động tại kỳ này.
    /// </summary>
    public long WorkingAgeLabourCount { get; set; }

    /// <summary>
    /// Số lao động ngoài độ tuổi lao động tại kỳ này.
    /// </summary>
    public long OutWorkingAgeLabourCount { get; set; }

    /// <summary>
    /// Tổng số lao động tại kỳ này.
    /// </summary>
    public long TotalLabourCount { get; set; }

    /// <summary>
    /// true nếu kỳ này đã bị ETL tính lại (revision/backfill) so với
    /// lần công bố trước — FE dùng để highlight. Xem Gap G-09,
    /// database/05_summary/labour_time_summary.sql.
    /// </summary>
    public bool IsRevised { get; set; }
}
