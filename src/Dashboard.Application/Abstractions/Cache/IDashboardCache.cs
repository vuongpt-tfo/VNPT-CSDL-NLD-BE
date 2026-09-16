namespace Dashboard.Application.Abstractions.Cache;

/// <summary>
/// Abstraction cho cache doc/ghi ket qua Dashboard da tinh san (Summary
/// query result, KPI, ...). Redis la performance cache, KHONG phai
/// source of truth (xem database/05_summary/README.md muc 26 - Summary
/// and Redis) - mat Redis khong duoc lam mat du lieu, chi lam cham lai
/// vi phai doc thang tu Postgres.
///
/// Chi la abstraction + implementation (DashboardRedisCache) da duoc
/// dang ky DI - CHUA co endpoint/handler nao trong LabourDashboard goi
/// interface nay (cache-per-endpoint/cache key convention la mot quyet
/// dinh rieng, ngoai pham vi D01-06).
/// </summary>
public interface IDashboardCache
{
    Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken);

    Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration,
        CancellationToken cancellationToken);

    Task RemoveAsync(
        string key,
        CancellationToken cancellationToken);
}
