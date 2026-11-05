using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Business.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Business.Interfaces.Rest.Transform;
public static class MembershipStatusResourceFromSnapshotAssembler
{
    public static MembershipStatusResource ToResourceFromSnapshot(MembershipStatusSnapshot snapshot) => new(snapshot.BusinessId, snapshot.Status);
}
