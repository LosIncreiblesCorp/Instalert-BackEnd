using InstAlertPlatform.API.Profiles.Domain.Model.Aggregates;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Profiles.Domain.Repositories;
public interface IUserProfileRepository : IBaseRepository<UserProfile>
{
    Task<UserProfile?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
}
