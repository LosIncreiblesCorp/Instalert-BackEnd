using InstAlertPlatform.API.Alert.Domain.Model.Commands;
using InstAlertPlatform.API.Alert.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Alert.Interfaces.Rest.Transform;
public static class PreparePanicCommandFromResourceAssembler
{
    public static PreparePanicCommand ToCommandFromResource(PreparePanicResource resource) => new(resource.BusinessId, resource.Location, resource.Latitude, resource.Longitude);
}
