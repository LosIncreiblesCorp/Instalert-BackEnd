namespace InstAlertPlatform.API.Alert.Domain.Model.ValueObjects;
public sealed record PanicPreparation(string Id, string BusinessId, string UserId, string Location, double? Latitude, double? Longitude, DateTimeOffset NotBefore, DateTimeOffset ExpiresAt);
