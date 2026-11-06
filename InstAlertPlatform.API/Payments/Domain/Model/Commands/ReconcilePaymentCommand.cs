namespace InstAlertPlatform.API.Payments.Domain.Model.Commands;
public sealed record ReconcilePaymentCommand(string EventId, string EventType, string ProviderSubscriptionId);
