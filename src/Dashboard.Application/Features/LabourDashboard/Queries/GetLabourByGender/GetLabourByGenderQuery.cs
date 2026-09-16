using Dashboard.Application.Common.Dtos;
using MediatR;

namespace Dashboard.Application.Features.LabourDashboard.Queries.GetLabourByGender;

// D01-W04 - provinceCode va year deu ap dung that (nguon
// labour_economic_status_summary, xem GetByGenderAsync).
public sealed record GetLabourByGenderQuery(
    string? ProvinceCode,
    int? Year)
    : IRequest<IReadOnlyList<LabourGenderDto>>;
