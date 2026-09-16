namespace Dashboard.Application.Common.Dtos;

public sealed class LabourEconomicStatusDto
{
    /// <summary>
    /// Mã trạng thái kinh tế (EMP/UNE/INA, theo catalog ECONOMIC_ACTIVITY).
    /// </summary>
    public string EconomicStatusCode { get; set; } = null!;

    /// <summary>
    /// Tên hiển thị trạng thái kinh tế.
    /// </summary>
    public string EconomicStatusName { get; set; } = null!;

    /// <summary>
    /// Số lượng người lao động thuộc trạng thái này.
    /// </summary>
    public long LabourCount { get; set; }

    /// <summary>
    /// Tỷ lệ phần trăm trên tổng số người lao động thỏa điều kiện lọc.
    /// </summary>
    public decimal Percentage { get; set; }
}
