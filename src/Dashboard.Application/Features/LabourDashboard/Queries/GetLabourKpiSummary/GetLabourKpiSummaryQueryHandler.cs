using Dashboard.Application.Abstractions.Persistence;
using Dashboard.Application.Common.Dtos;
using MediatR;

namespace Dashboard.Application.Features.LabourDashboard.Queries.GetLabourKpiSummary;

public sealed class GetLabourKpiSummaryQueryHandler
    : IRequestHandler<
        GetLabourKpiSummaryQuery,
        LabourKpiSummaryDto>
{
    private readonly ILabourDashboardReadRepository _repository;

    public GetLabourKpiSummaryQueryHandler(
        ILabourDashboardReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<LabourKpiSummaryDto> Handle(
        GetLabourKpiSummaryQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetKpiSummaryAsync(
            request.ProvinceCode,
            request.Year,
            cancellationToken);
    }
}
