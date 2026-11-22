namespace InstAlertPlatform.API.Notifications.Application.Services.Snapshots;
public sealed record PreferencesSnapshot(string UserId, string BusinessId, bool NearbyAlertsEnabled, int NearbyRadiusMeters, string? Version);
