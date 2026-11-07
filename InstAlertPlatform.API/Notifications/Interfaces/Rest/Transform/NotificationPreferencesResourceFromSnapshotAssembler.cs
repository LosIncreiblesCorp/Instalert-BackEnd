using InstAlertPlatform.API.Notifications.Application.Services;
using InstAlertPlatform.API.Notifications.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Notifications.Interfaces.Rest.Transform;
public static class NotificationPreferencesResourceFromSnapshotAssembler
{
    public static NotificationPreferencesResource ToResourceFromSnapshot(PreferencesSnapshot snapshot) => new(snapshot.UserId, snapshot.BusinessId, snapshot.NearbyAlertsEnabled, snapshot.NearbyRadiusMeters, snapshot.Version);
}
