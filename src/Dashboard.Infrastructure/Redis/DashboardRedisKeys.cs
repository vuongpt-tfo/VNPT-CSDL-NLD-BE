namespace Dashboard.Infrastructure.Redis;

/// <summary>
/// Chuan hoa cache key cho Dashboard - tranh moi noi tu ghep chuoi
/// key rieng, de doi prefix/quy uoc sau nay o mot cho duy nhat.
///
/// Cache key khong phai business identifier (xem
/// database/05_summary/README.md muc 33 - Cache Key).
/// </summary>
public static class DashboardRedisKeys
{
    private const string Prefix = "dashboard";

    public static string Build(params string[] segments)
    {
        var normalized = segments
            .Select(NormalizeSegment)
            .Where(s => !string.IsNullOrEmpty(s));

        return string.Join(':', new[] { Prefix }.Concat(normalized));
    }

    private static string NormalizeSegment(string segment) =>
        string.IsNullOrWhiteSpace(segment)
            ? string.Empty
            : segment.Trim().ToLowerInvariant();
}
