using InstAlertPlatform.API.Alert.Application.Errors;
using InstAlertPlatform.API.Alert.Application.Services;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Alert.Interfaces.Rest.Transform;
public static class ActionResultFromUpdateAlertLocationResultAssembler
{
    public static IActionResult ToActionResultFromUpdateAlertLocationResult(Result<AlertFact, EUpdateAlertLocationError> result, ControllerBase controller, IStringLocalizer<SharedResource> localizer, Func<AlertFact, IActionResult> onSuccess) => result switch
    {
        Result<AlertFact, EUpdateAlertLocationError>.Success success => onSuccess(success.Value),
        Result<AlertFact, EUpdateAlertLocationError>.Failure failure => ProblemDetailsAssembler.ToActionResult(controller, failure.Error switch
        {
            EUpdateAlertLocationError.InvalidInput => "validation",
            EUpdateAlertLocationError.Unauthenticated => "unauthenticated",
            EUpdateAlertLocationError.Forbidden => "forbidden",
            EUpdateAlertLocationError.NotFound => "not_found",
            EUpdateAlertLocationError.Conflict => "conflict",
            EUpdateAlertLocationError.ConcurrencyConflict => "concurrency",
            EUpdateAlertLocationError.InvalidTransition => "transition",
            EUpdateAlertLocationError.AlreadyCompleted => "already_completed",
            EUpdateAlertLocationError.UnexpectedError => "unexpected",
            _ => "unexpected"
        }, failure.Field, localizer),
        _ => ProblemDetailsAssembler.ToActionResult(controller, "unexpected", "", localizer)};
}
