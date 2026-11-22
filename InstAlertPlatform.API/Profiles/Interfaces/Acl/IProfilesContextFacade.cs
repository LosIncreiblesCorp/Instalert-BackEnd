namespace InstAlertPlatform.API.Profiles.Interfaces.Acl;
public interface IProfilesContextFacade
{
    Task<bool> IsComplete(string userId, CancellationToken token);
    Task<string?> FindDisplayName(string userId, CancellationToken token);
}
