using InstAlertPlatform.API.Alert.Application.Errors;
using InstAlertPlatform.API.Alert.Application.Services;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Alert.Interfaces.Rest.Transform;
public static class ActionResultFromUpdatePanicReportDraftResultAssembler
{
    public static IActionResult ToActionResultFromUpdatePanicReportDraftResult(Result<AlertFact, EUpdatePanicReportDraftError> result, ControllerBase controller, IStringLocalizer<SharedResource> localizer, Func<AlertFact, IActionResult> onSuccess) => result switch
    {
        Result<AlertFact, EUpdatePanicReportDraftError>.Success success => onSuccess(success.Value),
        Result<AlertFact, EUpdatePanicReportDraftError>.Failure failure => ProblemDetailsAssembler.ToActionResult(controller, failure.Error switch
        {
            EUpdatePanicReportDraftError.InvalidInput => "validation",
            EUpdatePanicReportDraftError.Unauthenticated => "unauthenticated",
            EUpdatePanicReportDraftError.Forbidden => "forbidden",
            EUpdatePanicReportDraftError.NotFound => "not_found",
            EUpdatePanicReportDraftError.Conflict => "conflict",
            EUpdatePanicReportDraftError.ConcurrencyConflict => "concurrency",
            EUpdatePanicReportDraftError.InvalidTransition => "transition",
            EUpdatePanicReportDraftError.AlreadyCompleted => "already_completed",
            EUpdatePanicReportDraftError.UnexpectedError => "unexpected",
            _ => "unexpected"
        }, failure.Field, localizer),
        _ => ProblemDetailsAssembler.ToActionResult(controller, "unexpected", "", localizer)};
}
