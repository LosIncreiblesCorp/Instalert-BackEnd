using InstAlertPlatform.API.Payments.Application.Errors;
using InstAlertPlatform.API.Payments.Application.Services;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Payments.Interfaces.Rest.Transform;
public static class ActionResultFromVerifySubscriptionPlanChangeResultAssembler
{
    public static IActionResult ToActionResultFromVerifySubscriptionPlanChangeResult(Result<SubscriptionSnapshot, EVerifySubscriptionPlanChangeError> result, ControllerBase controller, IStringLocalizer<SharedResource> localizer, Func<SubscriptionSnapshot, IActionResult> onSuccess) => result switch
    {
        Result<SubscriptionSnapshot, EVerifySubscriptionPlanChangeError>.Success success => onSuccess(success.Value),
        Result<SubscriptionSnapshot, EVerifySubscriptionPlanChangeError>.Failure failure => ProblemDetailsAssembler.ToActionResult(controller, failure.Error switch
        {
            EVerifySubscriptionPlanChangeError.InvalidInput => "validation",
            EVerifySubscriptionPlanChangeError.Unauthenticated => "unauthenticated",
            EVerifySubscriptionPlanChangeError.Forbidden => "forbidden",
            EVerifySubscriptionPlanChangeError.NotFound => "not_found",
            EVerifySubscriptionPlanChangeError.Conflict => "conflict",
            EVerifySubscriptionPlanChangeError.ConcurrencyConflict => "concurrency",
            EVerifySubscriptionPlanChangeError.CapacityExceeded => "capacity",
            EVerifySubscriptionPlanChangeError.InvalidTransition => "transition",
            EVerifySubscriptionPlanChangeError.ProviderUnavailable => "provider_unavailable",
            EVerifySubscriptionPlanChangeError.UnexpectedError => "unexpected",
            _ => "unexpected"
        }, failure.Field, localizer),
        _ => ProblemDetailsAssembler.ToActionResult(controller, "unexpected", "", localizer)};
}
