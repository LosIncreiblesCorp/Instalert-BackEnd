using InstAlertPlatform.API.Notifications.Domain.Model.Commands;
using InstAlertPlatform.API.Notifications.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Notifications.Interfaces.Rest.Transform;
public static class ChangeNotificationPreferencesCommandFromResourceAssembler
{
    public static ChangeNotificationPreferencesCommand ToCommandFromResource(ChangeNotificationPreferencesResource resource, string userId, string businessId) => new(userId, businessId, resource.NearbyAlertsEnabled, resource.NearbyRadiusMeters);
}
