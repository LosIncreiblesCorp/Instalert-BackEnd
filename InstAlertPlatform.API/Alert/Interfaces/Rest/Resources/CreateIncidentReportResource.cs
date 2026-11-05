namespace InstAlertPlatform.API.Alert.Interfaces.Rest.Resources;
public sealed record CreateIncidentReportResource(string BusinessId, string Kind, string Category, string Description, string Location, double? Latitude, double? Longitude, DateTimeOffset Moment);
