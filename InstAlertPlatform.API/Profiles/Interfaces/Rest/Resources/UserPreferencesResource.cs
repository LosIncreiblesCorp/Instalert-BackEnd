namespace InstAlertPlatform.API.Profiles.Interfaces.Rest.Resources;
public sealed record UserPreferencesResource(string UserId, string Locale, DateTimeOffset? UpdatedAt, string Version);
