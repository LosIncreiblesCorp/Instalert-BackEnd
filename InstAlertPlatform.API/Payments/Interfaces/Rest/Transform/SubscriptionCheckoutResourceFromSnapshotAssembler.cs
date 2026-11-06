using InstAlertPlatform.API.Payments.Application.Services;
using InstAlertPlatform.API.Payments.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Payments.Interfaces.Rest.Transform;
public static class SubscriptionCheckoutResourceFromSnapshotAssembler
{
    public static SubscriptionCheckoutResource ToResourceFromSnapshot(SubscriptionCheckoutSnapshot snapshot) => new(snapshot.CheckoutId, snapshot.PlanId, snapshot.PayPalPlanId, snapshot.CustomId, snapshot.ExpiresAt, snapshot.SubscriptionId);
}
