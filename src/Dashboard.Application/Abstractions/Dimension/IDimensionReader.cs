using Dashboard.Domain.Entities.Dimension;
using Dashboard.Domain.Entities.Dimension.Analytical;

namespace Dashboard.Application.Abstractions.Dimension;

/// <summary>
/// Đọc dữ liệu Dimension phục vụ tầng Aggregate: rule phân loại
/// (DimAnalytical*) và các bảng tra cứu. Application chỉ biết
/// interface này, không biết EF Core.
/// </summary>
public interface IDimensionReader
{
    /// <summary>
    /// Toàn bộ Rule (kèm Conditions) đang Active của 1 DimAnalytical,
    /// theo AnalyticalCode (vd "ECONOMIC_STATUS", "AGE_GROUP") - dùng
    /// cho Domain.Rules.AnalyticalRuleEvaluator.
    /// </summary>
    Task<IReadOnlyList<DimAnalyticalRule>> GetActiveRulesAsync(
        string analyticalCode,
        CancellationToken cancellationToken);

    /// <summary>
    /// Version hiện hành (cao nhất, active) của 1 DimAnalytical - ghi
    /// vào Summary.AnalyticalDefinitionVersion để lineage biết kết quả
    /// được tính theo bộ rule nào.
    /// </summary>
    Task<int?> GetActiveVersionAsync(
        string analyticalCode,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<DimAdministrativeUnit>> GetActiveAdministrativeUnitsAsync(
        CancellationToken cancellationToken);

    Task<IReadOnlyList<DimAgeRange>> GetActiveAgeRangesAsync(
        CancellationToken cancellationToken);
}
