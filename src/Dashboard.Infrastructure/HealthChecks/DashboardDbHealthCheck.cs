using Dashboard.Infrastructure.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Dashboard.Infrastructure.HealthChecks;

/// <summary>
/// Health check toi thieu cho Postgres (dashboard_sch) qua
/// DashboardDbContext - dung Database.CanConnectAsync() thay vi them
/// package AspNetCore.HealthChecks.NpgSql, giu dependency toi thieu.
/// </summary>
public sealed class DashboardDbHealthCheck : IHealthCheck
{
    private readonly DashboardDbContext _context;

    public DashboardDbHealthCheck(DashboardDbContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await _context.Database
                .CanConnectAsync(cancellationToken);

            return canConnect
                ? HealthCheckResult.Healthy("Ket noi Postgres binh thuong.")
                : HealthCheckResult.Unhealthy("Khong ket noi duoc Postgres.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "Loi khi kiem tra ket noi Postgres.", ex);
        }
    }
}
