using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Payments.Domain.Model.Aggregates;
public sealed partial class Subscription : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
