using InstAlertPlatform.API.Payments.Domain.Model.Commands;
using InstAlertPlatform.API.Payments.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Payments.Interfaces.Rest.Transform;
public static class VerifySubscriptionCommandFromResourceAssembler
{
    public static VerifySubscriptionCommand ToCommandFromResource(VerifySubscriptionResource resource, string businessId) => new(businessId, resource.CheckoutId, resource.SubscriptionId);
}
