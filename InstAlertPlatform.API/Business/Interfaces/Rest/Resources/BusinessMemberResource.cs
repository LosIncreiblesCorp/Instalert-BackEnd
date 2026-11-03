namespace InstAlertPlatform.API.Business.Interfaces.Rest.Resources;
public sealed record BusinessMemberResource(string Id, string BusinessId, string UserId, string DisplayName, string Email, string Role, string Status, string Version);
