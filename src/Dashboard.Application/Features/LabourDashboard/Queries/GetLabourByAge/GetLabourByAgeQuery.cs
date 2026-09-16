using Dashboard.Application.Common.Dtos;
using MediatR;

namespace Dashboard.Application.Features.LabourDashboard.Queries.GetLabourByAge;

// D01-W05 - provinceCode va year deu ap dung that (nguon
// labour_economic_status_summary.by_age_group, xem GetByAgeAsync).
public sealed record GetLabourByAgeQuery(
    string? ProvinceCode,
    int? Year)
    : IRequest<IReadOnlyList<LabourAgeDto>>;
