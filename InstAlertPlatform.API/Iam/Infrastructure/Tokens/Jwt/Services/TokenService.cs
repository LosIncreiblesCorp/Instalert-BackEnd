using System.Security.Claims;
using System.Text;
using InstAlertPlatform.API.Iam.Application.Internal.OutboundServices;
using InstAlertPlatform.API.Iam.Application.Services;
using InstAlertPlatform.API.Iam.Domain.Model.Aggregates;
using InstAlertPlatform.API.Iam.Infrastructure.Tokens.Jwt.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace InstAlertPlatform.API.Iam.Infrastructure.Tokens.Jwt.Services;

public sealed class TokenService(IOptions<TokenSettings> options, TimeProvider clock) : ITokenService
{
    public AuthenticationSnapshot Create(User user)
    {
        var settings = options.Value;
        var now = clock.GetUtcNow();
        var expires = now.AddMinutes(settings.AccessTokenMinutes);
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim("sub", user.Id),
                new Claim("email", user.Email),
                new Claim("email_verified", user.Role == "employee" ? "true" : "false"),
                new Claim("role", user.Role),
                new Claim("password_change_required", user.MustChangePassword ? "true" : "false"),
                new Claim("security_stamp", user.SecurityStamp),
                new Claim("jti", Guid.NewGuid().ToString("N"))
            ]),
            Issuer = settings.Issuer,
            Audience = settings.Audience,
            IssuedAt = now.UtcDateTime,
            NotBefore = now.UtcDateTime,
            Expires = expires.UtcDateTime,
            SigningCredentials = new(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret)), SecurityAlgorithms.HmacSha256)
        };
        return new(new JsonWebTokenHandler().CreateToken(descriptor), expires, user.Id, user.Email, user.Role, user.MustChangePassword);
    }
}
