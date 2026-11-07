using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Notifications.Domain.Model.Aggregates;
public sealed partial class ReceivedNotification : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
