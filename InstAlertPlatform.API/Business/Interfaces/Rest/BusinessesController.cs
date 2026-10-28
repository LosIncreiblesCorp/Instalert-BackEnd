using System.Net.Mime;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Business.Interfaces.Rest.Resources;
using InstAlertPlatform.API.Business.Interfaces.Rest.Transform;
using InstAlertPlatform.API.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Business.Interfaces.Rest;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Businesses")]
public class BusinessesController(IBusinessProfileCommandService commands, IBusinessProfileQueryService queries,
    IStringLocalizer<SharedResource> localizer, IConfiguration configuration, IHostEnvironment environment,
    ILogger<BusinessesController> logger) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BusinessResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetBusinesses([FromHeader(Name = "User-Id")] string? userId,
        CancellationToken cancellationToken)
    {
        var denied = CheckAdministrator(userId, out var actorId);
        if (denied is not null) return denied;
        var profiles = await queries.Handle(new GetBusinessProfilesByOwnerUserIdQuery(actorId), cancellationToken);
        return Ok(profiles.Select(BusinessResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(BusinessResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetBusinessById(int id, [FromHeader(Name = "User-Id")] string? userId,
        CancellationToken cancellationToken)
    {
        var denied = CheckAdministrator(userId, out var actorId);
        if (denied is not null) return denied;
        var profile = await queries.Handle(new GetBusinessProfileByIdQuery(id, actorId), cancellationToken);
        return profile is null ? ToProblem(404, "not_found", "BusinessNotFound")
            : Ok(BusinessResourceFromEntityAssembler.ToResourceFromEntity(profile));
    }

    [HttpPost]
    [ProducesResponseType(typeof(BusinessResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CreateBusiness([FromBody] CreateBusinessResource resource,
        [FromHeader(Name = "User-Id")] string? userId, CancellationToken cancellationToken)
    {
        var denied = CheckAdministrator(userId, out var actorId);
        if (denied is not null) return denied;
        try
        {
            var command = CreateBusinessProfileCommandFromResourceAssembler.ToCommandFromResource(resource, actorId);
            var result = await commands.Handle(command, cancellationToken);
            return ActionResultFromCreateBusinessProfileResultAssembler.ToActionResultFromCreateBusinessProfileResult(
                result, this, localizer, nameof(GetBusinessById));
        }
        catch (ArgumentException)
        {
            logger.LogWarning("Invalid business registration request.");
            return ToProblem(400, "validation", "BusinessDetailsInvalid");
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(BusinessResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateBusiness(int id, [FromBody] UpdateBusinessResource resource,
        [FromHeader(Name = "User-Id")] string? userId, CancellationToken cancellationToken)
    {
        var denied = CheckAdministrator(userId, out var actorId);
        if (denied is not null) return denied;
        try
        {
            var command = UpdateBusinessProfileCommandFromResourceAssembler.ToCommandFromResource(resource, id, actorId);
            var result = await commands.Handle(command, cancellationToken);
            return ActionResultFromUpdateBusinessProfileResultAssembler.ToActionResultFromUpdateBusinessProfileResult(result, this, localizer);
        }
        catch (ArgumentException)
        {
            logger.LogWarning("Invalid business update request.");
            return ToProblem(400, "validation", "BusinessDetailsInvalid");
        }
    }

    private ActionResult? CheckAdministrator(string? userId, out string actorId)
    {
        actorId = "";
        var knownUsers = configuration.GetSection("DevelopmentActors:UserIds").Get<string[]>() ?? [];
        if (!environment.IsDevelopment() || !configuration.GetValue<bool>("DevelopmentActors:Enabled") ||
            string.IsNullOrWhiteSpace(userId) || !knownUsers.Contains(userId, StringComparer.Ordinal))
            return ToProblem(401, "unauthenticated", "BusinessUnauthenticated");

        var administrators = configuration.GetSection("DevelopmentActors:AdministratorUserIds").Get<string[]>() ?? [];
        if (!administrators.Contains(userId, StringComparer.Ordinal))
            return ToProblem(403, "forbidden", "BusinessForbidden");

        actorId = userId;
        return null;
    }

    private ObjectResult ToProblem(int status, string code, string message)
    {
        var result = Problem(statusCode: status, title: localizer["BusinessRequestFailed"].Value,
            detail: localizer[message].Value);
        if (result.Value is ProblemDetails problem) problem.Extensions["code"] = code;
        return result;
    }
}
