using InstAlertPlatform.API.Alert.Application.Errors;
using InstAlertPlatform.API.Alert.Application.Services;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Alert.Interfaces.Rest.Transform;
public static class ActionResultFromResolvePanicAlertResultAssembler
{
    public static IActionResult ToActionResultFromResolvePanicAlertResult(Result<AlertFact, EResolvePanicAlertError> result, ControllerBase controller, IStringLocalizer<SharedResource> localizer, Func<AlertFact, IActionResult> onSuccess) => result switch
    {
        Result<AlertFact, EResolvePanicAlertError>.Success success => onSuccess(success.Value),
        Result<AlertFact, EResolvePanicAlertError>.Failure failure => ProblemDetailsAssembler.ToActionResult(controller, failure.Error switch
        {
            EResolvePanicAlertError.InvalidInput => "validation",
            EResolvePanicAlertError.Unauthenticated => "unauthenticated",
            EResolvePanicAlertError.Forbidden => "forbidden",
            EResolvePanicAlertError.NotFound => "not_found",
            EResolvePanicAlertError.Conflict => "conflict",
            EResolvePanicAlertError.ConcurrencyConflict => "concurrency",
            EResolvePanicAlertError.InvalidTransition => "transition",
            EResolvePanicAlertError.AlreadyCompleted => "already_completed",
            EResolvePanicAlertError.UnexpectedError => "unexpected",
            _ => "unexpected"
        }, failure.Field, localizer),
        _ => ProblemDetailsAssembler.ToActionResult(controller, "unexpected", "", localizer)};
}
