namespace InstAlertPlatform.API.Notifications.Application.Services;
public sealed record DeliveryStatusSnapshot(string Id, string SourceAlertId, string? SourceInvitationId, string Channel, string Status, int Attempts, DateTimeOffset? SentAt);
