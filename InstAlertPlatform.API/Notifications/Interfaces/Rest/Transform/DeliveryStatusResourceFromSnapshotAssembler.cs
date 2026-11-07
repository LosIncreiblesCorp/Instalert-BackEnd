using InstAlertPlatform.API.Notifications.Application.Services;
using InstAlertPlatform.API.Notifications.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Notifications.Interfaces.Rest.Transform;
public static class DeliveryStatusResourceFromSnapshotAssembler
{
    public static DeliveryStatusResource ToResourceFromSnapshot(DeliveryStatusSnapshot snapshot) => new(snapshot.Id, snapshot.SourceAlertId, snapshot.SourceInvitationId, snapshot.Channel, snapshot.Status, snapshot.Attempts, snapshot.SentAt);
}
