using InstAlertPlatform.API.Alert.Application.Errors;
using InstAlertPlatform.API.Alert.Application.Services;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Alert.Interfaces.Rest.Transform;
public static class ActionResultFromCreateIncidentReportResultAssembler
{
    public static IActionResult ToActionResultFromCreateIncidentReportResult(Result<AlertFact, ECreateIncidentReportError> result, ControllerBase controller, IStringLocalizer<SharedResource> localizer, Func<AlertFact, IActionResult> onSuccess) => result switch
    {
        Result<AlertFact, ECreateIncidentReportError>.Success success => onSuccess(success.Value),
        Result<AlertFact, ECreateIncidentReportError>.Failure failure => ProblemDetailsAssembler.ToActionResult(controller, failure.Error switch
        {
            ECreateIncidentReportError.InvalidInput => "validation",
            ECreateIncidentReportError.Unauthenticated => "unauthenticated",
            ECreateIncidentReportError.Forbidden => "forbidden",
            ECreateIncidentReportError.NotFound => "not_found",
            ECreateIncidentReportError.Conflict => "conflict",
            ECreateIncidentReportError.ConcurrencyConflict => "concurrency",
            ECreateIncidentReportError.InvalidTransition => "transition",
            ECreateIncidentReportError.AlreadyCompleted => "already_completed",
            ECreateIncidentReportError.UnexpectedError => "unexpected",
            _ => "unexpected"
        }, failure.Field, localizer),
        _ => ProblemDetailsAssembler.ToActionResult(controller, "unexpected", "", localizer)};
}
