using InstAlertPlatform.API.Shared.Domain.Model;
namespace InstAlertPlatform.API.Iam.Domain.Model.Aggregates;
public sealed partial class User : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
