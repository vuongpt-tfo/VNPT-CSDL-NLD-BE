using Dashboard.Application.Common.Dtos;
using MediatR;

namespace Dashboard.Application.Features.LabourDashboard.Queries.GetLabourByEducationLevel;

// D01-W06 - provinceCode va year deu ap dung that (nguon
// labour_economic_status_summary, xem GetByEducationLevelAsync).
public sealed record GetLabourByEducationLevelQuery(
    string? ProvinceCode,
    int? Year)
    : IRequest<IReadOnlyList<LabourEducationLevelDto>>;
