using Dashboard.Application.Common.Dtos;
using MediatR;

namespace Dashboard.Application.Features.LabourDashboard.Queries.GetLabourTrend;

// D01-W08 - Line xu huong lao dong theo thoi gian. Nguon
// labour_time_summary (grain THANG x administrative_unit x
// education_level x technical_level). Khac voi GetLabourByEconomicStatusQuery
// (W02), ca ProvinceCode lan Year o day deu duoc ap dung that trong
// WHERE vi labour_time_summary co reference_period (DATE) that, xem
// GetTrendAsync. Neu Year null, tra ve toan bo cac ky hien co.
public sealed record GetLabourTrendQuery(
    string? ProvinceCode,
    int? Year)
    : IRequest<IReadOnlyList<LabourTrendDto>>;
