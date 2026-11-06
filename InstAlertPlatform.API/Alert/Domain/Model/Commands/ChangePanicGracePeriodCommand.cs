namespace InstAlertPlatform.API.Alert.Domain.Model.Commands;
public sealed record ChangePanicGracePeriodCommand(string UserId, int PanicGracePeriodSeconds);
