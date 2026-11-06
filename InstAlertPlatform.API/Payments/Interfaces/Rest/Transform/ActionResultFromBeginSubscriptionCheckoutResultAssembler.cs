using InstAlertPlatform.API.Payments.Application.Errors;
using InstAlertPlatform.API.Payments.Application.Services;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Payments.Interfaces.Rest.Transform;
public static class ActionResultFromBeginSubscriptionCheckoutResultAssembler
{
    public static IActionResult ToActionResultFromBeginSubscriptionCheckoutResult(Result<SubscriptionCheckoutSnapshot, EBeginSubscriptionCheckoutError> result, ControllerBase controller, IStringLocalizer<SharedResource> localizer, Func<SubscriptionCheckoutSnapshot, IActionResult> onSuccess) => result switch
    {
        Result<SubscriptionCheckoutSnapshot, EBeginSubscriptionCheckoutError>.Success success => onSuccess(success.Value),
        Result<SubscriptionCheckoutSnapshot, EBeginSubscriptionCheckoutError>.Failure failure => ProblemDetailsAssembler.ToActionResult(controller, failure.Error switch
        {
            EBeginSubscriptionCheckoutError.InvalidInput => "validation",
            EBeginSubscriptionCheckoutError.Unauthenticated => "unauthenticated",
            EBeginSubscriptionCheckoutError.Forbidden => "forbidden",
            EBeginSubscriptionCheckoutError.NotFound => "not_found",
            EBeginSubscriptionCheckoutError.Conflict => "conflict",
            EBeginSubscriptionCheckoutError.ConcurrencyConflict => "concurrency",
            EBeginSubscriptionCheckoutError.CapacityExceeded => "capacity",
            EBeginSubscriptionCheckoutError.InvalidTransition => "transition",
            EBeginSubscriptionCheckoutError.ProviderUnavailable => "provider_unavailable",
            EBeginSubscriptionCheckoutError.PolicyUnavailable => "policy_unavailable",
            EBeginSubscriptionCheckoutError.UnexpectedError => "unexpected",
            _ => "unexpected"
        }, failure.Field, localizer),
        _ => ProblemDetailsAssembler.ToActionResult(controller, "unexpected", "", localizer)};
}
