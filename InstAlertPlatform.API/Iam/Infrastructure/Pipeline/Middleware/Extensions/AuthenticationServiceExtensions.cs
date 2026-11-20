using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using System.Text;
using InstAlertPlatform.API.Iam.Application.CommandServices;
using InstAlertPlatform.API.Iam.Application.QueryServices;
using InstAlertPlatform.API.Iam.Application.Internal.CommandServices;
using InstAlertPlatform.API.Iam.Application.Internal.QueryServices;
using InstAlertPlatform.API.Iam.Application.Internal.OutboundServices;
using InstAlertPlatform.API.Iam.Domain.Repositories;
using InstAlertPlatform.API.Iam.Infrastructure.Hashing.BCrypt.Services;
using InstAlertPlatform.API.Iam.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using InstAlertPlatform.API.Iam.Infrastructure.Tokens.Jwt.Configuration;
using InstAlertPlatform.API.Iam.Infrastructure.Tokens.Jwt.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace InstAlertPlatform.API.Iam.Infrastructure.Pipeline.Middleware.Extensions;

public static class AuthenticationServiceExtensions
{
    public static IServiceCollection AddIdentityAuthentication(this IServiceCollection services)
    {
        services.AddOptions<TokenSettings>().BindConfiguration("TokenSettings")
            .Validate(s => Encoding.UTF8.GetByteCount(s.Secret) >= 32, "Configure a signing key containing at least 32 bytes.")
            .Validate(s => !string.IsNullOrWhiteSpace(s.Issuer) && !string.IsNullOrWhiteSpace(s.Audience), "Configure token issuer and audience.")
            .Validate(s => s.AccessTokenMinutes is >= 5 and <= 10080, "Access token duration must be between 5 minutes and 7 days.")
            .ValidateOnStart();
        services.AddScoped<InstAlertPlatform.API.Iam.Interfaces.Acl.IIamContextFacade, InstAlertPlatform.API.Iam.Application.Acl.IamContextFacade>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IHashingService, HashingService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserCommandService, UserCommandService>();
        services.AddScoped<IUserQueryService, UserQueryService>();
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy("identity", context => RateLimitPartition.GetFixedWindowLimiter(context.Connection.RemoteIpAddress?.ToString() ?? "local", _ => new FixedWindowRateLimiterOptions
            { PermitLimit = 30, Window = TimeSpan.FromMinutes(1), QueueLimit = 0, AutoReplenishment = true }));
            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = 429;
                context.HttpContext.Response.Headers.RetryAfter = "60";
                var messages = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<SharedResource>>();
                await context.HttpContext.Response.WriteAsJsonAsync(ProblemDetailsAssembler.ToProblemDetails("rate_limited", "", context.HttpContext, messages), (System.Text.Json.JsonSerializerOptions?)null, "application/problem+json", token);
            };
        });
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<TokenSettings>>((options, configured) =>
            {
                var settings = configured.Value;
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret)),
                    ValidateIssuer = true,
                    ValidIssuer = settings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = settings.Audience,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    RequireSignedTokens = true,
                    ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                    NameClaimType = "sub",
                    RoleClaimType = "role",
                    ClockSkew = TimeSpan.FromSeconds(15)
                };
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = 401;
                        context.Response.Headers.WWWAuthenticate = "Bearer";
                        var messages = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<SharedResource>>();
                        var problem = ProblemDetailsAssembler.ToProblemDetails("unauthenticated", "", context.HttpContext, messages);
                        await context.Response.WriteAsJsonAsync(problem, (System.Text.Json.JsonSerializerOptions?)null, "application/problem+json", context.HttpContext.RequestAborted);
                    },
                    OnForbidden = async context =>
                    {
                        context.Response.StatusCode = 403;
                        var messages = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<SharedResource>>();
                        await context.Response.WriteAsJsonAsync(ProblemDetailsAssembler.ToProblemDetails("forbidden", "", context.HttpContext, messages), (System.Text.Json.JsonSerializerOptions?)null, "application/problem+json", context.HttpContext.RequestAborted);
                    },
                    OnTokenValidated = async context =>
                    {
                        var id = context.Principal?.FindFirst("sub")?.Value;
                        var repository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
                        var user = id is null ? null : await repository.FindByIdAsync(id, context.HttpContext.RequestAborted);
                        var clock = context.HttpContext.RequestServices.GetRequiredService<TimeProvider>();
                        if (user is null || !user.HasValidCredentials(clock.GetUtcNow()) || user.SecurityStamp != context.Principal?.FindFirst("security_stamp")?.Value
                            || user.Role != context.Principal?.FindFirst("role")?.Value)
                            context.Fail("The session is no longer valid.");
                    }
                };
            });
        services.AddAuthorization();
        return services;
    }
}
