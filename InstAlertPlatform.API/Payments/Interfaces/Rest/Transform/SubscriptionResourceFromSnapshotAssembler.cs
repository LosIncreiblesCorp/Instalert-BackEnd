using InstAlertPlatform.API.Payments.Application.Services;
using InstAlertPlatform.API.Payments.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Payments.Interfaces.Rest.Transform;
public static class SubscriptionResourceFromSnapshotAssembler
{
    public static SubscriptionResource ToResourceFromSnapshot(SubscriptionSnapshot snapshot) => new(snapshot.Id, snapshot.BusinessId, snapshot.PlanId, snapshot.Status, snapshot.CurrentPeriodEnd, snapshot.CancelAtPeriodEnd, snapshot.Version, snapshot.IsSimulated);
}
