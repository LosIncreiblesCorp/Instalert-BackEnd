namespace InstAlertPlatform.API.Mapping.Application.Services;
public sealed record RiskZoneSnapshot(string Id, string Name, string? RiskLevel, int IncidentCount, double[][] Bounds, DateTimeOffset? WindowStart = null, DateTimeOffset? WindowEnd = null);
