namespace Dashboard.Domain.Enums;

/// <summary>
/// Loại phép tổng hợp mà tầng Aggregate áp dụng khi tính 1 metric của
/// Summary từ dữ liệu Snapshot đã phân loại.
///
/// Hiện tại LabourAggregationService dùng trực tiếp Count/GroupedCount
/// khi build 3 Summary (xem Infrastructure.Aggregates); enum này là
/// taxonomy nhẹ, chuẩn bị cho lúc logic tổng hợp được mô tả bằng
/// data-driven metric definition thay vì hard-code trong C#.
/// </summary>
public enum AggregationType
{
    /// <summary>Đếm số bản ghi thỏa 1 grain (vd LabourCount).</summary>
    Count,

    /// <summary>Đếm số bản ghi, nhóm theo 1 field phụ trợ, không đổi
    /// grain - kết quả là map dạng JSONB (vd ByGender, ByIndustry).</summary>
    GroupedCount,

    /// <summary>Tổng giá trị số của 1 field.</summary>
    Sum,
}
