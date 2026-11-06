using InstAlertPlatform.API.Alert.Application.Errors;
using InstAlertPlatform.API.Alert.Application.Services;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Alert.Interfaces.Rest.Transform;
public static class ActionResultFromPreparePanicResultAssembler
{
    public static IActionResult ToActionResultFromPreparePanicResult(Result<PanicPreparationSnapshot, EPreparePanicError> result, ControllerBase controller, IStringLocalizer<SharedResource> localizer, Func<PanicPreparationSnapshot, IActionResult> onSuccess) => result switch
    {
        Result<PanicPreparationSnapshot, EPreparePanicError>.Success success => onSuccess(success.Value),
        Result<PanicPreparationSnapshot, EPreparePanicError>.Failure failure => ProblemDetailsAssembler.ToActionResult(controller, failure.Error switch
        {
            EPreparePanicError.InvalidInput => "validation",
            EPreparePanicError.Unauthenticated => "unauthenticated",
            EPreparePanicError.Forbidden => "forbidden",
            EPreparePanicError.NotFound => "not_found",
            EPreparePanicError.Conflict => "conflict",
            EPreparePanicError.ConcurrencyConflict => "concurrency",
            EPreparePanicError.InvalidTransition => "transition",
            EPreparePanicError.AlreadyCompleted => "already_completed",
            EPreparePanicError.UnexpectedError => "unexpected",
            _ => "unexpected"
        }, failure.Field, localizer),
        _ => ProblemDetailsAssembler.ToActionResult(controller, "unexpected", "", localizer)};
}
