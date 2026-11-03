using InstAlertPlatform.API.Business.Domain.Model.Commands;
using InstAlertPlatform.API.Business.Domain.Model.ValueObjects;
using InstAlertPlatform.API.Business.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Business.Interfaces.Rest.Transform;

public static class UpdateBusinessProfileCommandFromResourceAssembler
{
    public static UpdateBusinessProfileCommand ToCommandFromResource(UpdateBusinessResource resource, int id, string ownerUserId) =>
        new(id, ownerUserId, resource.TaxId is null ? null : new TaxIdentifier(resource.TaxId), resource.Name,
            resource.Address, resource.Latitude!.Value, resource.Longitude!.Value);
}
