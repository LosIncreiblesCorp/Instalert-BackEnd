using InstAlertPlatform.API.Profiles.Application.Services;
using InstAlertPlatform.API.Profiles.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Profiles.Interfaces.Rest.Transform;
public static class UserPreferencesResourceFromSnapshotAssembler
{
    public static UserPreferencesResource ToResourceFromSnapshot(UserPreferencesSnapshot snapshot) => new(snapshot.UserId, snapshot.Locale, snapshot.UpdatedAt, snapshot.Version);
}
