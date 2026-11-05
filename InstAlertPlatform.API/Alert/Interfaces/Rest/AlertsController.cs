using InstAlertPlatform.API.Alert.Application.Services;
using InstAlertPlatform.API.Alert.Domain.Model.Commands;
using InstAlertPlatform.API.Alert.Domain.Model.Queries;
using InstAlertPlatform.API.Alert.Interfaces.Rest.Resources;
using InstAlertPlatform.API.Alert.Interfaces.Rest.Transform;
using InstAlertPlatform.API.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Alert.Interfaces.Rest;
[ApiController, Route("api/v1/[controller]"), Tags("Alert")]
[ProducesResponseType(typeof(ProblemDetails), 400), ProducesResponseType(typeof(ProblemDetails), 401), ProducesResponseType(typeof(ProblemDetails), 403), ProducesResponseType(typeof(ProblemDetails), 404), ProducesResponseType(typeof(ProblemDetails), 409), ProducesResponseType(typeof(ProblemDetails), 503)]
[ProducesResponseType(typeof(ProblemDetails), 500)]
public sealed class AlertsController(IAlertQueryService alertQueries, IAlertCommandService alertCommands, IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<AlertResource>), 200)]
    public async Task<IActionResult> List(string businessId, CancellationToken cancellationToken, string? status = null, int page = 1, int pageSize = 50)
    {
        var snapshot = await alertQueries.Handle(new GetAlertsQuery(businessId, status, page, pageSize), cancellationToken);
        return Ok(snapshot.Select(AlertResourceFromSnapshotAssembler.ToResourceFromSnapshot).ToList());
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AlertResource), 200)]
    public async Task<IActionResult> Get(string id, CancellationToken cancellationToken)
    {
        var snapshot = await alertQueries.Handle(new GetAlertByIdQuery(id), cancellationToken);
        return Ok(AlertResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot));
    }

    [HttpPost("reports")]
    [ProducesResponseType(typeof(AlertResource), 201)]
    public async Task<IActionResult> Report(CreateIncidentReportResource resource, CancellationToken cancellationToken)
    {
        var result = await alertCommands.Handle(CreateIncidentReportCommandFromResourceAssembler.ToCommandFromResource(resource), cancellationToken);
        return ActionResultFromCreateIncidentReportResultAssembler.ToActionResultFromCreateIncidentReportResult(result, this, localizer, snapshot => CreatedAtAction(nameof(Get), new { id = snapshot.Id }, AlertResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot)));
    }
}
