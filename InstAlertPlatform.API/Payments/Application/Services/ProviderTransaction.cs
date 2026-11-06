namespace InstAlertPlatform.API.Payments.Application.Services;
public sealed record ProviderTransaction(string Id, decimal Amount, string Currency, DateTimeOffset PaidAt);
