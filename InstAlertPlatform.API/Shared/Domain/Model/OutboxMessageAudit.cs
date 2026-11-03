using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Shared.Domain.Model;
public sealed partial class OutboxMessage : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
