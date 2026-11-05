namespace InstAlertPlatform.API.Business.Domain.Model.Commands;
public sealed record ChangeBusinessMemberStatusCommand(string Id, bool Active);
