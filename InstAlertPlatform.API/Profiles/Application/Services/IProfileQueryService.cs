using InstAlertPlatform.API.Profiles.Application.Services;
using InstAlertPlatform.API.Profiles.Domain.Model.Queries;

namespace InstAlertPlatform.API.Profiles.Application.Services;
public interface IProfileQueryService
{
    Task<ProfileSnapshot> Handle(GetUserProfileQuery query, CancellationToken cancellationToken = default);
    Task<UserPreferencesSnapshot> Handle(GetUserPreferencesQuery query, CancellationToken cancellationToken = default);
}
