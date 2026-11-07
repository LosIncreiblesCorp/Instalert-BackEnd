namespace InstAlertPlatform.API.Notifications.Application.Services;
public sealed record NotificationSnapshot(string Id, string RecipientUserId, string TargetBusinessId, string Type, string Title, string Message, string SourceAlertId, double DistanceMeters, DateTimeOffset? CreatedAt, DateTimeOffset? ReadAt);
