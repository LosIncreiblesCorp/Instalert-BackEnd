namespace InstAlertPlatform.API.Payments.Application.Services.Snapshots;
public sealed record PaymentSnapshot(string Id, string BusinessId, string Folio, DateTimeOffset IssuedAt, decimal Amount, string Currency, string Status);
