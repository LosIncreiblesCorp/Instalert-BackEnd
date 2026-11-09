using InstAlertPlatform.API.Alert.Application.Services;

namespace InstAlertPlatform.API.Alert.Interfaces.Rest.Resources;
public sealed record AlertResource(string Id, string BusinessId, string EmployeeId, string Kind, string Status, string? ReportStatus, string Category, string Description, string Location, double? Latitude, double? Longitude, DateTimeOffset? Moment, DateTimeOffset CreatedAt, DateTimeOffset? ActivatedAt, DateTimeOffset? ResolvedAt, DateTimeOffset? CompletedAt);
