using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Mapping.Domain.Model.Aggregates;
public sealed partial class IncidentProjection : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
