using Dashboard.Application.Abstractions.Persistence;
using Dashboard.Application.Common.Dtos;
using MediatR;

namespace Dashboard.Application.Features.LabourDashboard.Queries.GetLabourByEconomicStatus;

public sealed class GetLabourByEconomicStatusQueryHandler
    : IRequestHandler<
        GetLabourByEconomicStatusQuery,
        IReadOnlyList<LabourEconomicStatusDto>>
{
    private readonly ILabourDashboardReadRepository _repository;

    public GetLabourByEconomicStatusQueryHandler(
        ILabourDashboardReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<LabourEconomicStatusDto>> Handle(
        GetLabourByEconomicStatusQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetByEconomicStatusAsync(
            request.ProvinceCode,
            request.Year,
            cancellationToken);
    }
}
