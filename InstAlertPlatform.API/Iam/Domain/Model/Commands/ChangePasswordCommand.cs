namespace InstAlertPlatform.API.Iam.Domain.Model.Commands;
public sealed record ChangePasswordCommand(string CurrentPassword, string NewPassword);
