namespace InstAlertPlatform.API.Business.Domain.Model.Commands;
public sealed record CreateBusinessCommand(string TaxId, string Name, string Address, double? Latitude, double? Longitude);
