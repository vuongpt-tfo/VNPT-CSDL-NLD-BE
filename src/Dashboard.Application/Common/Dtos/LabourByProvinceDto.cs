namespace Dashboard.Application.Common.Dtos;

public sealed class LabourByProvinceDto
{
    /// <summary>
    /// Mã hành chính tỉnh/thành (administrative_unit_code).
    /// </summary>
    public string ProvinceCode { get; set; } = null!;

    /// <summary>
    /// Tên tỉnh/thành hiển thị.
    /// </summary>
    public string ProvinceName { get; set; } = null!;

    /// <summary>
    /// Số lượng người lao động tại tỉnh này.
    /// </summary>
    public long LabourCount { get; set; }

    /// <summary>
    /// Tỷ lệ phần trăm trên tổng số người lao động toàn quốc.
    /// </summary>
    public decimal Percentage { get; set; }
}
