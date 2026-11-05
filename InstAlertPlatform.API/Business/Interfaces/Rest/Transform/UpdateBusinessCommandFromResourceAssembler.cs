using InstAlertPlatform.API.Business.Domain.Model.Commands;
using InstAlertPlatform.API.Business.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Business.Interfaces.Rest.Transform;
public static class UpdateBusinessCommandFromResourceAssembler
{
    public static UpdateBusinessCommand ToCommandFromResource(UpdateBusinessResource resource, string id) => new(id, resource.TaxId, resource.Name, resource.Address, resource.Latitude, resource.Longitude);
}
