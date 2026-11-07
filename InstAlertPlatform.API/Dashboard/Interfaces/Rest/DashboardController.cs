using InstAlertPlatform.API.Dashboard.Application.Services;
using InstAlertPlatform.API.Dashboard.Domain.Model.Queries;
using InstAlertPlatform.API.Dashboard.Interfaces.Rest.Resources;
using InstAlertPlatform.API.Dashboard.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace InstAlertPlatform.API.Dashboard.Interfaces.Rest;
[ApiController, Route("api/v1/businesses/{businessId}"), Tags("Dashboard")]
[ProducesResponseType(typeof(ProblemDetails), 400), ProducesResponseType(typeof(ProblemDetails), 401), ProducesResponseType(typeof(ProblemDetails), 403), ProducesResponseType(typeof(ProblemDetails), 404), ProducesResponseType(typeof(ProblemDetails), 409), ProducesResponseType(typeof(ProblemDetails), 503)]
[ProducesResponseType(typeof(ProblemDetails), 500)]
public sealed class DashboardController(IDashboardQueryService dashboardQueries) : ControllerBase
{
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(DashboardResource), 200)]
    public async Task<IActionResult> Current(string businessId, CancellationToken cancellationToken, int radiusMeters = 500)
    {
        var snapshot = await dashboardQueries.Handle(new GetCurrentDashboardQuery(businessId, radiusMeters), cancellationToken);
        return Ok(DashboardResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot));
    }

}
