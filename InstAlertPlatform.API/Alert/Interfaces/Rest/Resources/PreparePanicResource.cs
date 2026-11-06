namespace InstAlertPlatform.API.Alert.Interfaces.Rest.Resources;
public sealed record PreparePanicResource(string BusinessId, string Location, double? Latitude, double? Longitude);
