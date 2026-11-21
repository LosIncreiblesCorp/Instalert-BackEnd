namespace InstAlertPlatform.API.Profiles.Interfaces.Acl;
public interface IProfilesContextFacade
{
    Task<string?> FindDisplayName(string userId, CancellationToken token);
}
