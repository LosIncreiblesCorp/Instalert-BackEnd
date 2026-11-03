namespace InstAlertPlatform.API.Business.Application.Services;
public sealed record InvitationSnapshot(string Id, string BusinessId, string DisplayName, string Email, string Status, DateTimeOffset LastRequestedAt, string Version);
