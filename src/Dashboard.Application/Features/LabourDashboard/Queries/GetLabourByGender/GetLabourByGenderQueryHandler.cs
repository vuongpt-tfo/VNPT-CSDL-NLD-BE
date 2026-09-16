using Dashboard.Application.Abstractions.Persistence;
using Dashboard.Application.Common.Dtos;
using MediatR;

namespace Dashboard.Application.Features.LabourDashboard.Queries.GetLabourByGender;

public sealed class GetLabourByGenderQueryHandler
    : IRequestHandler<
        GetLabourByGenderQuery,
        IReadOnlyList<LabourGenderDto>>
{
    private readonly ILabourDashboardReadRepository _repository;

    public GetLabourByGenderQueryHandler(
        ILabourDashboardReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<LabourGenderDto>> Handle(
        GetLabourByGenderQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetByGenderAsync(
            request.ProvinceCode,
            request.Year,
            cancellationToken);
    }
}
