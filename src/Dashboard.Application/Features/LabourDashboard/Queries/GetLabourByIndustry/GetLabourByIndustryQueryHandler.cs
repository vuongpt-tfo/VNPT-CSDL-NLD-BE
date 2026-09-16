using Dashboard.Application.Abstractions.Persistence;
using Dashboard.Application.Common.Dtos;
using MediatR;

namespace Dashboard.Application.Features.LabourDashboard.Queries.GetLabourByIndustry;

public sealed class GetLabourByIndustryQueryHandler
    : IRequestHandler<
        GetLabourByIndustryQuery,
        IReadOnlyList<LabourIndustryDto>>
{
    private readonly ILabourDashboardReadRepository _repository;

    public GetLabourByIndustryQueryHandler(
        ILabourDashboardReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<LabourIndustryDto>> Handle(
        GetLabourByIndustryQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetByIndustryAsync(
            request.ProvinceCode,
            request.Year,
            cancellationToken);
    }
}
