using Dashboard.Domain.Entities.Snapshot;

namespace Dashboard.Domain.Repositories;

/// <summary>
/// Truy cập dữ liệu Snapshot thô (Employment/Identity) - chỉ đọc entity,
/// không join, không business logic. Dùng cho các nhu cầu đọc Snapshot
/// chung; tầng Aggregate dùng
/// Application.Abstractions.Snapshots.ISnapshotReader (bản ghi đã join,
/// business-facing) thay vì gọi interface này trực tiếp.
/// </summary>
public interface ISnapshotRepository
{
    Task<IReadOnlyList<LabourEmploymentSnapshot>> GetAllEmploymentSnapshotsAsync(
        CancellationToken cancellationToken);

    Task<IReadOnlyList<LabourIdentitySnapshot>> GetAllIdentitySnapshotsAsync(
        CancellationToken cancellationToken);
}
