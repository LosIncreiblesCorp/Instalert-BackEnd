using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Contacts.Domain.Model.Aggregates;
public sealed partial class EmergencyContact : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
