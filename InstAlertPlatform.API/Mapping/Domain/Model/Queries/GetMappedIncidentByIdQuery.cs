namespace InstAlertPlatform.API.Mapping.Domain.Model.Queries;
public sealed record GetMappedIncidentByIdQuery(string BusinessId, string Id, double RadiusMeters);
