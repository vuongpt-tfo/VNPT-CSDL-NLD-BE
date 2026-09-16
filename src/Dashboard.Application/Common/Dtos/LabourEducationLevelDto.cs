namespace Dashboard.Application.Common.Dtos;

public sealed class LabourEducationLevelDto
{
    /// <summary>
    /// Mã cấp trình độ (dim_catalog_items.code, catalog_name =
    /// EDUCATION_LEVEL). Xem Open Issue W06 về khả năng đây chưa phải
    /// đúng cột A06 (còn TECHNICAL_EDUCATION_LEVEL song song).
    /// </summary>
    public string EducationLevelCode { get; set; } = null!;

    /// <summary>
    /// Tên hiển thị cấp trình độ. "Không xác định" khi không khớp
    /// được dim_catalog_items (thay vì bị loại âm thầm).
    /// </summary>
    public string EducationLevelName { get; set; } = null!;

    /// <summary>
    /// Số lượng người lao động thuộc cấp trình độ này.
    /// </summary>
    public long LabourCount { get; set; }

    /// <summary>
    /// Tỷ lệ phần trăm trên tổng số người lao động.
    /// </summary>
    public decimal Percentage { get; set; }
}
