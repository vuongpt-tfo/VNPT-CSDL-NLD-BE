namespace Dashboard.Application.Common.Dtos;

public sealed class LabourKpiSummaryDto
{
    /// <summary>
    /// Tổng số lao động (Đang làm việc + Thất nghiệp + Không tham gia
    /// HĐKT) tại kỳ/tháng gần nhất trong Năm đã chọn (bộ lọc dùng
    /// chung) — hoặc Năm hiện tại nếu không truyền Năm. Áp dụng đúng
    /// nguyên tắc "quyền của bộ lọc dùng chung áp dụng lên toàn bộ số
    /// liệu của widget có dim liên quan" — không còn là số liệu
    /// "hiện tại" cố định như trước. Xem GetKpiSummaryAsync.
    /// </summary>
    public long TotalLabourCount { get; set; }

    /// <summary>
    /// Số lao động đang làm việc (economic_status_code = EMP) tại kỳ
    /// theo Năm đã chọn — xem ghi chú TotalLabourCount.
    /// </summary>
    public long EmployedCount { get; set; }

    /// <summary>
    /// Số lao động thất nghiệp (economic_status_code = UNE) tại kỳ
    /// theo Năm đã chọn — xem ghi chú TotalLabourCount.
    /// </summary>
    public long UnemployedCount { get; set; }

    /// <summary>
    /// Số lao động không tham gia hoạt động kinh tế
    /// (economic_status_code = INA) tại kỳ theo Năm đã chọn — xem ghi
    /// chú TotalLabourCount.
    /// </summary>
    public long NotInLabourForceCount { get; set; }

    /// <summary>
    /// Tỷ lệ % tăng trưởng của Tổng số so với cùng kỳ: "cùng kỳ" = Năm
    /// đã chọn (hoặc năm hiện tại nếu không truyền) trừ 1, lấy tổng
    /// labour_economic_status_summary.labour_count (cộng cả 3 nhóm
    /// EMP/UNE/INA) tại kỳ/tháng gần nhất đã có dữ liệu trong năm đó —
    /// cùng nguồn/cùng kỳ đại diện với 4 số ở trên để nhất quán. Trả
    /// về null khi năm đó chưa có dữ liệu (ví dụ mới triển khai chưa
    /// đủ 12 tháng). Quy tắc "cùng kỳ" là placeholder (Gap G-09), CHƯA
    /// chốt với VNPT trước go-live — xem
    /// database/05_summary/labour_economic_status_summary.sql. FE cần
    /// tự ẩn phần %tăng trưởng khi giá trị này là null, KHÔNG hiển thị
    /// số giả.
    /// </summary>
    public decimal? GrowthPercentage { get; set; }

    /// <summary>
    /// Tỷ lệ % tăng trưởng riêng của EmployedCount so với cùng kỳ
    /// (cùng quy tắc/nguồn với GrowthPercentage, lọc thêm
    /// economic_status_code = EMP). Null nếu chưa có dữ liệu cùng kỳ.
    /// </summary>
    public decimal? EmployedGrowthPercentage { get; set; }

    /// <summary>
    /// Tỷ lệ % tăng trưởng riêng của UnemployedCount so với cùng kỳ
    /// (economic_status_code = UNE). Null nếu chưa có dữ liệu cùng kỳ.
    /// </summary>
    public decimal? UnemployedGrowthPercentage { get; set; }

    /// <summary>
    /// Tỷ lệ % tăng trưởng riêng của NotInLabourForceCount so với cùng
    /// kỳ (economic_status_code = INA). Null nếu chưa có dữ liệu cùng
    /// kỳ.
    /// </summary>
    public decimal? NotInLabourForceGrowthPercentage { get; set; }
}
