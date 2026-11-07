namespace InstAlertPlatform.API.Mapping.Interfaces.Rest.Resources;
public sealed record IncidentResource(string Id, string BusinessId, string Title, string Description, string Type, string Status, double Latitude, double Longitude, DateTimeOffset ReportedAt, string Category = "");
