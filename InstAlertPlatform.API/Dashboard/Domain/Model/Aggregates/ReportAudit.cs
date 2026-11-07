using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Dashboard.Domain.Model.Aggregates;
public sealed partial class Report : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
