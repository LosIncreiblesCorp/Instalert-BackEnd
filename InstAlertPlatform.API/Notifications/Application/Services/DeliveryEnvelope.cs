namespace InstAlertPlatform.API.Notifications.Application.Services;
public sealed record DeliveryEnvelope(string Id, string Channel, string Destination, string Subject, string Text, string SourceAlertId);
