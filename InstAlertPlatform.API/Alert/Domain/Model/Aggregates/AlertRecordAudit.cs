using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Alert.Domain.Model.Aggregates;
public sealed partial class AlertRecord : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
