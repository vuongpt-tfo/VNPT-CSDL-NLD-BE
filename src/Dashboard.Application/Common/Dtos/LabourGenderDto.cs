namespace Dashboard.Application.Common.Dtos;

public sealed class LabourGenderDto
{
    /// <summary>
    /// Mã giới tính nội bộ: MALE hoặc FEMALE.
    /// </summary>
    public string GenderCode { get; set; } = null!;

    /// <summary>
    /// Nhãn hiển thị, ví dụ: Nam, Nữ.
    /// </summary>
    public string GenderName { get; set; } = null!;

    /// <summary>
    /// Số lượng người lao động.
    /// </summary>
    public long LabourCount { get; set; }

    /// <summary>
    /// Tỷ lệ phần trăm trên tổng số người lao động.
    /// </summary>
    public decimal Percentage { get; set; }
}
