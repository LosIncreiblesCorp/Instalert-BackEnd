using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Business.Domain.Repositories;

namespace InstAlertPlatform.API.Business.Application.Internal.QueryServices;

public sealed class StaffInvitationQueryService(
    IStaffInvitationRepository invitations,
    IBusinessProfileQueryService businesses) : IStaffInvitationQueryService
{
    public async Task<List<InvitationSnapshot>> Handle(GetStaffInvitationsQuery query,
        CancellationToken cancellationToken = default)
    {
        await businesses.Handle(
            new RequireBusinessAccessQuery(query.Id, query.UserId, OwnerOnly: true), cancellationToken);

        return (await invitations.FindByBusinessIdAsync(query.Id.ToString(), cancellationToken))
            .Select(invitation => new InvitationSnapshot(invitation.Id, invitation.BusinessId.ToString(),
                invitation.DisplayName, invitation.Email, invitation.Status, invitation.LastRequestedAt,
                invitation.Version))
            .ToList();
    }
}
