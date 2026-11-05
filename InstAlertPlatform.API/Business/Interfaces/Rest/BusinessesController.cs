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
public sealed class BusinessesController(IBusinessProfileQueryService businessProfileQueries, IBusinessProfileCommandService businessProfileCommands, IBusinessMemberQueryService businessMemberQueries, IStaffInvitationQueryService staffInvitationQueries, IStaffInvitationCommandService staffInvitationCommands, IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<BusinessResource>), 200)]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var snapshot = await businessProfileQueries.Handle(new GetBusinessesQuery(), cancellationToken);
        return Ok(snapshot.Select(BusinessResourceFromSnapshotAssembler.ToResourceFromSnapshot).ToList());
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BusinessResource), 200)]
    public async Task<IActionResult> Get(string id, CancellationToken cancellationToken)
    {
        var snapshot = await businessProfileQueries.Handle(new GetBusinessByIdQuery(id), cancellationToken);
        return Ok(BusinessResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot));
    }

    [HttpPost]
    [ProducesResponseType(typeof(BusinessResource), 201)]
    public async Task<IActionResult> Create(CreateBusinessResource resource, CancellationToken cancellationToken)
    {
        var result = await businessProfileCommands.Handle(CreateBusinessCommandFromResourceAssembler.ToCommandFromResource(resource), cancellationToken);
        return ActionResultFromCreateBusinessResultAssembler.ToActionResultFromCreateBusinessResult(result, this, localizer, snapshot => CreatedAtAction(nameof(Get), new { id = snapshot.Id }, BusinessResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot)));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(BusinessResource), 200)]
    public async Task<IActionResult> Update(string id, UpdateBusinessResource resource, CancellationToken cancellationToken)
    {
        var result = await businessProfileCommands.Handle(UpdateBusinessCommandFromResourceAssembler.ToCommandFromResource(resource, id), cancellationToken);
        return ActionResultFromUpdateBusinessResultAssembler.ToActionResultFromUpdateBusinessResult(result, this, localizer, snapshot => Ok(BusinessResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot)));
    }

    [HttpGet("{id}/seats")]
    [ProducesResponseType(typeof(BusinessSeatsResource), 200)]
    public async Task<IActionResult> Seats(string id, CancellationToken cancellationToken)
    {
        var snapshot = await businessProfileQueries.Handle(new GetBusinessSeatsQuery(id), cancellationToken);
        return Ok(BusinessSeatsResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot));
    }

    [HttpGet("{id}/members")]
    [ProducesResponseType(typeof(List<BusinessMemberResource>), 200)]
    public async Task<IActionResult> Members(string id, CancellationToken cancellationToken)
    {
        var snapshot = await businessMemberQueries.Handle(new GetBusinessMembersQuery(id), cancellationToken);
        return Ok(snapshot.Select(BusinessMemberResourceFromSnapshotAssembler.ToResourceFromSnapshot).ToList());
    }

    [HttpGet("{id}/invitations")]
    [ProducesResponseType(typeof(List<StaffInvitationResource>), 200)]
    public async Task<IActionResult> Invitations(string id, CancellationToken cancellationToken)
    {
        var snapshot = await staffInvitationQueries.Handle(new GetStaffInvitationsQuery(id), cancellationToken);
        return Ok(snapshot.Select(StaffInvitationResourceFromSnapshotAssembler.ToResourceFromSnapshot).ToList());
    }

    [HttpPost("{id}/invitations")]
    [ProducesResponseType(typeof(StaffInvitationResource), 201)]
    public async Task<IActionResult> Invite(string id, InviteStaffMemberResource resource, CancellationToken cancellationToken)
    {
        var result = await staffInvitationCommands.Handle(InviteStaffMemberCommandFromResourceAssembler.ToCommandFromResource(resource, id), cancellationToken);
        return ActionResultFromInviteStaffMemberResultAssembler.ToActionResultFromInviteStaffMemberResult(result, this, localizer, snapshot => StatusCode(201, StaffInvitationResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot)));
    }
}
