using InstAlertPlatform.API.Payments.Domain.Model.Commands;
using InstAlertPlatform.API.Payments.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Payments.Interfaces.Rest.Transform;
public static class VerifySubscriptionPlanChangeCommandFromResourceAssembler
{
    public static VerifySubscriptionPlanChangeCommand ToCommandFromResource(VerifySubscriptionPlanChangeResource resource, string businessId) => new(businessId, resource.CheckoutId);
}
