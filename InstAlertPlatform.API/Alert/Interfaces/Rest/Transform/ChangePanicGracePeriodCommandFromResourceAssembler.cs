using InstAlertPlatform.API.Alert.Domain.Model.Commands;
using InstAlertPlatform.API.Alert.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Alert.Interfaces.Rest.Transform;
public static class ChangePanicGracePeriodCommandFromResourceAssembler
{
    public static ChangePanicGracePeriodCommand ToCommandFromResource(ChangePanicGracePeriodResource resource, string userId) => new(userId, resource.PanicGracePeriodSeconds);
}
