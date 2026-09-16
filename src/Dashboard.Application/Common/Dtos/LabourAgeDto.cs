namespace Dashboard.Application.Common.Dtos;

public sealed class LabourAgeDto
{
    /// <summary>
    /// ID của nhóm tuổi trong dim_age_range.
    /// </summary>
    public int AgeRangeId { get; set; }

    /// <summary>
    /// Nhãn nhóm tuổi, ví dụ: 15-19, 20-24, 65+.
    /// </summary>
    public string AgeRange { get; set; } = null!;

    /// <summary>
    /// Màu hiển thị của nhóm tuổi.
    /// </summary>
    public string? AgeRangeColour { get; set; }

    /// <summary>
    /// Số lượng người lao động.
    /// </summary>
    public long LabourCount { get; set; }

    /// <summary>
    /// Tỷ lệ phần trăm trên tổng số người lao động.
    /// </summary>
    public decimal Percentage { get; set; }
}