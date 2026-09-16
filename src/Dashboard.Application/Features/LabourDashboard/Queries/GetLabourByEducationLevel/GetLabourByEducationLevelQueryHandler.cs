using Dashboard.Application.Abstractions.Persistence;
using Dashboard.Application.Common.Dtos;
using MediatR;

namespace Dashboard.Application.Features.LabourDashboard.Queries.GetLabourByEducationLevel;

public sealed class GetLabourByEducationLevelQueryHandler
    : IRequestHandler<
        GetLabourByEducationLevelQuery,
        IReadOnlyList<LabourEducationLevelDto>>
{
    private readonly ILabourDashboardReadRepository _repository;

    public GetLabourByEducationLevelQueryHandler(
        ILabourDashboardReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<LabourEducationLevelDto>> Handle(
        GetLabourByEducationLevelQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetByEducationLevelAsync(
            request.ProvinceCode,
            request.Year,
            cancellationToken);
    }
}
