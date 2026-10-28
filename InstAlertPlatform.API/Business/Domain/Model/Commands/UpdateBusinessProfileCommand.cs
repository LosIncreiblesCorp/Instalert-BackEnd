using InstAlertPlatform.API.Business.Domain.Model.ValueObjects;

namespace InstAlertPlatform.API.Business.Domain.Model.Commands;

public record UpdateBusinessProfileCommand(int Id, string OwnerUserId, TaxIdentifier? TaxId, string Name,
    string Address, double Latitude, double Longitude);
