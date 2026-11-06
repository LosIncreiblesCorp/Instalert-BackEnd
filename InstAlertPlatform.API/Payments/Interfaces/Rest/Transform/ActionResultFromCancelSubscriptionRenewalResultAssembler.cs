using InstAlertPlatform.API.Payments.Application.Errors;
using InstAlertPlatform.API.Payments.Application.Services;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Payments.Interfaces.Rest.Transform;
public static class ActionResultFromCancelSubscriptionRenewalResultAssembler
{
    public static IActionResult ToActionResultFromCancelSubscriptionRenewalResult(Result<SubscriptionSnapshot, ECancelSubscriptionRenewalError> result, ControllerBase controller, IStringLocalizer<SharedResource> localizer, Func<SubscriptionSnapshot, IActionResult> onSuccess) => result switch
    {
        Result<SubscriptionSnapshot, ECancelSubscriptionRenewalError>.Success success => onSuccess(success.Value),
        Result<SubscriptionSnapshot, ECancelSubscriptionRenewalError>.Failure failure => ProblemDetailsAssembler.ToActionResult(controller, failure.Error switch
        {
            ECancelSubscriptionRenewalError.InvalidInput => "validation",
            ECancelSubscriptionRenewalError.Unauthenticated => "unauthenticated",
            ECancelSubscriptionRenewalError.Forbidden => "forbidden",
            ECancelSubscriptionRenewalError.NotFound => "not_found",
            ECancelSubscriptionRenewalError.Conflict => "conflict",
            ECancelSubscriptionRenewalError.ConcurrencyConflict => "concurrency",
            ECancelSubscriptionRenewalError.CapacityExceeded => "capacity",
            ECancelSubscriptionRenewalError.InvalidTransition => "transition",
            ECancelSubscriptionRenewalError.ProviderUnavailable => "provider_unavailable",
            ECancelSubscriptionRenewalError.UnexpectedError => "unexpected",
            _ => "unexpected"
        }, failure.Field, localizer),
        _ => ProblemDetailsAssembler.ToActionResult(controller, "unexpected", "", localizer)};
}
