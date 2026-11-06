using InstAlertPlatform.API.Alert.Application.Errors;
using InstAlertPlatform.API.Alert.Application.Services;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Alert.Interfaces.Rest.Transform;
public static class ActionResultFromChangePanicGracePeriodResultAssembler
{
    public static IActionResult ToActionResultFromChangePanicGracePeriodResult(Result<AlertPreferencesSnapshot, EChangePanicGracePeriodError> result, ControllerBase controller, IStringLocalizer<SharedResource> localizer, Func<AlertPreferencesSnapshot, IActionResult> onSuccess) => result switch
    {
        Result<AlertPreferencesSnapshot, EChangePanicGracePeriodError>.Success success => onSuccess(success.Value),
        Result<AlertPreferencesSnapshot, EChangePanicGracePeriodError>.Failure failure => ProblemDetailsAssembler.ToActionResult(controller, failure.Error switch
        {
            EChangePanicGracePeriodError.InvalidInput => "validation",
            EChangePanicGracePeriodError.Unauthenticated => "unauthenticated",
            EChangePanicGracePeriodError.Forbidden => "forbidden",
            EChangePanicGracePeriodError.NotFound => "not_found",
            EChangePanicGracePeriodError.Conflict => "conflict",
            EChangePanicGracePeriodError.ConcurrencyConflict => "concurrency",
            EChangePanicGracePeriodError.InvalidTransition => "transition",
            EChangePanicGracePeriodError.AlreadyCompleted => "already_completed",
            EChangePanicGracePeriodError.UnexpectedError => "unexpected",
            _ => "unexpected"
        }, failure.Field, localizer),
        _ => ProblemDetailsAssembler.ToActionResult(controller, "unexpected", "", localizer)};
}
