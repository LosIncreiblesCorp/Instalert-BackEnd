namespace InstAlertPlatform.API.Alert.Domain.Model.Commands;
public sealed record UpdatePanicReportDraftCommand(string Id, string Category, string Description, string Location, double? Latitude, double? Longitude, DateTimeOffset? Moment);
