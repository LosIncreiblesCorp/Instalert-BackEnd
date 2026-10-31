using System.Security.Claims;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp;
public sealed class HttpCurrentActor(IHttpContextAccessor accessor, IWebHostEnvironment environment, IConfiguration configuration) : ICurrentActor
{
    public bool IsAdministrator
    {
        get
        {
            var userId = UserId;
            var principal = accessor.HttpContext?.User;
            if (principal?.Identity?.IsAuthenticated == true)
                return principal.IsInRole("Administrator");
            return environment.IsDevelopment() && configuration.GetValue<bool>("DevelopmentActors:Enabled") &&
                (configuration.GetSection("DevelopmentActors:AdministratorUserIds").Get<string[]>() ?? []).Contains(userId, StringComparer.Ordinal);
        }
    }

    public string? VerifiedEmail
    {
        get
        {
            var user = accessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated == true && user.FindFirstValue("email_verified") == "true")
                return user.FindFirstValue(ClaimTypes.Email) ?? user.FindFirstValue("email");
            if (environment.IsDevelopment() && configuration.GetValue<bool>("DevelopmentActors:Enabled"))
                return configuration["DevelopmentActors:Emails:" + UserId];
            return null;
        }
    }

    public string UserId
    {
        get
        {
            var context = accessor.HttpContext;
            var principal = context?.User;
            if (principal?.Identity?.IsAuthenticated == true)
                return principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? principal.FindFirstValue("sub") ?? throw new DomainException("unauthenticated");
            if (environment.IsDevelopment() && configuration.GetValue<bool>("DevelopmentActors:Enabled"))
            {
                var id = context?.Request.Headers["User-Id"].ToString();
                var allowed = configuration.GetSection("DevelopmentActors:UserIds").Get<string[]>() ?? [];
                if (!string.IsNullOrEmpty(id) && allowed.Contains(id, StringComparer.Ordinal))
                    return id;
            }

            throw new DomainException("unauthenticated");
        }
    }
}
