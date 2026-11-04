using InstAlertPlatform.API.Business.Application.Errors;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Business.Interfaces.Rest.Transform;
public static class ActionResultFromUpdateBusinessResultAssembler
{
    public static IActionResult ToActionResultFromUpdateBusinessResult(Result<BusinessSnapshot, EUpdateBusinessError> result, ControllerBase controller, IStringLocalizer<SharedResource> localizer, Func<BusinessSnapshot, IActionResult> onSuccess) => result switch
    {
        Result<BusinessSnapshot, EUpdateBusinessError>.Success success => onSuccess(success.Value),
        Result<BusinessSnapshot, EUpdateBusinessError>.Failure failure => ProblemDetailsAssembler.ToActionResult(controller, failure.Error switch
        {
            EUpdateBusinessError.InvalidInput => "validation",
            EUpdateBusinessError.Unauthenticated => "unauthenticated",
            EUpdateBusinessError.Forbidden => "forbidden",
            EUpdateBusinessError.NotFound => "not_found",
            EUpdateBusinessError.Conflict => "conflict",
            EUpdateBusinessError.ConcurrencyConflict => "concurrency",
            EUpdateBusinessError.CapacityExceeded => "capacity",
            EUpdateBusinessError.InvalidTransition => "transition",
            EUpdateBusinessError.ProviderUnavailable => "provider_unavailable",
            EUpdateBusinessError.IdentityUnavailable => "identity_unavailable",
            EUpdateBusinessError.UnexpectedError => "unexpected",
            _ => "unexpected"
        }, failure.Field, localizer),
        _ => ProblemDetailsAssembler.ToActionResult(controller, "unexpected", "", localizer)};
}
