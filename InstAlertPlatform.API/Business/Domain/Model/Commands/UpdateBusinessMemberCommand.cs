namespace InstAlertPlatform.API.Business.Domain.Model.Commands;
public sealed record UpdateBusinessMemberCommand(string Id, string DisplayName, string Email);
