using InstAlertPlatform.API.Business.Domain.Model.Commands;
using InstAlertPlatform.API.Business.Domain.Model.ValueObjects;
using InstAlertPlatform.API.Business.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Business.Interfaces.Rest.Transform;

public static class CreateBusinessProfileCommandFromResourceAssembler
{
    public static CreateBusinessProfileCommand ToCommandFromResource(CreateBusinessResource resource, string ownerUserId) =>
        new(ownerUserId, new TaxIdentifier(resource.TaxId), resource.Name, resource.Address,
            resource.Latitude!.Value, resource.Longitude!.Value);
}
