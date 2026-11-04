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
public sealed class BusinessMembersController(IBusinessMemberQueryService businessMemberQueries, IBusinessMemberCommandService businessMemberCommands, IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet("me")]
    [ProducesResponseType(typeof(List<MembershipStatusResource>), 200)]
    public async Task<IActionResult> OwnMemberships(CancellationToken cancellationToken)
    {
        var snapshot = await businessMemberQueries.Handle(new GetOwnMembershipsQuery(), cancellationToken);
        return Ok(snapshot.Select(MembershipStatusResourceFromSnapshotAssembler.ToResourceFromSnapshot).ToList());
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(BusinessMemberResource), 200)]
    public async Task<IActionResult> Update(string id, UpdateBusinessMemberResource resource, CancellationToken cancellationToken)
    {
        var result = await businessMemberCommands.Handle(UpdateBusinessMemberCommandFromResourceAssembler.ToCommandFromResource(resource, id), cancellationToken);
        return ActionResultFromUpdateBusinessMemberResultAssembler.ToActionResultFromUpdateBusinessMemberResult(result, this, localizer, snapshot => Ok(BusinessMemberResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot)));
    }

    [HttpPost("{id}/activation")]
    [ProducesResponseType(typeof(BusinessMemberResource), 200)]
    public async Task<IActionResult> Activate(string id, CancellationToken cancellationToken)
    {
        var result = await businessMemberCommands.Handle(new ChangeBusinessMemberStatusCommand(id, true), cancellationToken);
        return ActionResultFromChangeBusinessMemberStatusResultAssembler.ToActionResultFromChangeBusinessMemberStatusResult(result, this, localizer, snapshot => Ok(BusinessMemberResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot)));
    }

    [HttpPost("{id}/deactivation")]
    [ProducesResponseType(typeof(BusinessMemberResource), 200)]
    public async Task<IActionResult> Deactivate(string id, CancellationToken cancellationToken)
    {
        var result = await businessMemberCommands.Handle(new ChangeBusinessMemberStatusCommand(id, false), cancellationToken);
        return ActionResultFromChangeBusinessMemberStatusResultAssembler.ToActionResultFromChangeBusinessMemberStatusResult(result, this, localizer, snapshot => Ok(BusinessMemberResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot)));
    }
}
