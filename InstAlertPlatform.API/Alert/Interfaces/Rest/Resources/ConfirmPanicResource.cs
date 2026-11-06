namespace InstAlertPlatform.API.Alert.Interfaces.Rest.Resources;
public sealed record ConfirmPanicResource(string PreparationToken, string? Location = null, double? Latitude = null, double? Longitude = null);
