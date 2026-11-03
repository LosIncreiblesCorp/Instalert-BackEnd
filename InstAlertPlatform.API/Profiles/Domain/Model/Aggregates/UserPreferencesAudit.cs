using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Profiles.Domain.Model.Aggregates;

public sealed partial class UserPreferences : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
