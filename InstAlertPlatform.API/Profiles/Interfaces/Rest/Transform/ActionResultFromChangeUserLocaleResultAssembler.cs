using InstAlertPlatform.API.Profiles.Application.Errors;
using InstAlertPlatform.API.Profiles.Application.Services;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Profiles.Interfaces.Rest.Transform;
public static class ActionResultFromChangeUserLocaleResultAssembler
{
    public static IActionResult ToActionResultFromChangeUserLocaleResult(Result<UserPreferencesSnapshot, EChangeUserLocaleError> result, ControllerBase controller, IStringLocalizer<SharedResource> localizer, Func<UserPreferencesSnapshot, IActionResult> onSuccess) => result switch
    {
        Result<UserPreferencesSnapshot, EChangeUserLocaleError>.Success success => onSuccess(success.Value),
        Result<UserPreferencesSnapshot, EChangeUserLocaleError>.Failure failure => ProblemDetailsAssembler.ToActionResult(controller, failure.Error switch
        {
            EChangeUserLocaleError.InvalidInput => "validation",
            EChangeUserLocaleError.Unauthenticated => "unauthenticated",
            EChangeUserLocaleError.Forbidden => "forbidden",
            EChangeUserLocaleError.NotFound => "not_found",
            EChangeUserLocaleError.Conflict => "conflict",
            EChangeUserLocaleError.ConcurrencyConflict => "concurrency",
            EChangeUserLocaleError.UnexpectedError => "unexpected",
            _ => "unexpected"
        }, failure.Field, localizer),
        _ => ProblemDetailsAssembler.ToActionResult(controller, "unexpected", "", localizer)};
}
