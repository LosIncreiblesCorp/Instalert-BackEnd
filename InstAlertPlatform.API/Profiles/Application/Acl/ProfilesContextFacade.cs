using InstAlertPlatform.API.Profiles.Domain.Repositories;
using InstAlertPlatform.API.Profiles.Interfaces.Acl;

namespace InstAlertPlatform.API.Profiles.Application.Acl;
public sealed class ProfilesContextFacade(IUserProfileRepository profiles) : IProfilesContextFacade
{
    public async Task<bool> IsComplete(string userId, CancellationToken token)
    {
        var profile = await profiles.FindByIdAsync(userId, token);
        return profile is not null && !string.IsNullOrWhiteSpace(profile.FirstName)
            && !string.IsNullOrWhiteSpace(profile.LastName) && profile.Dni.Value.Length == 8;
    }

    public async Task<string?> FindDisplayName(string userId, CancellationToken token)
    {
        var profile = await profiles.FindByIdAsync(userId, token);
        if (profile is null) return null;
        var name = (profile.FirstName + " " + profile.LastName).Trim();
        return string.IsNullOrWhiteSpace(name) ? null : name;
    }
}
