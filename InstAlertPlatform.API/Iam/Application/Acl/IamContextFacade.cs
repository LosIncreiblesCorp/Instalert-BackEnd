using System.Security.Cryptography;
using InstAlertPlatform.API.Profiles.Application.Services;
using System.Text.Json;
using InstAlertPlatform.API.Iam.Application.Internal.OutboundServices;
using InstAlertPlatform.API.Iam.Domain.Model.Aggregates;
using InstAlertPlatform.API.Iam.Domain.Repositories;
using InstAlertPlatform.API.Profiles.Domain.Model.Queries;
using InstAlertPlatform.API.Shared.Domain.Model;
using Microsoft.AspNetCore.DataProtection;

using InstAlertPlatform.API.Iam.Interfaces.Acl;
namespace InstAlertPlatform.API.Iam.Application.Acl;
public sealed class IamContextFacade(IUserRepository users, IHashingService hashing, IDataProtectionProvider protection,
    IProfileQueryService profiles, TimeProvider clock) : IIamContextFacade
{
    private readonly IDataProtector protector = protection.CreateProtector("InstAlert.InvitationCredential.v1");
    public async Task<string?> FindUserIdByEmail(string email, CancellationToken token) => (await users.FindByEmailAsync(Guard.Email(email).ToLowerInvariant(), token))?.Id;

    public async Task<string?> PrepareInvitationCredential(string email, CancellationToken token)
    {
        email = Guard.Email(email).ToLowerInvariant();
        var user = await users.FindByEmailAsync(email, token);
        Guard.Require(user is null || user.Role == "employee", "conflict", "email");
        if (user is not null && !user.MustChangePassword) return null;
        var password = "T7!" + Convert.ToBase64String(RandomNumberGenerator.GetBytes(24));
        var expiresAt = clock.GetUtcNow().AddHours(24);
        if (user is null)
        {
            user = new User(email, hashing.HashPassword(password), "employee", true, expiresAt);
            await users.AddAsync(user, token);
        }
        else user.SetTemporaryPassword(hashing.HashPassword(password), expiresAt);
        return protector.Protect(JsonSerializer.Serialize(new Credential(user.Id, user.Email, user.SecurityStamp, password, expiresAt)));
    }
    public async Task<string?> ReadInvitationCredential(string encryptedCredential, string email, CancellationToken token)
    {
        Credential? credential;
        try { credential = JsonSerializer.Deserialize<Credential>(protector.Unprotect(encryptedCredential)); }
        catch (Exception error) when (error is CryptographicException or JsonException) { return null; }
        if (credential is null || credential.Email != email.ToLowerInvariant() || credential.ExpiresAt <= clock.GetUtcNow()) return null;
        var user = await users.FindByIdAsync(credential.UserId, token);
        return user?.MustChangePassword == true && user.SecurityStamp == credential.SecurityStamp ? credential.Password : null;
    }
    public async Task RequireCompletedEmployee(string userId, CancellationToken token)
    {
        var user = await users.FindByIdAsync(userId, token) ?? throw new DomainException("unauthenticated");
        Guard.Require(user.Role == "employee", "forbidden");
        Guard.Require(!user.MustChangePassword, "transition");
        var profile = await profiles.Handle(new GetUserProfileQuery(userId), token);
        Guard.Require(!string.IsNullOrWhiteSpace(profile.FirstName) && !string.IsNullOrWhiteSpace(profile.LastName) && profile.Dni.Length == 8, "transition");
    }
    private sealed record Credential(string UserId, string Email, string SecurityStamp, string Password, DateTimeOffset ExpiresAt);
}
