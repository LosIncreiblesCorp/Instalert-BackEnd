namespace InstAlertPlatform.API.Alert.Domain.Model.Commands;
public sealed record UpdateAlertLocationCommand(string Id, string Location, double? Latitude, double? Longitude);
