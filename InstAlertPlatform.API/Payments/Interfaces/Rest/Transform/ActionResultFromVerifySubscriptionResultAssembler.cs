using InstAlertPlatform.API.Payments.Application.Errors;
using InstAlertPlatform.API.Payments.Application.Services;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Payments.Interfaces.Rest.Transform;
public static class ActionResultFromVerifySubscriptionResultAssembler
{
    public static IActionResult ToActionResultFromVerifySubscriptionResult(Result<SubscriptionSnapshot, EVerifySubscriptionError> result, ControllerBase controller, IStringLocalizer<SharedResource> localizer, Func<SubscriptionSnapshot, IActionResult> onSuccess) => result switch
    {
        Result<SubscriptionSnapshot, EVerifySubscriptionError>.Success success => onSuccess(success.Value),
        Result<SubscriptionSnapshot, EVerifySubscriptionError>.Failure failure => ProblemDetailsAssembler.ToActionResult(controller, failure.Error switch
        {
            EVerifySubscriptionError.InvalidInput => "validation",
            EVerifySubscriptionError.Unauthenticated => "unauthenticated",
            EVerifySubscriptionError.Forbidden => "forbidden",
            EVerifySubscriptionError.NotFound => "not_found",
            EVerifySubscriptionError.Conflict => "conflict",
            EVerifySubscriptionError.ConcurrencyConflict => "concurrency",
            EVerifySubscriptionError.CapacityExceeded => "capacity",
            EVerifySubscriptionError.InvalidTransition => "transition",
            EVerifySubscriptionError.ProviderUnavailable => "provider_unavailable",
            EVerifySubscriptionError.UnexpectedError => "unexpected",
            _ => "unexpected"
        }, failure.Field, localizer),
        _ => ProblemDetailsAssembler.ToActionResult(controller, "unexpected", "", localizer)};
}
