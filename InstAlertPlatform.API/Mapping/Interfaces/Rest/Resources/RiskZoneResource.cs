namespace InstAlertPlatform.API.Mapping.Interfaces.Rest.Resources;
public sealed record RiskZoneResource(string Id, string Name, string? RiskLevel, int IncidentCount, double[][] Bounds, DateTimeOffset? WindowStart = null, DateTimeOffset? WindowEnd = null);
