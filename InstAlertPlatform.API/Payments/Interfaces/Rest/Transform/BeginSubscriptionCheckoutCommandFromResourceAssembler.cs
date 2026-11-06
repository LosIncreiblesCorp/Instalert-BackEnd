using InstAlertPlatform.API.Payments.Domain.Model.Commands;
using InstAlertPlatform.API.Payments.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Payments.Interfaces.Rest.Transform;
public static class BeginSubscriptionCheckoutCommandFromResourceAssembler
{
    public static BeginSubscriptionCheckoutCommand ToCommandFromResource(BeginSubscriptionCheckoutResource resource, string businessId) => new(businessId, resource.PlanId);
}
