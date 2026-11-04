using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Business.Domain.Model.Aggregates;
public sealed partial class BusinessProfile : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
