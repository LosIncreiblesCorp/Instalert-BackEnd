namespace InstAlertPlatform.API.Business.Application.Services.Snapshots;
public sealed record InvitationSnapshot(string Id, string BusinessId, string DisplayName, string Email, string Status, DateTimeOffset LastRequestedAt, string Version, string? BusinessName = null);
