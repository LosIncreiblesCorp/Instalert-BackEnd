namespace InstAlertPlatform.API.Payments.Application.Internal.OutboundServices;
public sealed record ProviderSubscription(string Id, string PlanId, string Status, string? CustomId, DateTimeOffset? PaidUntil, bool HasConfirmedPayment);
