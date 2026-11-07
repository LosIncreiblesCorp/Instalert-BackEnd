namespace InstAlertPlatform.API.Notifications.Application.Services;
public sealed record PreferencesSnapshot(string UserId, string BusinessId, bool NearbyAlertsEnabled, int NearbyRadiusMeters, string? Version);
