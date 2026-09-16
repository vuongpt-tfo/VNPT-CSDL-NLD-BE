using Dashboard.Application.Common.Dtos;

namespace Dashboard.Application.Abstractions.Persistence;

public interface ILabourDashboardReadRepository
{
    Task<IReadOnlyList<LabourAgeDto>> GetByAgeAsync(
        string? provinceCode,
        int? year,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<LabourEconomicStatusDto>> GetByEconomicStatusAsync(
        string? provinceCode,
        int? year,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<LabourByProvinceDto>> GetByProvinceAsync(
        string? provinceCode,
        int? year,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<LabourGenderDto>> GetByGenderAsync(
        string? provinceCode,
        int? year,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<LabourIndustryDto>> GetByIndustryAsync(
        string? provinceCode,
        int? year,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<LabourEducationLevelDto>> GetByEducationLevelAsync(
        string? provinceCode,
        int? year,
        CancellationToken cancellationToken);


    Task<LabourKpiSummaryDto> GetKpiSummaryAsync(
        string? provinceCode,
        int? year,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<LabourTrendDto>> GetTrendAsync(
        string? provinceCode,
        int? year,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<int>> GetAvailableYearsAsync(
        CancellationToken cancellationToken);

}
