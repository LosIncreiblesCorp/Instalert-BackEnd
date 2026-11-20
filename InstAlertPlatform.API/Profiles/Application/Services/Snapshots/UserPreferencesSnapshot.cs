namespace InstAlertPlatform.API.Profiles.Application.Services.Snapshots;
public sealed record UserPreferencesSnapshot(string UserId, string Locale, DateTimeOffset? UpdatedAt, string Version);
