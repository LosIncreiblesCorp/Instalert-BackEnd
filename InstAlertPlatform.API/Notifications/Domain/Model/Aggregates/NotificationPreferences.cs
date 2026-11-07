using InstAlertPlatform.API.Notifications.Domain.Model.ValueObjects;
using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Notifications.Domain.Model.Aggregates;
public sealed partial class NotificationPreferences : AuditedEntity
{
    private NotificationPreferences()
    {
    }

    public NotificationPreferences(string userId, string businessId)
    {
        UserId = userId;
        BusinessId = businessId;
        Id = businessId + ":" + userId;
    }

    public string UserId { get; private set; } = "";
    public string BusinessId { get; private set; } = "";
    public bool NearbyAlertsEnabled { get; private set; } = true;
    public ReceptionRadius NearbyRadiusMeters { get; private set; } = new(500);

    public void Update(bool enabled, int radius)
    {
        var receptionRadius = new ReceptionRadius(radius);
        NearbyAlertsEnabled = enabled;
        NearbyRadiusMeters = receptionRadius;
    }
}
