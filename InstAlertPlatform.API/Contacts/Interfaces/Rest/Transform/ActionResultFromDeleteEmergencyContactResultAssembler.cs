using InstAlertPlatform.API.Contacts.Application.Errors;
using InstAlertPlatform.API.Contacts.Application.Services;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Contacts.Interfaces.Rest.Transform;
public static class ActionResultFromDeleteEmergencyContactResultAssembler
{
    public static IActionResult ToActionResultFromDeleteEmergencyContactResult(Result<bool, EDeleteEmergencyContactError> result, ControllerBase controller, IStringLocalizer<SharedResource> localizer, Func<bool, IActionResult> onSuccess) => result switch
    {
        Result<bool, EDeleteEmergencyContactError>.Success success => onSuccess(success.Value),
        Result<bool, EDeleteEmergencyContactError>.Failure failure => ProblemDetailsAssembler.ToActionResult(controller, failure.Error switch
        {
            EDeleteEmergencyContactError.InvalidInput => "validation",
            EDeleteEmergencyContactError.Unauthenticated => "unauthenticated",
            EDeleteEmergencyContactError.Forbidden => "forbidden",
            EDeleteEmergencyContactError.NotFound => "not_found",
            EDeleteEmergencyContactError.Conflict => "conflict",
            EDeleteEmergencyContactError.ConcurrencyConflict => "concurrency",
            EDeleteEmergencyContactError.UnexpectedError => "unexpected",
            _ => "unexpected"
        }, failure.Field, localizer),
        _ => ProblemDetailsAssembler.ToActionResult(controller, "unexpected", "", localizer)};
}
