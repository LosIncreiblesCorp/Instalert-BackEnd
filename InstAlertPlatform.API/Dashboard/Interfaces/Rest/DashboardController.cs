using InstAlertPlatform.API.Resources;
using Microsoft.Extensions.Localization;
using InstAlertPlatform.API.Dashboard.Application.Services;
using InstAlertPlatform.API.Dashboard.Domain.Model.Commands;
using InstAlertPlatform.API.Dashboard.Domain.Model.Queries;
using InstAlertPlatform.API.Dashboard.Interfaces.Rest.Resources;
using InstAlertPlatform.API.Dashboard.Interfaces.Rest.Transform;
using InstAlertPlatform.API.Shared.Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace InstAlertPlatform.API.Dashboard.Interfaces.Rest;
[ApiController, Route("api/v1/businesses/{businessId}"), Tags("Dashboard")]
[ProducesResponseType(typeof(ProblemDetails), 400), ProducesResponseType(typeof(ProblemDetails), 401), ProducesResponseType(typeof(ProblemDetails), 403), ProducesResponseType(typeof(ProblemDetails), 404), ProducesResponseType(typeof(ProblemDetails), 409), ProducesResponseType(typeof(ProblemDetails), 503)]
[ProducesResponseType(typeof(ProblemDetails), 500)]
public sealed class DashboardController(IDashboardQueryService dashboardQueries, IReportCommandService reportCommands, IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(DashboardResource), 200)]
    public async Task<IActionResult> Current(string businessId, CancellationToken cancellationToken, int radiusMeters = 500)
    {
        var snapshot = await dashboardQueries.Handle(new GetCurrentDashboardQuery(businessId, radiusMeters), cancellationToken);
        return Ok(DashboardResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot));
    }

    [HttpGet("reports")]
    [ProducesResponseType(typeof(List<ReportResource>), 200)]
    public async Task<IActionResult> List(string businessId, CancellationToken cancellationToken)
    {
        var snapshot = await dashboardQueries.Handle(new GetReportsQuery(businessId), cancellationToken);
        return Ok(snapshot.Select(ReportResourceFromSnapshotAssembler.ToResourceFromSnapshot).ToList());
    }

    [HttpGet("reports/{id}")]
    [ProducesResponseType(typeof(ReportResource), 200)]
    public async Task<IActionResult> Get(string businessId, string id, CancellationToken cancellationToken)
    {
        var snapshot = await dashboardQueries.Handle(new GetReportByIdQuery(businessId, id), cancellationToken);
        return Ok(ReportResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot));
    }

    [HttpPost("reports")]
    [ProducesResponseType(typeof(ReportResource), 201)]
    public async Task<IActionResult> Generate(string businessId, GenerateReportResource resource, CancellationToken cancellationToken)
    {
        var result = await reportCommands.Handle(GenerateReportCommandFromResourceAssembler.ToCommandFromResource(businessId, resource), cancellationToken);
        return ActionResultFromGenerateReportResultAssembler.ToActionResultFromGenerateReportResult(result, this, localizer, snapshot => CreatedAtAction(nameof(Get), new { businessId, id = snapshot.Id }, ReportResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot)));
    }
}
