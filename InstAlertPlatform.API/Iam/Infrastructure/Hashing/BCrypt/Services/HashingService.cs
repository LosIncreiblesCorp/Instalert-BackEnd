using InstAlertPlatform.API.Iam.Application.Internal.OutboundServices;

namespace InstAlertPlatform.API.Iam.Infrastructure.Hashing.BCrypt.Services;

public sealed class HashingService : IHashingService
{
    public string HashPassword(string password) => global::BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    public bool VerifyPassword(string password, string hash)
    {
        try { return global::BCrypt.Net.BCrypt.Verify(password, hash); }
        catch (global::BCrypt.Net.SaltParseException) { return false; }
    }
}
