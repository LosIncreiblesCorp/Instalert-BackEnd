using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Business.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Business.Interfaces.Rest.Transform;
public static class StaffInvitationResourceFromSnapshotAssembler
{
    public static StaffInvitationResource ToResourceFromSnapshot(InvitationSnapshot snapshot) => new(snapshot.Id, snapshot.BusinessId, snapshot.DisplayName, snapshot.Email, snapshot.Status, snapshot.LastRequestedAt, snapshot.Version);
}
