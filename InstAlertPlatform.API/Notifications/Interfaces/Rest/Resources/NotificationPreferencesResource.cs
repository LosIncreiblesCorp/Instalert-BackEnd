namespace InstAlertPlatform.API.Notifications.Interfaces.Rest.Resources;
public sealed record NotificationPreferencesResource(string UserId, string BusinessId, bool NearbyAlertsEnabled, int NearbyRadiusMeters, string? Version);
