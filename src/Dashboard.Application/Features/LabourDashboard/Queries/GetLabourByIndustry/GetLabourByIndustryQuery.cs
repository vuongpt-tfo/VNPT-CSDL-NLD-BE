using Dashboard.Application.Common.Dtos;
using MediatR;

namespace Dashboard.Application.Features.LabourDashboard.Queries.GetLabourByIndustry;

// D01-W07 - provinceCode va year deu ap dung that (nguon
// labour_economic_status_summary.by_industry, xem GetByIndustryAsync).
public sealed record GetLabourByIndustryQuery(
    string? ProvinceCode,
    int? Year)
    : IRequest<IReadOnlyList<LabourIndustryDto>>;
