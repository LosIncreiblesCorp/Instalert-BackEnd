namespace InstAlertPlatform.API.Payments.Domain.Model.Commands;
public sealed record BeginSubscriptionCheckoutCommand(string Id, string PlanId);
