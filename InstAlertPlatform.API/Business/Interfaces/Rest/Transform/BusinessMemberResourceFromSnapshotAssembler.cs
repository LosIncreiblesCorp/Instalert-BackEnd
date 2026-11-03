using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Business.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Business.Interfaces.Rest.Transform;
public static class BusinessMemberResourceFromSnapshotAssembler
{
    public static BusinessMemberResource ToResourceFromSnapshot(MemberSnapshot snapshot) => new(snapshot.Id, snapshot.BusinessId, snapshot.UserId, snapshot.DisplayName, snapshot.Email, snapshot.Role, snapshot.Status, snapshot.Version);
}
