using InstAlertPlatform.API.Profiles.Application.Services;
using InstAlertPlatform.API.Profiles.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Profiles.Interfaces.Rest.Transform;
public static class UserProfileResourceFromSnapshotAssembler
{
    public static UserProfileResource ToResourceFromSnapshot(ProfileSnapshot snapshot) => new(snapshot.UserId, snapshot.FirstName, snapshot.LastName, snapshot.Dni, snapshot.Phone, snapshot.UpdatedAt, snapshot.Version);
}
