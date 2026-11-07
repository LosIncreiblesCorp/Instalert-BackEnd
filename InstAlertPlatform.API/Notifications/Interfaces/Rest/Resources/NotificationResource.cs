namespace InstAlertPlatform.API.Notifications.Interfaces.Rest.Resources;
public sealed record NotificationResource(string Id, string RecipientUserId, string TargetBusinessId, string Type, string Title, string Message, string SourceAlertId, double DistanceMeters, DateTimeOffset? CreatedAt, DateTimeOffset? ReadAt);
