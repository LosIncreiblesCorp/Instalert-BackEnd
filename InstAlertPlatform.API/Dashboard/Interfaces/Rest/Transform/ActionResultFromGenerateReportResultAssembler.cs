using InstAlertPlatform.API.Dashboard.Application.Errors;
using InstAlertPlatform.API.Dashboard.Application.Services;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Dashboard.Interfaces.Rest.Transform;
public static class ActionResultFromGenerateReportResultAssembler
{
    public static IActionResult ToActionResultFromGenerateReportResult(Result<ReportSnapshot, EGenerateReportError> result, ControllerBase controller, IStringLocalizer<SharedResource> localizer, Func<ReportSnapshot, IActionResult> onSuccess) => result switch
    {
        Result<ReportSnapshot, EGenerateReportError>.Success success => onSuccess(success.Value),
        Result<ReportSnapshot, EGenerateReportError>.Failure failure => ProblemDetailsAssembler.ToActionResult(controller, failure.Error switch
        {
            EGenerateReportError.InvalidInput => "validation",
            EGenerateReportError.Unauthenticated => "unauthenticated",
            EGenerateReportError.Forbidden => "forbidden",
            EGenerateReportError.NotFound => "not_found",
            EGenerateReportError.Conflict => "conflict",
            EGenerateReportError.ConcurrencyConflict => "concurrency",
            EGenerateReportError.InvalidTransition => "transition",
            EGenerateReportError.PolicyUnavailable => "policy_unavailable",
            EGenerateReportError.UnexpectedError => "unexpected",
            _ => "unexpected"
        }, failure.Field, localizer),
        _ => ProblemDetailsAssembler.ToActionResult(controller, "unexpected", "", localizer)};
}
