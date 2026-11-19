using Microsoft.AspNetCore.RateLimiting;
using InstAlertPlatform.API.Resources;
using Microsoft.Extensions.Localization;
using InstAlertPlatform.API.Iam.Application.CommandServices;
using InstAlertPlatform.API.Iam.Domain.Model.Commands;
using InstAlertPlatform.API.Iam.Interfaces.Rest.Resources;
using InstAlertPlatform.API.Iam.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstAlertPlatform.API.Iam.Interfaces.Rest;

[Authorize]
[ApiController, Route("api/v1/tokens"), Tags("Identity")]
public sealed class TokensController(IUserCommandService users, IStringLocalizer<SharedResource> messages) : ControllerBase
{
    [HttpPost, AllowAnonymous, EnableRateLimiting("identity")]
    [ProducesResponseType(429)]
    [ProducesResponseType(200), ProducesResponseType(400), ProducesResponseType(401)]
    public async Task<IActionResult> SignIn(SignInResource resource, CancellationToken cancellationToken) =>
        AuthenticationResultAssembler.ToActionResult(await users.Handle(new SignInCommand(resource.Email, resource.Password), cancellationToken), this, messages);

    [HttpDelete("current")]
    [ProducesResponseType(204), ProducesResponseType(401)]
    public async Task<IActionResult> SignOut(CancellationToken cancellationToken)
    {
        await users.RevokeCurrentTokens(cancellationToken);
        return NoContent();
    }
}
