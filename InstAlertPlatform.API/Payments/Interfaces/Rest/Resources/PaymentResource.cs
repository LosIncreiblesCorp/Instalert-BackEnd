namespace InstAlertPlatform.API.Payments.Interfaces.Rest.Resources;
public sealed record PaymentResource(string Id, string BusinessId, string Folio, DateTimeOffset IssuedAt, decimal Amount, string Currency, string Status);
