using BuildingBlocks.SharedKernel.Common;
using Dashboard.Application.Common.Dtos;
using Dashboard.Application.Features.LabourDashboard.Queries.GetLabourByAge;
using Dashboard.Application.Features.LabourDashboard.Queries.GetLabourByEconomicStatus;
using Dashboard.Application.Features.LabourDashboard.Queries.GetLabourByEducationLevel;
using Dashboard.Application.Features.LabourDashboard.Queries.GetLabourByGender;
using Dashboard.Application.Features.LabourDashboard.Queries.GetLabourByIndustry;
using Dashboard.Application.Features.LabourDashboard.Queries.GetLabourByProvince;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Dashboard.Application.Features.LabourDashboard.Queries.GetLabourKpiSummary;
using Dashboard.Application.Features.LabourDashboard.Queries.GetLabourTrend;
using Dashboard.Application.Features.LabourDashboard.Queries.GetLabourAvailableYears;

namespace Dashboard.API.Controllers;

[ApiController]
[Route("api/v1/dashboard/labour")]
//[Authorize]
public class LabourDashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public LabourDashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ============================================================
    // GET api/v1/dashboard/labour/by-age
    // ============================================================

    [HttpGet("by-age")]
    [ProducesResponseType(
        typeof(ApiResult<IReadOnlyList<LabourAgeDto>>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> ByAge(
        [FromQuery] string? provinceCode,
        [FromQuery] int? year,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new GetLabourByAgeQuery(provinceCode, year),
            ct);

        return Ok(ApiResult.Ok(result));
    }

    // ============================================================
    // GET api/v1/dashboard/labour/by-economic-status
    // ============================================================

    [HttpGet("by-economic-status")]
    [ProducesResponseType(
        typeof(ApiResult<IReadOnlyList<LabourEconomicStatusDto>>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> ByEconomicStatus(
        [FromQuery] int? year,
        [FromQuery] string? provinceCode,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new GetLabourByEconomicStatusQuery(year, provinceCode),
            ct);

        return Ok(ApiResult.Ok(result));
    }

    // ============================================================
    // GET api/v1/dashboard/labour/by-province
    // ============================================================

    [HttpGet("by-province")]
    [ProducesResponseType(
        typeof(ApiResult<IReadOnlyList<LabourByProvinceDto>>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> ByProvince(
        [FromQuery] string? provinceCode,
        [FromQuery] int? year,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new GetLabourByProvinceQuery(provinceCode, year),
            ct);

        return Ok(ApiResult.Ok(result));
    }

    // ============================================================
    // GET api/v1/dashboard/labour/by-gender
    // ============================================================

    [HttpGet("by-gender")]
    [ProducesResponseType(
        typeof(ApiResult<IReadOnlyList<LabourGenderDto>>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> ByGender(
        [FromQuery] string? provinceCode,
        [FromQuery] int? year,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new GetLabourByGenderQuery(provinceCode, year),
            ct);

        return Ok(ApiResult.Ok(result));
    }

    // ============================================================
    // GET api/v1/dashboard/labour/by-industry
    // ============================================================

    [HttpGet("by-industry")]
    [ProducesResponseType(
        typeof(ApiResult<IReadOnlyList<LabourIndustryDto>>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> ByIndustry(
        [FromQuery] string? provinceCode,
        [FromQuery] int? year,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new GetLabourByIndustryQuery(provinceCode, year),
            ct);

        return Ok(ApiResult.Ok(result));
    }

    // ============================================================
    // GET api/v1/dashboard/labour/by-education-level
    // ============================================================

    [HttpGet("by-education-level")]
    [ProducesResponseType(
        typeof(ApiResult<IReadOnlyList<LabourEducationLevelDto>>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> ByEducationLevel(
        [FromQuery] string? provinceCode,
        [FromQuery] int? year,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new GetLabourByEducationLevelQuery(provinceCode, year),
            ct);

        return Ok(ApiResult.Ok(result));
    }

    // ============================================================
    // GET api/v1/dashboard/labour/kpi-summary
    // ============================================================

    [HttpGet("kpi-summary")]
    [ProducesResponseType(
        typeof(ApiResult<LabourKpiSummaryDto>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> KpiSummary(
        [FromQuery] string? provinceCode,
        [FromQuery] int? year,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new GetLabourKpiSummaryQuery(provinceCode, year),
            ct);

        return Ok(ApiResult.Ok(result));
    }

    // ============================================================
    // GET api/v1/dashboard/labour/trend
    // ============================================================

    [HttpGet("trend")]
    [ProducesResponseType(
        typeof(ApiResult<IReadOnlyList<LabourTrendDto>>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> Trend(
        [FromQuery] string? provinceCode,
        [FromQuery] int? year,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new GetLabourTrendQuery(provinceCode, year),
            ct);

        return Ok(ApiResult.Ok(result));
    }

    // ============================================================
    // GET api/v1/dashboard/labour/available-years
    // ============================================================

    [HttpGet("available-years")]
    [ProducesResponseType(
        typeof(ApiResult<IReadOnlyList<int>>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> AvailableYears(
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new GetLabourAvailableYearsQuery(),
            ct);

        return Ok(ApiResult.Ok(result));
    }
}
