using InstAlertPlatform.API.Resources;
using Microsoft.Extensions.Localization;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Business.Domain.Model.Commands;
using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Business.Interfaces.Rest.Resources;
using InstAlertPlatform.API.Business.Interfaces.Rest.Transform;
using InstAlertPlatform.API.Shared.Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace InstAlertPlatform.API.Business.Interfaces.Rest;
[ApiController, Route("api/v1/[controller]"), Tags("Business")]
[ProducesResponseType(typeof(ProblemDetails), 400), ProducesResponseType(typeof(ProblemDetails), 401), ProducesResponseType(typeof(ProblemDetails), 403), ProducesResponseType(typeof(ProblemDetails), 404), ProducesResponseType(typeof(ProblemDetails), 409), ProducesResponseType(typeof(ProblemDetails), 503)]
[ProducesResponseType(typeof(ProblemDetails), 500)]
public sealed class StaffInvitationsController(IStaffInvitationCommandService staffInvitationCommands, IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(StaffInvitationResource), 200)]
    public async Task<IActionResult> Update(string id, UpdateStaffInvitationResource resource, CancellationToken cancellationToken)
    {
        var result = await staffInvitationCommands.Handle(UpdateStaffInvitationCommandFromResourceAssembler.ToCommandFromResource(resource, id), cancellationToken);
        return ActionResultFromUpdateStaffInvitationResultAssembler.ToActionResultFromUpdateStaffInvitationResult(result, this, localizer, snapshot => Ok(StaffInvitationResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot)));
    }

    [HttpPost("{id}/cancellation")]
    [ProducesResponseType(typeof(StaffInvitationResource), 200)]
    public async Task<IActionResult> Cancel(string id, CancellationToken cancellationToken)
    {
        var result = await staffInvitationCommands.Handle(new CancelStaffInvitationCommand(id), cancellationToken);
        return ActionResultFromCancelStaffInvitationResultAssembler.ToActionResultFromCancelStaffInvitationResult(result, this, localizer, snapshot => Ok(StaffInvitationResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot)));
    }

    [HttpPost("{id}/resend")]
    [ProducesResponseType(typeof(StaffInvitationResource), 200)]
    public async Task<IActionResult> Resend(string id, CancellationToken cancellationToken)
    {
        var result = await staffInvitationCommands.Handle(new ResendStaffInvitationCommand(id), cancellationToken);
        return ActionResultFromResendStaffInvitationResultAssembler.ToActionResultFromResendStaffInvitationResult(result, this, localizer, snapshot => Ok(StaffInvitationResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot)));
    }

    [HttpPost("{id}/acceptance")]
    [ProducesResponseType(typeof(BusinessMemberResource), 200)]
    public async Task<IActionResult> Accept(string id, CancellationToken cancellationToken)
    {
        var result = await staffInvitationCommands.Handle(new AcceptStaffInvitationCommand(id), cancellationToken);
        return ActionResultFromAcceptStaffInvitationResultAssembler.ToActionResultFromAcceptStaffInvitationResult(result, this, localizer, snapshot => Ok(BusinessMemberResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot)));
    }
}
