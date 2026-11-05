using InstAlertPlatform.API.Business.Application.Errors;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Business.Interfaces.Rest.Transform;
public static class ActionResultFromCreateBusinessResultAssembler
{
    public static IActionResult ToActionResultFromCreateBusinessResult(Result<BusinessSnapshot, ECreateBusinessError> result, ControllerBase controller, IStringLocalizer<SharedResource> localizer, Func<BusinessSnapshot, IActionResult> onSuccess) => result switch
    {
        Result<BusinessSnapshot, ECreateBusinessError>.Success success => onSuccess(success.Value),
        Result<BusinessSnapshot, ECreateBusinessError>.Failure failure => ProblemDetailsAssembler.ToActionResult(controller, failure.Error switch
        {
            ECreateBusinessError.InvalidInput => "validation",
            ECreateBusinessError.Unauthenticated => "unauthenticated",
            ECreateBusinessError.Forbidden => "forbidden",
            ECreateBusinessError.NotFound => "not_found",
            ECreateBusinessError.Conflict => "conflict",
            ECreateBusinessError.ConcurrencyConflict => "concurrency",
            ECreateBusinessError.CapacityExceeded => "capacity",
            ECreateBusinessError.InvalidTransition => "transition",
            ECreateBusinessError.ProviderUnavailable => "provider_unavailable",
            ECreateBusinessError.IdentityUnavailable => "identity_unavailable",
            ECreateBusinessError.UnexpectedError => "unexpected",
            _ => "unexpected"
        }, failure.Field, localizer),
        _ => ProblemDetailsAssembler.ToActionResult(controller, "unexpected", "", localizer)};
}
