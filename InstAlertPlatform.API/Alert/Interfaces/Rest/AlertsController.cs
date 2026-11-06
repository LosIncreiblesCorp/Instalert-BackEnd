using InstAlertPlatform.API.Resources;
using Microsoft.Extensions.Localization;
using InstAlertPlatform.API.Alert.Application.Services;
using InstAlertPlatform.API.Alert.Domain.Model.Commands;
using InstAlertPlatform.API.Alert.Domain.Model.Queries;
using InstAlertPlatform.API.Alert.Interfaces.Rest.Resources;
using InstAlertPlatform.API.Alert.Interfaces.Rest.Transform;
using InstAlertPlatform.API.Shared.Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace InstAlertPlatform.API.Alert.Interfaces.Rest;
[ApiController, Route("api/v1/[controller]"), Tags("Alert")]
[ProducesResponseType(typeof(ProblemDetails), 400), ProducesResponseType(typeof(ProblemDetails), 401), ProducesResponseType(typeof(ProblemDetails), 403), ProducesResponseType(typeof(ProblemDetails), 404), ProducesResponseType(typeof(ProblemDetails), 409), ProducesResponseType(typeof(ProblemDetails), 503)]
[ProducesResponseType(typeof(ProblemDetails), 500)]
public sealed class AlertsController(IAlertQueryService alertQueries, IAlertCommandService alertCommands, IAlertPreferencesQueryService alertPreferencesQueries, IAlertPreferencesCommandService alertPreferencesCommands, IStringLocalizer<SharedResource> localizer) : ControllerBase
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

    [HttpPost("panic-preparations")]
    [ProducesResponseType(typeof(PanicPreparationResource), 200)]
    public async Task<IActionResult> Prepare(PreparePanicResource resource, CancellationToken cancellationToken)
    {
        var result = await alertCommands.Handle(PreparePanicCommandFromResourceAssembler.ToCommandFromResource(resource), cancellationToken);
        return ActionResultFromPreparePanicResultAssembler.ToActionResultFromPreparePanicResult(result, this, localizer, snapshot => Ok(PanicPreparationResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot)));
    }

    [HttpPost("panic")]
    [ProducesResponseType(typeof(AlertResource), 200)]
    public async Task<IActionResult> Confirm(ConfirmPanicResource resource, CancellationToken cancellationToken)
    {
        var result = await alertCommands.Handle(ConfirmPanicCommandFromResourceAssembler.ToCommandFromResource(resource), cancellationToken);
        return ActionResultFromConfirmPanicResultAssembler.ToActionResultFromConfirmPanicResult(result, this, localizer, snapshot => Ok(AlertResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot)));
    }

    [HttpPost("reports")]
    [ProducesResponseType(typeof(AlertResource), 201)]
    public async Task<IActionResult> Report(CreateIncidentReportResource resource, CancellationToken cancellationToken)
    {
        var result = await alertCommands.Handle(CreateIncidentReportCommandFromResourceAssembler.ToCommandFromResource(resource), cancellationToken);
        return ActionResultFromCreateIncidentReportResultAssembler.ToActionResultFromCreateIncidentReportResult(result, this, localizer, snapshot => CreatedAtAction(nameof(Get), new { id = snapshot.Id }, AlertResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot)));
    }

    [HttpPut("{id}/location"), ProducesResponseType(typeof(AlertResource), 200)]
    public async Task<IActionResult> Location(string id, UpdateAlertLocationResource resource, CancellationToken cancellationToken)
    {
        var result = await alertCommands.Handle(UpdateAlertLocationCommandFromResourceAssembler.ToCommandFromResource(resource, id), cancellationToken);
        return ActionResultFromUpdateAlertLocationResultAssembler.ToActionResultFromUpdateAlertLocationResult(result, this, localizer, snapshot => Ok(AlertResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot)));
    }

    [HttpPost("{id}/resolution")]
    [ProducesResponseType(typeof(AlertResource), 200)]
    public async Task<IActionResult> Resolve(string id, CancellationToken cancellationToken)
    {
        var result = await alertCommands.Handle(new ResolvePanicAlertCommand(id), cancellationToken);
        return ActionResultFromResolvePanicAlertResultAssembler.ToActionResultFromResolvePanicAlertResult(result, this, localizer, snapshot => Ok(AlertResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot)));
    }

    [HttpPut("{id}/report-draft")]
    [ProducesResponseType(typeof(AlertResource), 200)]
    public async Task<IActionResult> Draft(string id, UpdatePanicReportDraftResource resource, CancellationToken cancellationToken)
    {
        var result = await alertCommands.Handle(UpdatePanicReportDraftCommandFromResourceAssembler.ToCommandFromResource(resource, id), cancellationToken);
        return ActionResultFromUpdatePanicReportDraftResultAssembler.ToActionResultFromUpdatePanicReportDraftResult(result, this, localizer, snapshot => Ok(AlertResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot)));
    }

    [HttpPost("{id}/report-completion")]
    [ProducesResponseType(typeof(AlertResource), 200)]
    public async Task<IActionResult> Complete(string id, CancellationToken cancellationToken)
    {
        var result = await alertCommands.Handle(new CompletePanicReportCommand(id), cancellationToken);
        return ActionResultFromCompletePanicReportResultAssembler.ToActionResultFromCompletePanicReportResult(result, this, localizer, snapshot => Ok(AlertResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot)));
    }

    [HttpGet("/api/v1/alert-preferences/{userId}")]
    [ProducesResponseType(typeof(AlertPreferencesResource), 200)]
    public async Task<IActionResult> Preferences(string userId, CancellationToken cancellationToken)
    {
        var snapshot = await alertPreferencesQueries.Handle(new GetPanicPreferencesQuery(userId), cancellationToken);
        return Ok(AlertPreferencesResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot));
    }

    [HttpPut("/api/v1/alert-preferences/{userId}")]
    [ProducesResponseType(typeof(AlertPreferencesResource), 200)]
    public async Task<IActionResult> ChangePreferences(string userId, ChangePanicGracePeriodResource resource, CancellationToken cancellationToken)
    {
        var result = await alertPreferencesCommands.Handle(ChangePanicGracePeriodCommandFromResourceAssembler.ToCommandFromResource(resource, userId), cancellationToken);
        return ActionResultFromChangePanicGracePeriodResultAssembler.ToActionResultFromChangePanicGracePeriodResult(result, this, localizer, snapshot => Ok(AlertPreferencesResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot)));
    }
}
