using InstAlertPlatform.API.Notifications.Application.Errors;
using InstAlertPlatform.API.Notifications.Application.Services;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Notifications.Interfaces.Rest.Transform;
public static class ActionResultFromChangeNotificationPreferencesResultAssembler
{
    public static IActionResult ToActionResultFromChangeNotificationPreferencesResult(Result<PreferencesSnapshot, EChangeNotificationPreferencesError> result, ControllerBase controller, IStringLocalizer<SharedResource> localizer, Func<PreferencesSnapshot, IActionResult> onSuccess) => result switch
    {
        Result<PreferencesSnapshot, EChangeNotificationPreferencesError>.Success success => onSuccess(success.Value),
        Result<PreferencesSnapshot, EChangeNotificationPreferencesError>.Failure failure => ProblemDetailsAssembler.ToActionResult(controller, failure.Error switch
        {
            EChangeNotificationPreferencesError.InvalidInput => "validation",
            EChangeNotificationPreferencesError.Unauthenticated => "unauthenticated",
            EChangeNotificationPreferencesError.Forbidden => "forbidden",
            EChangeNotificationPreferencesError.NotFound => "not_found",
            EChangeNotificationPreferencesError.Conflict => "conflict",
            EChangeNotificationPreferencesError.ConcurrencyConflict => "concurrency",
            EChangeNotificationPreferencesError.UnexpectedError => "unexpected",
            _ => "unexpected"
        }, failure.Field, localizer),
        _ => ProblemDetailsAssembler.ToActionResult(controller, "unexpected", "", localizer)};
}
