namespace InstAlertPlatform.API.Mapping.Domain.Model.Queries;
public sealed record FindMappedIncidentsQuery(string BusinessId, double RadiusMeters, string? Category, DateTimeOffset? Since);
