namespace InstAlertPlatform.API.Payments.Domain.Model.Commands;
public sealed record VerifySubscriptionCommand(string BusinessId, string CheckoutId, string SubscriptionId);
