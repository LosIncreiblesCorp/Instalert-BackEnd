using InstAlertPlatform.API.Alert.Domain.Model.Commands;
using InstAlertPlatform.API.Alert.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Alert.Interfaces.Rest.Transform;
public static class ConfirmPanicCommandFromResourceAssembler
{
    public static ConfirmPanicCommand ToCommandFromResource(ConfirmPanicResource resource) => new(resource.PreparationToken, resource.Location, resource.Latitude, resource.Longitude);
}
