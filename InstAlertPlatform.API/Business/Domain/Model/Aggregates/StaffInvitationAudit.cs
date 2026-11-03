using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Business.Domain.Model.Aggregates;
public sealed partial class StaffInvitation : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
