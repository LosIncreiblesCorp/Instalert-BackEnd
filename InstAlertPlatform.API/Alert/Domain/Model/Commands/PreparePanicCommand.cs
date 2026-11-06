namespace InstAlertPlatform.API.Alert.Domain.Model.Commands;
public sealed record PreparePanicCommand(string BusinessId, string Location, double? Latitude, double? Longitude);
