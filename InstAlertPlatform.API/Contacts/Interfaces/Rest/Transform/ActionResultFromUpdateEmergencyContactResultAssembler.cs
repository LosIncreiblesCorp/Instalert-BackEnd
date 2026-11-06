using InstAlertPlatform.API.Contacts.Application.Errors;
using InstAlertPlatform.API.Contacts.Application.Services;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Contacts.Interfaces.Rest.Transform;
public static class ActionResultFromUpdateEmergencyContactResultAssembler
{
    public static IActionResult ToActionResultFromUpdateEmergencyContactResult(Result<ContactSnapshot, EUpdateEmergencyContactError> result, ControllerBase controller, IStringLocalizer<SharedResource> localizer, Func<ContactSnapshot, IActionResult> onSuccess) => result switch
    {
        Result<ContactSnapshot, EUpdateEmergencyContactError>.Success success => onSuccess(success.Value),
        Result<ContactSnapshot, EUpdateEmergencyContactError>.Failure failure => ProblemDetailsAssembler.ToActionResult(controller, failure.Error switch
        {
            EUpdateEmergencyContactError.InvalidInput => "validation",
            EUpdateEmergencyContactError.Unauthenticated => "unauthenticated",
            EUpdateEmergencyContactError.Forbidden => "forbidden",
            EUpdateEmergencyContactError.NotFound => "not_found",
            EUpdateEmergencyContactError.Conflict => "conflict",
            EUpdateEmergencyContactError.ConcurrencyConflict => "concurrency",
            EUpdateEmergencyContactError.UnexpectedError => "unexpected",
            _ => "unexpected"
        }, failure.Field, localizer),
        _ => ProblemDetailsAssembler.ToActionResult(controller, "unexpected", "", localizer)};
}
