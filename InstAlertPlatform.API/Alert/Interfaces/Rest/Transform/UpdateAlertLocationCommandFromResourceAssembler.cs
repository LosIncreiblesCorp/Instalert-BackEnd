using InstAlertPlatform.API.Alert.Domain.Model.Commands;
using InstAlertPlatform.API.Alert.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Alert.Interfaces.Rest.Transform;
public static class UpdateAlertLocationCommandFromResourceAssembler
{
    public static UpdateAlertLocationCommand ToCommandFromResource(UpdateAlertLocationResource resource, string id) => new(id, resource.Location, resource.Latitude, resource.Longitude);
}
