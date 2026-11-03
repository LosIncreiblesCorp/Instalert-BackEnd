namespace InstAlertPlatform.API.Business.Interfaces.Rest.Resources;
public sealed record StaffInvitationResource(string Id, string BusinessId, string DisplayName, string Email, string Status, DateTimeOffset LastRequestedAt, string Version);
