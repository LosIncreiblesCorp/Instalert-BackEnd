namespace InstAlertPlatform.API.Mapping.Domain.Model.Queries;
public sealed record GetRiskZoneClassificationsQuery(DateTimeOffset AsOf, bool Historical);
