namespace InstAlertPlatform.API.Business.Application.Services.Snapshots;
public sealed record MemberSnapshot(string Id, string BusinessId, string UserId, string DisplayName, string Email, string Role, string Status, string Version);
