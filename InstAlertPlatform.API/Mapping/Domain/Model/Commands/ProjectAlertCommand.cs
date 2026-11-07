namespace InstAlertPlatform.API.Mapping.Domain.Model.Commands;
public sealed record ProjectAlertCommand(string Id, string BusinessId, string Kind, string Status, string? ReportStatus, string Category, string Description, double? Latitude, double? Longitude, DateTimeOffset CreatedAt, int Revision);
