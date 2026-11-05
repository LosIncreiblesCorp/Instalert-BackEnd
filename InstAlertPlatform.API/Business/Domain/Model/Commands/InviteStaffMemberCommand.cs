namespace InstAlertPlatform.API.Business.Domain.Model.Commands;
public sealed record InviteStaffMemberCommand(string BusinessId, string DisplayName, string Email);
