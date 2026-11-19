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
[ApiController, Route("api/v1/users"), Tags("Identity")]
public sealed class UsersController(IUserCommandService users, IStringLocalizer<SharedResource> messages) : ControllerBase
{
    [HttpPost, AllowAnonymous, EnableRateLimiting("identity")]
    [ProducesResponseType(429)]
    [ProducesResponseType(201), ProducesResponseType(400), ProducesResponseType(409)]
    public async Task<IActionResult> Register(RegisterAdministratorResource resource, CancellationToken cancellationToken) =>
        AuthenticationResultAssembler.ToActionResult(await users.Handle(new RegisterAdministratorCommand(resource.Email, resource.Password), cancellationToken), this, messages, true);

    [HttpPut("me/password")]
    [ProducesResponseType(200), ProducesResponseType(400), ProducesResponseType(401)]
    public async Task<IActionResult> ChangePassword(ChangePasswordResource resource, CancellationToken cancellationToken) =>
        AuthenticationResultAssembler.ToActionResult(await users.Handle(new ChangePasswordCommand(resource.CurrentPassword, resource.NewPassword), cancellationToken), this, messages);
}
