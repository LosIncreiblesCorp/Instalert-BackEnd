using InstAlertPlatform.API.Resources;
using Microsoft.Extensions.Localization;
using InstAlertPlatform.API.Contacts.Application.Services;
using InstAlertPlatform.API.Contacts.Domain.Model.Commands;
using InstAlertPlatform.API.Contacts.Domain.Model.Queries;
using InstAlertPlatform.API.Contacts.Interfaces.Rest.Resources;
using InstAlertPlatform.API.Contacts.Interfaces.Rest.Transform;
using InstAlertPlatform.API.Shared.Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace InstAlertPlatform.API.Contacts.Interfaces.Rest;
[ApiController, Route("api/v1/[controller]"), Tags("Contacts")]
[ProducesResponseType(typeof(ProblemDetails), 400), ProducesResponseType(typeof(ProblemDetails), 401), ProducesResponseType(typeof(ProblemDetails), 403), ProducesResponseType(typeof(ProblemDetails), 404), ProducesResponseType(typeof(ProblemDetails), 409), ProducesResponseType(typeof(ProblemDetails), 503)]
[ProducesResponseType(typeof(ProblemDetails), 500)]
public sealed class EmergencyContactsController(IEmergencyContactQueryService emergencyContactQueries, IEmergencyContactCommandService emergencyContactCommands, IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<EmergencyContactResource>), 200)]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var snapshot = await emergencyContactQueries.Handle(new GetEmergencyContactsQuery(), cancellationToken);
        return Ok(snapshot.Select(EmergencyContactResourceFromSnapshotAssembler.ToResourceFromSnapshot).ToList());
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EmergencyContactResource), 200)]
    public async Task<IActionResult> Get(string id, CancellationToken cancellationToken)
    {
        var snapshot = await emergencyContactQueries.Handle(new GetEmergencyContactByIdQuery(id), cancellationToken);
        return Ok(EmergencyContactResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot));
    }

    [HttpPost]
    [ProducesResponseType(typeof(EmergencyContactResource), 201)]
    public async Task<IActionResult> Create(CreateEmergencyContactResource command, CancellationToken cancellationToken)
    {
        var result = await emergencyContactCommands.Handle(CreateEmergencyContactCommandFromResourceAssembler.ToCommandFromResource(command), cancellationToken);
        return ActionResultFromCreateEmergencyContactResultAssembler.ToActionResultFromCreateEmergencyContactResult(result, this, localizer, snapshot => CreatedAtAction(nameof(Get), new { id = snapshot.Id }, EmergencyContactResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot)));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(EmergencyContactResource), 200)]
    public async Task<IActionResult> Update(string id, UpdateEmergencyContactResource command, CancellationToken cancellationToken)
    {
        var result = await emergencyContactCommands.Handle(UpdateEmergencyContactCommandFromResourceAssembler.ToCommandFromResource(command, id), cancellationToken);
        return ActionResultFromUpdateEmergencyContactResultAssembler.ToActionResultFromUpdateEmergencyContactResult(result, this, localizer, snapshot => Ok(EmergencyContactResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot)));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var result = await emergencyContactCommands.Handle(new DeleteEmergencyContactCommand(id), cancellationToken);
        return ActionResultFromDeleteEmergencyContactResultAssembler.ToActionResultFromDeleteEmergencyContactResult(result, this, localizer, _ => NoContent());
    }
}
