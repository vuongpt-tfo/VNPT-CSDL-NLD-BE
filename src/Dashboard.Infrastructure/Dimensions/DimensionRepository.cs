using Dashboard.Application.Abstractions.Dimension;
using Dashboard.Domain.Entities.Dimension;
using Dashboard.Domain.Entities.Dimension.Analytical;
using Dashboard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Dashboard.Infrastructure.Dimensions;

public sealed class DimensionRepository : IDimensionReader
{
    private readonly DashboardDbContext _context;

    public DimensionRepository(DashboardDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<DimAnalyticalRule>> GetActiveRulesAsync(
        string analyticalCode,
        CancellationToken cancellationToken)
    {
        return await _context.DimAnalyticalRules
            .AsNoTracking()
            .Include(r => r.Conditions)
                .ThenInclude(c => c.AnalyticalItem)
            .Where(r => r.IsActive)
            .Where(r => r.Analytical.AnalyticalCode == analyticalCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<int?> GetActiveVersionAsync(
        string analyticalCode,
        CancellationToken cancellationToken)
    {
        return await _context.DimAnalyticals
            .AsNoTracking()
            .Where(a => a.AnalyticalCode == analyticalCode && a.IsActive)
            .OrderByDescending(a => a.Version)
            .Select(a => (int?)a.Version)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DimAdministrativeUnit>> GetActiveAdministrativeUnitsAsync(
        CancellationToken cancellationToken)
    {
        return await _context.DimAdministrativeUnits
            .AsNoTracking()
            .Where(u => u.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DimAgeRange>> GetActiveAgeRangesAsync(
        CancellationToken cancellationToken)
    {
        return await _context.DimAgeRanges
            .AsNoTracking()
            .Where(r => r.IsActive)
            .ToListAsync(cancellationToken);
    }
}
