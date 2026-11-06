using InstAlertPlatform.API.Contacts.Application.Errors;
using InstAlertPlatform.API.Contacts.Application.Services;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Contacts.Interfaces.Rest.Transform;
public static class ActionResultFromCreateEmergencyContactResultAssembler
{
    public static IActionResult ToActionResultFromCreateEmergencyContactResult(Result<ContactSnapshot, ECreateEmergencyContactError> result, ControllerBase controller, IStringLocalizer<SharedResource> localizer, Func<ContactSnapshot, IActionResult> onSuccess) => result switch
    {
        Result<ContactSnapshot, ECreateEmergencyContactError>.Success success => onSuccess(success.Value),
        Result<ContactSnapshot, ECreateEmergencyContactError>.Failure failure => ProblemDetailsAssembler.ToActionResult(controller, failure.Error switch
        {
            ECreateEmergencyContactError.InvalidInput => "validation",
            ECreateEmergencyContactError.Unauthenticated => "unauthenticated",
            ECreateEmergencyContactError.Forbidden => "forbidden",
            ECreateEmergencyContactError.NotFound => "not_found",
            ECreateEmergencyContactError.Conflict => "conflict",
            ECreateEmergencyContactError.ConcurrencyConflict => "concurrency",
            ECreateEmergencyContactError.UnexpectedError => "unexpected",
            _ => "unexpected"
        }, failure.Field, localizer),
        _ => ProblemDetailsAssembler.ToActionResult(controller, "unexpected", "", localizer)};
}
