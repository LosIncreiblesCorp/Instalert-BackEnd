using InstAlertPlatform.API.Business.Domain.Model.Commands;
using InstAlertPlatform.API.Business.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Business.Interfaces.Rest.Transform;
public static class CreateBusinessCommandFromResourceAssembler
{
    public static CreateBusinessCommand ToCommandFromResource(CreateBusinessResource resource) => new(resource.TaxId, resource.Name, resource.Address, resource.Latitude, resource.Longitude);
}
