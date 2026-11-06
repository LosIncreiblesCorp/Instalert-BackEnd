namespace InstAlertPlatform.API.Alert.Domain.Model.Commands;
public sealed record ConfirmPanicCommand(string PreparationToken, string? Location, double? Latitude, double? Longitude);
