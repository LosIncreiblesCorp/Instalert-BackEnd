namespace InstAlertPlatform.API.Profiles.Application.Services.Snapshots;
public sealed record ProfileSnapshot(string UserId, string FirstName, string LastName, string Dni, string? Phone, DateTimeOffset? UpdatedAt, string Version);
