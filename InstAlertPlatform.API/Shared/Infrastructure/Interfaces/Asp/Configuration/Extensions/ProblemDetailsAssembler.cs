using InstAlertPlatform.API.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
public static class ProblemDetailsAssembler
{
    public static int StatusCode(string code) => code switch
    {
        "unauthenticated" => 401,
        "forbidden" => 403,
        "not_found" => 404,
        "conflict" or "concurrency" or "capacity" or "transition" or "already_completed" => 409,
        "provider_unavailable" or "identity_unavailable" or "policy_unavailable" => 503,
        "unexpected" => 500,
        _ => 400
    };
    public static ProblemDetails ToProblemDetails(string code, string field, HttpContext context, IStringLocalizer<SharedResource> localizer)
    {
        var status = StatusCode(code);
        var problem = new ProblemDetails
        {
            Status = status,
            Title = localizer["RequestFailedTitle"].Value,
            Detail = localizer[$"ErrorDetail{status}"].Value,
            Instance = context.Request.Path
        };
        problem.Extensions["code"] = code;
        problem.Extensions["traceId"] = context.TraceIdentifier;
        if (!string.IsNullOrEmpty(field))
            problem.Extensions["field"] = field;
        return problem;
    }

    public static IActionResult ToActionResult(ControllerBase controller, string code, string field, IStringLocalizer<SharedResource> localizer)
    {
        var problem = ToProblemDetails(code, field, controller.HttpContext, localizer);
        var result = new ObjectResult(problem)
        {
            StatusCode = problem.Status
        };
        result.ContentTypes.Add("application/problem+json");
        return result;
    }
}
