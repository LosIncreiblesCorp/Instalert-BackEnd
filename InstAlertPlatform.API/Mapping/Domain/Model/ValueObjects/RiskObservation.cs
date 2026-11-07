namespace InstAlertPlatform.API.Mapping.Domain.Model.ValueObjects;
public sealed record RiskObservation(string Id, double Latitude, double Longitude, DateTimeOffset RegisteredAt);
