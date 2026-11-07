using InstAlertPlatform.API.Notifications.Application.Services;
using InstAlertPlatform.API.Notifications.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Notifications.Interfaces.Rest.Transform;
public static class NotificationResourceFromSnapshotAssembler
{
    public static NotificationResource ToResourceFromSnapshot(NotificationSnapshot snapshot) => new(snapshot.Id, snapshot.RecipientUserId, snapshot.TargetBusinessId, snapshot.Type, snapshot.Title, snapshot.Message, snapshot.SourceAlertId, snapshot.DistanceMeters, snapshot.CreatedAt, snapshot.ReadAt);
}
