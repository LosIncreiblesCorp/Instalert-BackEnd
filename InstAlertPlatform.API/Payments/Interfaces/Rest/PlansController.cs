using InstAlertPlatform.API.Payments.Application.Services;
using InstAlertPlatform.API.Payments.Domain.Model.Commands;
using InstAlertPlatform.API.Payments.Domain.Model.Queries;
using InstAlertPlatform.API.Payments.Interfaces.Rest.Resources;
using InstAlertPlatform.API.Payments.Interfaces.Rest.Transform;
using InstAlertPlatform.API.Shared.Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace InstAlertPlatform.API.Payments.Interfaces.Rest;
[ApiController, Route("api/v1/[controller]"), Tags("Payments")]
[ProducesResponseType(typeof(ProblemDetails), 400), ProducesResponseType(typeof(ProblemDetails), 401), ProducesResponseType(typeof(ProblemDetails), 403), ProducesResponseType(typeof(ProblemDetails), 404), ProducesResponseType(typeof(ProblemDetails), 409), ProducesResponseType(typeof(ProblemDetails), 503)]
[ProducesResponseType(typeof(ProblemDetails), 500)]
public sealed class PlansController(IPlanQueryService planQueries) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<PlanResource>), 200)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var snapshot = await planQueries.Handle(new GetPlansQuery(), cancellationToken);
        return Ok(snapshot.Select(PlanResourceFromSnapshotAssembler.ToResourceFromSnapshot).ToList());
    }
}
