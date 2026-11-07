namespace InstAlertPlatform.API.Mapping.Domain.Model.Queries;
public sealed record GetRiskZonesQuery(string BusinessId, double RadiusMeters, string? Category, DateTimeOffset? Since);
