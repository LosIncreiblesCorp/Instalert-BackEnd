namespace InstAlertPlatform.API.Payments.Domain.Model.Commands;
public sealed record VerifySubscriptionPlanChangeCommand(string BusinessId, string CheckoutId);
