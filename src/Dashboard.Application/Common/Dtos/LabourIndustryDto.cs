namespace Dashboard.Application.Common.Dtos;

public sealed class LabourIndustryDto
{
    /// <summary>
    /// Mã ngành kinh tế (dim_industry.code).
    /// </summary>
    public string IndustryCode { get; set; } = null!;

    /// <summary>
    /// Tên ngành kinh tế.
    /// </summary>
    public string IndustryName { get; set; } = null!;

    /// <summary>
    /// Số lượng người lao động thuộc ngành (đếm lặp nếu 1 lao động
    /// thuộc nhiều ngành — xem Design Decision D01-W07).
    /// </summary>
    public long LabourCount { get; set; }

    /// <summary>
    /// Tỷ lệ phần trăm trên tổng số lượt lao động theo ngành
    /// (mẫu số là tổng lượt, không phải tổng số lao động đang có
    /// việc làm, vì có đếm lặp).
    /// </summary>
    public decimal Percentage { get; set; }
}
