using Dashboard.Application.Common.Dtos;
using MediatR;

namespace Dashboard.Application.Features.LabourDashboard.Queries.GetLabourByProvince;

// D01-W03 - ban do. provinceCode va year deu ap dung that (nguon
// labour_economic_status_summary, xem GetByProvinceAsync). Neu truyen
// provinceCode, ket qua CHI co 1 dong (dung tinh do) - FE dung de "chi
// hien ban do o tinh do".
public sealed record GetLabourByProvinceQuery(
    string? ProvinceCode,
    int? Year)
    : IRequest<IReadOnlyList<LabourByProvinceDto>>;
