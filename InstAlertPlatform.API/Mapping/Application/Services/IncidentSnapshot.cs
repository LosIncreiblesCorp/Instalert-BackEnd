namespace InstAlertPlatform.API.Mapping.Application.Services;
public sealed record IncidentSnapshot(string Id, string BusinessId, string Title, string Description, string Type, string Status, double Latitude, double Longitude, DateTimeOffset ReportedAt, string Category = "");
