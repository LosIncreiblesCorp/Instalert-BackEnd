using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Business.Domain.Model.Aggregates;
using InstAlertPlatform.API.Business.Domain.Model.Commands;
using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Business.Domain.Repositories;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Business.Application.Internal.QueryServices;
public sealed class StaffInvitationQueryService(IStaffInvitationRepository invitations, IBusinessProfileQueryService access, ICurrentActor actor) : IStaffInvitationQueryService
{
    public async Task<List<InvitationSnapshot>> Handle(GetStaffInvitationsQuery query, CancellationToken cancellationToken = default)
    {
        var id = query.Id;
        await access.Handle(new RequireBusinessAccessQuery(id, actor.UserId, true), cancellationToken);
        return (await invitations.FindByBusinessIdAsync(id, cancellationToken)).Select(ToInvitationSnapshot).ToList();
    }

    private static InvitationSnapshot ToInvitationSnapshot(StaffInvitation i) => new(i.Id, i.BusinessId.ToString(), i.DisplayName, i.Email, i.Status, i.LastRequestedAt, i.Version);
}
