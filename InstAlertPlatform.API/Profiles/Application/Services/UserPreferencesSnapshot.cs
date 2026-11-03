namespace InstAlertPlatform.API.Profiles.Application.Services;
public sealed record UserPreferencesSnapshot(string UserId, string Locale, DateTimeOffset? UpdatedAt, string Version);
