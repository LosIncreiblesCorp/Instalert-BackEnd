using InstAlertPlatform.API.Iam.Application.Services.Snapshots;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Iam.Application.Errors;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Iam.Interfaces.Rest.Transform;
public static class AuthenticationResultAssembler
{
    public static IActionResult ToActionResult(Result<AuthenticationSnapshot, EIdentityError> result,
        ControllerBase controller, IStringLocalizer<SharedResource> messages, bool created = false)
    {
        if (result is Result<AuthenticationSnapshot, EIdentityError>.Success success)
            return created ? controller.StatusCode(201, success.Value) : controller.Ok(success.Value);
        var failure = (Result<AuthenticationSnapshot, EIdentityError>.Failure)result;
        var duplicate = failure.Error == EIdentityError.EmailRegistered;
        var problem = ProblemDetailsAssembler.ToProblemDetails(duplicate ? "conflict" : "unauthenticated", "", controller.HttpContext, messages);
        problem.Extensions["code"] = duplicate ? "email_registered" : "invalid_credentials";
        return new ObjectResult(problem) { StatusCode = problem.Status, ContentTypes = { "application/problem+json" } };
    }
}
