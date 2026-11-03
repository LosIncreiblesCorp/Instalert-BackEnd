using InstAlertPlatform.API.Profiles.Application.Services;
using InstAlertPlatform.API.Profiles.Domain.Model.Aggregates;
using InstAlertPlatform.API.Profiles.Domain.Model.Commands;
using InstAlertPlatform.API.Profiles.Domain.Model.Queries;
using InstAlertPlatform.API.Profiles.Domain.Repositories;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Profiles.Application.Internal.QueryServices;
public sealed class ProfileQueryService(IUserProfileRepository profiles, IUserPreferencesRepository preferences, ICurrentActor actor) : IProfileQueryService
{
    public async Task<ProfileSnapshot> Handle(GetUserProfileQuery query, CancellationToken cancellationToken = default)
    {
        var userId = query.UserId;
        Guard.Require(userId == actor.UserId, "forbidden");
        var profile = await profiles.FindByIdAsync(userId, cancellationToken);
        return profile is null ? new(userId, "", "", "", null, null, "") : ToProfileSnapshot(profile);
    }

    public async Task<UserPreferencesSnapshot> Handle(GetUserPreferencesQuery query, CancellationToken cancellationToken = default)
    {
        var userId = query.UserId;
        Guard.Require(userId == actor.UserId, "forbidden");
        var preference = await preferences.FindByIdAsync(userId, cancellationToken);
        return preference is null ? new(userId, "en_US", null, "") : new(userId, preference.Locale.Value, preference.UpdatedAt, preference.Version);
    }

    private static ProfileSnapshot ToProfileSnapshot(UserProfile p) => new(p.Id, p.FirstName, p.LastName, p.Dni.Value, p.Phone, p.UpdatedAt, p.Version);
}
