namespace InstAlertPlatform.API.Alert.Interfaces.Rest.Resources;
public sealed record UpdatePanicReportDraftResource(string Category, string Description, string Location, double? Latitude, double? Longitude, DateTimeOffset? Moment);
