using InstAlertPlatform.API.Alert.Application.Errors;
using InstAlertPlatform.API.Alert.Application.Services;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Alert.Interfaces.Rest.Transform;
public static class ActionResultFromConfirmPanicResultAssembler
{
    public static IActionResult ToActionResultFromConfirmPanicResult(Result<AlertFact, EConfirmPanicError> result, ControllerBase controller, IStringLocalizer<SharedResource> localizer, Func<AlertFact, IActionResult> onSuccess) => result switch
    {
        Result<AlertFact, EConfirmPanicError>.Success success => onSuccess(success.Value),
        Result<AlertFact, EConfirmPanicError>.Failure failure => ProblemDetailsAssembler.ToActionResult(controller, failure.Error switch
        {
            EConfirmPanicError.InvalidInput => "validation",
            EConfirmPanicError.Unauthenticated => "unauthenticated",
            EConfirmPanicError.Forbidden => "forbidden",
            EConfirmPanicError.NotFound => "not_found",
            EConfirmPanicError.Conflict => "conflict",
            EConfirmPanicError.ConcurrencyConflict => "concurrency",
            EConfirmPanicError.InvalidTransition => "transition",
            EConfirmPanicError.AlreadyCompleted => "already_completed",
            EConfirmPanicError.UnexpectedError => "unexpected",
            _ => "unexpected"
        }, failure.Field, localizer),
        _ => ProblemDetailsAssembler.ToActionResult(controller, "unexpected", "", localizer)};
}
