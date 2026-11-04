namespace InstAlertPlatform.API.Business.Domain.Model.Commands;
public sealed record UpdateBusinessCommand(string Id, string? TaxId, string Name, string Address, double? Latitude, double? Longitude);
