using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Iam.Domain.Model.Aggregates;

public sealed partial class User : AuditedEntity
{
    private User() { }
    public User(string email, string passwordHash, string role, bool temporary = false, DateTimeOffset? expiresAt = null)
    {
        Email = Guard.Email(email).ToLowerInvariant();
        Guard.Require(role is "administrator" or "employee", "validation", "role");
        PasswordHash = passwordHash;
        Role = role;
        MustChangePassword = temporary;
        TemporaryPasswordExpiresAt = expiresAt;
    }
    public string Email { get; private set; } = "";
    public string PasswordHash { get; private set; } = "";
    public string Role { get; private set; } = "";
    public bool MustChangePassword { get; private set; }
    public DateTimeOffset? TemporaryPasswordExpiresAt { get; private set; }
    public string SecurityStamp { get; private set; } = Guid.NewGuid().ToString("N");

    public bool HasValidCredentials(DateTimeOffset now) => !MustChangePassword || TemporaryPasswordExpiresAt > now;

    public void SetTemporaryPassword(string hash, DateTimeOffset expiresAt)
    {
        Guard.Require(MustChangePassword, "transition");
        PasswordHash = hash;
        TemporaryPasswordExpiresAt = expiresAt;
        RevokeTokens();
    }
    public void ChangePassword(string hash)
    {
        PasswordHash = hash;
        MustChangePassword = false;
        TemporaryPasswordExpiresAt = null;
        RevokeTokens();
    }
    public void RevokeTokens() => SecurityStamp = Guid.NewGuid().ToString("N");
}
