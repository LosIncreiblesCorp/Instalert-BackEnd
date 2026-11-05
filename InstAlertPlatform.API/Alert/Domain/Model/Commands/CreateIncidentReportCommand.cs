namespace InstAlertPlatform.API.Alert.Domain.Model.Commands;
public sealed record CreateIncidentReportCommand(string BusinessId, string Kind, string Category, string Description, string Location, double? Latitude, double? Longitude, DateTimeOffset Moment);
