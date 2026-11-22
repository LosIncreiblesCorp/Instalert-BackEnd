namespace InstAlertPlatform.API.Iam.Interfaces.Acl;
public interface IIamContextFacade
{
    Task<bool> HasCompletedAccount(string userId, CancellationToken token);
    Task<string?> FindUserIdByEmail(string email, CancellationToken token);
    Task<string?> PrepareInvitationCredential(string email, CancellationToken token);
    Task<string?> ReadInvitationCredential(string encryptedCredential, string email, CancellationToken token);
    Task RequireCompletedEmployee(string userId, CancellationToken token);
}
