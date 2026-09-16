using Dashboard.Domain.Entities.Dimension.Analytical;

namespace Dashboard.Domain.Rules;

/// <summary>
/// Đánh giá DimAnalyticalRule/DimAnalyticalRuleCondition (đã nạp sẵn
/// navigation Conditions) trên 1 bản ghi nguồn để phân loại bản ghi đó
/// vào 1 DimAnalyticalItem.
///
/// Thuần Domain - không phụ thuộc EF Core/DB. Việc nạp Rule từ DB do
/// Application.Abstractions.Dimension.IDimensionReader +
/// Infrastructure.Dimensions.DimensionRepository đảm nhiệm; evaluator
/// chỉ nhận dữ liệu đã nạp sẵn và 1 hàm đọc giá trị field.
///
/// Lý do tồn tại: tách "công thức phân loại" (EconomicStatusCode,
/// AgeGroupCode, ...) ra khỏi code C#, để các quy tắc/công thức chính
/// thức (đang chờ VNPT xác nhận - xem docs/decisions/decision-log.md)
/// có thể cấu hình bằng DATA trong DimAnalyticalRule/
/// DimAnalyticalRuleCondition mà không cần build lại service.
/// </summary>
public static class AnalyticalRuleEvaluator
{
    /// <summary>
    /// Phân loại 1 bản ghi nguồn theo tập rule Active, effective tại
    /// <paramref name="asOf"/>, thuộc 1 DimAnalytical (vd
    /// "ECONOMIC_STATUS", "AGE_GROUP"). Rule có Version cao nhất trong
    /// số rule effective được ưu tiên. Trả về null nếu không rule nào
    /// khớp - caller tự quyết định giá trị fallback.
    /// </summary>
    public static DimAnalyticalItem? Classify(
        IEnumerable<DimAnalyticalRule> rules,
        Func<string, string?> fieldValueResolver,
        DateOnly asOf)
    {
        var candidateRules = rules
            .Where(r => r.IsActive)
            .Where(r => r.EffectiveFrom <= asOf)
            .Where(r => r.EffectiveTo is null || r.EffectiveTo >= asOf)
            .OrderByDescending(r => r.Version)
            .ThenBy(r => r.RuleCode, StringComparer.Ordinal);

        foreach (var rule in candidateRules)
        {
            var matched = MatchFirstItem(rule, fieldValueResolver);

            if (matched is not null)
            {
                return matched;
            }
        }

        return null;
    }

    // Trong 1 rule, các condition thuộc các AnalyticalItem khác nhau.
    // Condition cùng 1 AnalyticalItemId được coi là AND với nhau
    // (ConditionOrder chỉ định thứ tự đánh giá, không đổi ngữ nghĩa).
    // Item đầu tiên (theo ConditionOrder nhỏ nhất) có toàn bộ condition
    // khớp sẽ được chọn.
    private static DimAnalyticalItem? MatchFirstItem(
        DimAnalyticalRule rule,
        Func<string, string?> fieldValueResolver)
    {
        var byItem = rule.Conditions
            .GroupBy(c => c.AnalyticalItemId)
            .OrderBy(g => g.Min(c => c.ConditionOrder));

        foreach (var group in byItem)
        {
            var allMatch = group
                .OrderBy(c => c.ConditionOrder)
                .All(condition => Evaluate(condition, fieldValueResolver));

            if (allMatch)
            {
                return group.First().AnalyticalItem;
            }
        }

        return null;
    }

    private static bool Evaluate(
        DimAnalyticalRuleCondition condition,
        Func<string, string?> fieldValueResolver)
    {
        var actual = fieldValueResolver(condition.SourceField);

        return condition.Operator.ToUpperInvariant() switch
        {
            "EQ" => string.Equals(
                actual, condition.ValueFrom, StringComparison.OrdinalIgnoreCase),

            "NEQ" => !string.Equals(
                actual, condition.ValueFrom, StringComparison.OrdinalIgnoreCase),

            "IN" => SplitValues(condition.Values)
                .Any(v => string.Equals(v, actual, StringComparison.OrdinalIgnoreCase)),

            "NOT_IN" => !SplitValues(condition.Values)
                .Any(v => string.Equals(v, actual, StringComparison.OrdinalIgnoreCase)),

            "BETWEEN" => TryCompareRange(actual, condition.ValueFrom, condition.ValueTo),

            "IS_NULL" => string.IsNullOrEmpty(actual),

            "IS_NOT_NULL" => !string.IsNullOrEmpty(actual),

            _ => throw new NotSupportedException(
                $"Toán tử '{condition.Operator}' của DimAnalyticalRuleCondition " +
                "chưa được AnalyticalRuleEvaluator hỗ trợ."),
        };
    }

    private static IEnumerable<string> SplitValues(string? values) =>
        string.IsNullOrWhiteSpace(values)
            ? Enumerable.Empty<string>()
            : values.Split(
                ',',
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

    private static bool TryCompareRange(string? actual, string? from, string? to)
    {
        if (actual is null || !decimal.TryParse(actual, out var actualValue))
        {
            return false;
        }

        if (from is not null
            && decimal.TryParse(from, out var fromValue)
            && actualValue < fromValue)
        {
            return false;
        }

        if (to is not null
            && decimal.TryParse(to, out var toValue)
            && actualValue > toValue)
        {
            return false;
        }

        return true;
    }
}
