using Dashboard.Application.Common.Dtos;
using MediatR;

namespace Dashboard.Application.Features.LabourDashboard.Queries.GetLabourByEconomicStatus;

// year: theo LLD D01-W02 (claude/d01-w02-lld-content-08-09.md).
// labour_economic_status_summary gio da co reference_period (bo sung
// cho D01-W01/W02 - xem database/05_summary/labour_economic_status_summary.sql),
// nen year duoc ap dung that: chon ky/thang GAN NHAT trong Nam do lam
// dai dien (semi-additive - khong SUM nhieu ky). Neu Year null, lay
// ky gan nhat hien co (tuong duong "hien tai").
public sealed record GetLabourByEconomicStatusQuery(
    int? Year,
    string? ProvinceCode)
    : IRequest<IReadOnlyList<LabourEconomicStatusDto>>;
