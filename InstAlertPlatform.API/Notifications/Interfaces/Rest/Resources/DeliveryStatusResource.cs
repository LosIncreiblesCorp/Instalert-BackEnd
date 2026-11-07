namespace InstAlertPlatform.API.Notifications.Interfaces.Rest.Resources;
public sealed record DeliveryStatusResource(string Id, string SourceAlertId, string? SourceInvitationId, string Channel, string Status, int Attempts, DateTimeOffset? SentAt);
