using Dashboard.Application.Abstractions.Persistence;
using Dashboard.Application.Common.Dtos;
using MediatR;

namespace Dashboard.Application.Features.LabourDashboard.Queries.GetLabourByAge;

public sealed class GetLabourByAgeQueryHandler
    : IRequestHandler<
        GetLabourByAgeQuery,
        IReadOnlyList<LabourAgeDto>>
{
    private readonly ILabourDashboardReadRepository _repository;

    public GetLabourByAgeQueryHandler(
        ILabourDashboardReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<LabourAgeDto>> Handle(
        GetLabourByAgeQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetByAgeAsync(
            request.ProvinceCode,
            request.Year,
            cancellationToken);
    }
}
