using Microsoft.AspNetCore.Authorization;
using InstAlertPlatform.API.Iam.Application.QueryServices;
using InstAlertPlatform.API.Iam.Domain.Model.Queries;
using Microsoft.AspNetCore.Mvc;
namespace InstAlertPlatform.API.Iam.Interfaces.Rest;
[Authorize]
[ApiController, Route("api/v1/sessions/current"), Tags("Identity")]
public sealed class SessionsController(IUserQueryService users) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(200), ProducesResponseType(401)]
    public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken) =>
        Ok(await users.Handle(new GetCurrentUserQuery(), cancellationToken));
}
