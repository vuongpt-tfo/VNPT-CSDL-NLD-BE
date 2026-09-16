using MediatR;

namespace Dashboard.Application.Features.LabourDashboard.Queries.GetLabourAvailableYears;

// Phuc vu dropdown "Nam du lieu" (bo loc dung chung) tren dashboard.component.ts
// - load dong danh sach Nam co that trong labour_economic_status_summary,
// thay vi hardcode.
public sealed record GetLabourAvailableYearsQuery
    : IRequest<IReadOnlyList<int>>;
