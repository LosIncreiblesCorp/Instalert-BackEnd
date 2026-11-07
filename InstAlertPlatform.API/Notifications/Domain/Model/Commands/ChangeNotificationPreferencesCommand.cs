namespace InstAlertPlatform.API.Notifications.Domain.Model.Commands;
public sealed record ChangeNotificationPreferencesCommand(string UserId, string BusinessId, bool NearbyAlertsEnabled, int NearbyRadiusMeters);
