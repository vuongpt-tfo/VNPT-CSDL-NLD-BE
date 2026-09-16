using Dashboard.Application.Abstractions.Persistence;
using Dashboard.Application.Common.Dtos;
using MediatR;

namespace Dashboard.Application.Features.LabourDashboard.Queries.GetLabourByProvince;

public sealed class GetLabourByProvinceQueryHandler
    : IRequestHandler<
        GetLabourByProvinceQuery,
        IReadOnlyList<LabourByProvinceDto>>
{
    private readonly ILabourDashboardReadRepository _repository;

    public GetLabourByProvinceQueryHandler(
        ILabourDashboardReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<LabourByProvinceDto>> Handle(
        GetLabourByProvinceQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetByProvinceAsync(
            request.ProvinceCode,
            request.Year,
            cancellationToken);
    }
}
