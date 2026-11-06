using InstAlertPlatform.API.Alert.Application.Errors;
using InstAlertPlatform.API.Alert.Application.Services;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Alert.Interfaces.Rest.Transform;
public static class ActionResultFromCompletePanicReportResultAssembler
{
    public static IActionResult ToActionResultFromCompletePanicReportResult(Result<AlertFact, ECompletePanicReportError> result, ControllerBase controller, IStringLocalizer<SharedResource> localizer, Func<AlertFact, IActionResult> onSuccess) => result switch
    {
        Result<AlertFact, ECompletePanicReportError>.Success success => onSuccess(success.Value),
        Result<AlertFact, ECompletePanicReportError>.Failure failure => ProblemDetailsAssembler.ToActionResult(controller, failure.Error switch
        {
            ECompletePanicReportError.InvalidInput => "validation",
            ECompletePanicReportError.Unauthenticated => "unauthenticated",
            ECompletePanicReportError.Forbidden => "forbidden",
            ECompletePanicReportError.NotFound => "not_found",
            ECompletePanicReportError.Conflict => "conflict",
            ECompletePanicReportError.ConcurrencyConflict => "concurrency",
            ECompletePanicReportError.InvalidTransition => "transition",
            ECompletePanicReportError.AlreadyCompleted => "already_completed",
            ECompletePanicReportError.UnexpectedError => "unexpected",
            _ => "unexpected"
        }, failure.Field, localizer),
        _ => ProblemDetailsAssembler.ToActionResult(controller, "unexpected", "", localizer)};
}
