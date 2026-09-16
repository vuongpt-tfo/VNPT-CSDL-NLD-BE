using Dashboard.Application.Abstractions.Snapshots;
using Dashboard.Domain.Entities.Snapshot;
using Dashboard.Domain.Repositories;
using Dashboard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Dashboard.Infrastructure.Snapshots;

public sealed class SnapshotRepository
    : ISnapshotRepository, ISnapshotReader
{
    private readonly DashboardDbContext _context;

    public SnapshotRepository(DashboardDbContext context)
    {
        _context = context;
    }

    // ============================================================
    // ISnapshotRepository - doc entity tho, khong join
    // ============================================================

    public async Task<IReadOnlyList<LabourEmploymentSnapshot>> GetAllEmploymentSnapshotsAsync(
        CancellationToken cancellationToken)
    {
        return await _context.LabourEmploymentSnapshots
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LabourIdentitySnapshot>> GetAllIdentitySnapshotsAsync(
        CancellationToken cancellationToken)
    {
        return await _context.LabourIdentitySnapshots
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    // ============================================================
    // ISnapshotReader - ban ghi nghiep vu da join, phuc vu Aggregate
    // ============================================================
    //
    // INNER JOIN Identity x Employment theo LabourId: ca
    // LabourEconomicStatusSummary lan LabourIdentitySummary deu ghi
    // lineage tu CA HAI SourceIdentitySnapshotVersion va
    // SourceEmploymentSnapshotVersion (xem Domain.Entities.Summary.*),
    // nghia la 1 lao dong chi duoc tinh khi co du ca Identity lan
    // Employment snapshot. Lao dong chi co 1 trong 2 se bi loai o buoc
    // nay - CAN xac nhan lai voi nghiep vu neu day khong phai hanh vi
    // mong muon (vd nguoi moi dang ky, chua co Employment snapshot).

    public async Task<IReadOnlyList<LabourAggregationSourceRecord>> GetLabourRecordsAsync(
        CancellationToken cancellationToken)
    {
        var query =
            from identity in _context.LabourIdentitySnapshots.AsNoTracking()
            join employment in _context.LabourEmploymentSnapshots.AsNoTracking()
                on identity.LabourId equals employment.LabourId
            select new { identity, employment };

        var rows = await query.ToListAsync(cancellationToken);

        return rows
            .Select(row => Map(row.identity, row.employment))
            .ToList();
    }

    private static LabourAggregationSourceRecord Map(
        LabourIdentitySnapshot identity,
        LabourEmploymentSnapshot employment)
    {
        var fieldValues = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["GenderCode"] = identity.GenderCode,
            ["EthnicCode"] = identity.EthnicCode,
            ["MaritalStatus"] = identity.MaritalStatus,
            ["EducationLevelCode"] = employment.EducationLevelCode,
            ["TechnicalLevelCode"] = employment.TechnicalLevelCode,
            ["EconomicActivitiesCode"] = employment.EconomicActivitiesCode.ToString(),
            ["ContractCode"] = employment.ContractCode,
            ["JobTypesCode"] = employment.JobTypesCode,
            ["EmployerSectorsCode"] = employment.EmployerSectorsCode,
            ["EverWorked"] = employment.EverWorked?.ToString(),
            ["UnemploymentBenefitStatus"] = employment.UnemploymentBenefitStatus?.ToString(),
            ["JobSearchWanted"] = employment.JobSearchWanted?.ToString(),
            ["UneconomicReasonCode"] = employment.UneconomicReasonCode,
            ["ParticipationFormCode"] = employment.ParticipationFormCode,
        };

        return new LabourAggregationSourceRecord
        {
            LabourId = identity.LabourId,
            IdentitySnapshotVersion = identity.SnapshotVersion,
            EmploymentSnapshotVersion = employment.SnapshotVersion,
            FieldValues = fieldValues,
            GenderCode = identity.GenderCode,
            DateOfBirth = identity.DateOfBirth,
            PermanentAddressCode = identity.PermanentAddressCode,
            EducationLevelCode = employment.EducationLevelCode,
            TechnicalLevelCode = employment.TechnicalLevelCode,
            IndustryCodes = employment.IndustryCodes,
            PriorityCodes = employment.PriorityCodes,
            OccupationCode = employment.OccupationCode,
        };
    }
}
