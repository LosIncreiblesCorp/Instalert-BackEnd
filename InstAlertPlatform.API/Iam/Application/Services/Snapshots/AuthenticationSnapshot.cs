namespace InstAlertPlatform.API.Iam.Application.Services.Snapshots;
public sealed record AuthenticationSnapshot(string AccessToken, DateTimeOffset ExpiresAt, string UserId, string Email, string Role, bool MustChangePassword);
