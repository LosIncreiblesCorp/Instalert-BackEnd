using InstAlertPlatform.API.Profiles.Application.Errors;
using InstAlertPlatform.API.Profiles.Application.Services;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Profiles.Interfaces.Rest.Transform;
public static class ActionResultFromUpdateUserProfileResultAssembler
{
    public static IActionResult ToActionResultFromUpdateUserProfileResult(Result<ProfileSnapshot, EUpdateUserProfileError> result, ControllerBase controller, IStringLocalizer<SharedResource> localizer, Func<ProfileSnapshot, IActionResult> onSuccess) => result switch
    {
        Result<ProfileSnapshot, EUpdateUserProfileError>.Success success => onSuccess(success.Value),
        Result<ProfileSnapshot, EUpdateUserProfileError>.Failure failure => ProblemDetailsAssembler.ToActionResult(controller, failure.Error switch
        {
            EUpdateUserProfileError.InvalidInput => "validation",
            EUpdateUserProfileError.Unauthenticated => "unauthenticated",
            EUpdateUserProfileError.Forbidden => "forbidden",
            EUpdateUserProfileError.NotFound => "not_found",
            EUpdateUserProfileError.Conflict => "conflict",
            EUpdateUserProfileError.ConcurrencyConflict => "concurrency",
            EUpdateUserProfileError.UnexpectedError => "unexpected",
            _ => "unexpected"
        }, failure.Field, localizer),
        _ => ProblemDetailsAssembler.ToActionResult(controller, "unexpected", "", localizer)};
}
