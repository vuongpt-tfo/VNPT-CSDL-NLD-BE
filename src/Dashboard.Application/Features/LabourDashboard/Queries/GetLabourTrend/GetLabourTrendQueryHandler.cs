using Dashboard.Application.Abstractions.Persistence;
using Dashboard.Application.Common.Dtos;
using MediatR;

namespace Dashboard.Application.Features.LabourDashboard.Queries.GetLabourTrend;

public sealed class GetLabourTrendQueryHandler
    : IRequestHandler<GetLabourTrendQuery, IReadOnlyList<LabourTrendDto>>
{
    private readonly ILabourDashboardReadRepository _repository;

    public GetLabourTrendQueryHandler(ILabourDashboardReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<LabourTrendDto>> Handle(
        GetLabourTrendQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetTrendAsync(
            request.ProvinceCode,
            request.Year,
            cancellationToken);
    }
}
