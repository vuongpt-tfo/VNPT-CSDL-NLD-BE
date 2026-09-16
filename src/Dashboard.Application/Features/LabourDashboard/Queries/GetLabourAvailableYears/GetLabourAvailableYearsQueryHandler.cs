using Dashboard.Application.Abstractions.Persistence;
using MediatR;

namespace Dashboard.Application.Features.LabourDashboard.Queries.GetLabourAvailableYears;

public sealed class GetLabourAvailableYearsQueryHandler
    : IRequestHandler<GetLabourAvailableYearsQuery, IReadOnlyList<int>>
{
    private readonly ILabourDashboardReadRepository _repository;

    public GetLabourAvailableYearsQueryHandler(ILabourDashboardReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<int>> Handle(
        GetLabourAvailableYearsQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetAvailableYearsAsync(cancellationToken);
    }
}
