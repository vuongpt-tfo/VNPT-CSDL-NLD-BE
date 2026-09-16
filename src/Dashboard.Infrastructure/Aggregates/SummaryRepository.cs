using Dashboard.Domain.Entities.Summary;
using Dashboard.Domain.Repositories;
using Dashboard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Dashboard.Infrastructure.Aggregates;

public sealed class SummaryRepository : ISummaryRepository
{
    private readonly DashboardDbContext _context;

    public SummaryRepository(DashboardDbContext context)
    {
        _context = context;
    }

    public async Task ReplaceEconomicStatusSummaryAsync(
        DateOnly referencePeriod,
        IReadOnlyList<LabourEconomicStatusSummary> rows,
        CancellationToken cancellationToken)
    {
        var existing = await _context.LabourEconomicStatusSummaries
            .Where(x => x.ReferencePeriod == referencePeriod)
            .ToListAsync(cancellationToken);

        _context.LabourEconomicStatusSummaries.RemoveRange(existing);
        await _context.LabourEconomicStatusSummaries.AddRangeAsync(rows, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ReplaceIdentitySummaryAsync(
        DateOnly referencePeriod,
        IReadOnlyList<LabourIdentitySummary> rows,
        CancellationToken cancellationToken)
    {
        // Xem ghi chu tren ISummaryRepository.ReplaceIdentitySummaryAsync:
        // LabourIdentitySummary khong co ReferencePeriod nen o day thay
        // TOAN BO bang, tham so referencePeriod hien khong dung toi.
        _ = referencePeriod;

        var existing = await _context.LabourIdentitySummaries
            .ToListAsync(cancellationToken);

        _context.LabourIdentitySummaries.RemoveRange(existing);
        await _context.LabourIdentitySummaries.AddRangeAsync(rows, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpsertTimeSummaryAsync(
        DateOnly referencePeriod,
        IReadOnlyList<LabourTimeSummary> rows,
        CancellationToken cancellationToken)
    {
        var existingRows = await _context.LabourTimeSummaries
            .Where(x => x.ReferencePeriod == referencePeriod)
            .ToListAsync(cancellationToken);

        var existingByGrain = existingRows.ToDictionary(GrainKey);

        var now = DateTimeOffset.UtcNow;

        foreach (var row in rows)
        {
            if (existingByGrain.TryGetValue(GrainKey(row), out var previous))
            {
                previous.PreviousWorkingAgeLabourCount = previous.WorkingAgeLabourCount;
                previous.PreviousOutWorkingAgeLabourCount = previous.OutWorkingAgeLabourCount;
                previous.PreviousTotalLabourCount = previous.TotalLabourCount;

                previous.WorkingAgeLabourCount = row.WorkingAgeLabourCount;
                previous.OutWorkingAgeLabourCount = row.OutWorkingAgeLabourCount;
                previous.TotalLabourCount = row.TotalLabourCount;

                previous.IsRevised = true;
                previous.RevisedAt = now;

                previous.AnalyticalDefinitionVersion = row.AnalyticalDefinitionVersion;
                previous.WorkingAgeFrom = row.WorkingAgeFrom;
                previous.WorkingAgeTo = row.WorkingAgeTo;
                previous.SourceIdentitySnapshotVersion = row.SourceIdentitySnapshotVersion;
                previous.SourceEmploymentSnapshotVersion = row.SourceEmploymentSnapshotVersion;
                previous.ProcessingRunId = row.ProcessingRunId;
                previous.AggregatedAt = row.AggregatedAt;
            }
            else
            {
                await _context.LabourTimeSummaries.AddAsync(row, cancellationToken);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    private static (string AdministrativeUnitCode, string EducationLevelCode, string TechnicalLevelCode)
        GrainKey(LabourTimeSummary row) =>
        (row.AdministrativeUnitCode, row.EducationLevelCode, row.TechnicalLevelCode);
}
