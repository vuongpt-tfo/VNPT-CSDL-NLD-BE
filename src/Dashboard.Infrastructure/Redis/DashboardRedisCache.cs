using Dashboard.Application.Abstractions.Cache;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace Dashboard.Infrastructure.Redis;

/// <summary>
/// Implementation IDashboardCache tren StackExchange.Redis.
///
/// Redis la performance cache, KHONG phai source of truth (xem
/// database/05_summary/README.md muc 26). Vi vay moi loi ket noi/thao
/// tac Redis o day deu duoc nuot va log Warning thay vi nem exception
/// len tren - caller (khi co) coi nhu cache-miss va tu doc lai tu
/// Postgres, API khong duoc phep sap chi vi Redis khong san sang.
/// </summary>
public sealed class DashboardRedisCache : IDashboardCache
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly ILogger<DashboardRedisCache> _logger;

    public DashboardRedisCache(
        IConnectionMultiplexer connectionMultiplexer,
        ILogger<DashboardRedisCache> logger)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken)
    {
        try
        {
            var database = _connectionMultiplexer.GetDatabase();
            var value = await database.StringGetAsync(key);

            if (!value.HasValue)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(value!);
        }
        catch (RedisException ex)
        {
            _logger.LogWarning(
                ex,
                "Khong doc duoc Redis cache cho key {CacheKey} - coi nhu cache-miss",
                key);

            return default;
        }
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration,
        CancellationToken cancellationToken)
    {
        try
        {
            var database = _connectionMultiplexer.GetDatabase();
            var json = JsonSerializer.Serialize(value);

            await database.StringSetAsync(key, json, expiration);
        }
        catch (RedisException ex)
        {
            _logger.LogWarning(
                ex,
                "Khong ghi duoc Redis cache cho key {CacheKey} - bo qua",
                key);
        }
    }

    public async Task RemoveAsync(
        string key,
        CancellationToken cancellationToken)
    {
        try
        {
            var database = _connectionMultiplexer.GetDatabase();
            await database.KeyDeleteAsync(key);
        }
        catch (RedisException ex)
        {
            _logger.LogWarning(
                ex,
                "Khong xoa duoc Redis cache cho key {CacheKey} - bo qua",
                key);
        }
    }
}
