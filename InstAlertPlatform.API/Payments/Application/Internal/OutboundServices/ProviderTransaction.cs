namespace InstAlertPlatform.API.Payments.Application.Internal.OutboundServices;
public sealed record ProviderTransaction(string Id, decimal Amount, string Currency, DateTimeOffset PaidAt);
