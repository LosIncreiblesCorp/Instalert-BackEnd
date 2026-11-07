namespace InstAlertPlatform.API.Notifications.Interfaces.Rest.Resources;
public sealed record ChangeNotificationPreferencesResource(bool NearbyAlertsEnabled, int NearbyRadiusMeters);
