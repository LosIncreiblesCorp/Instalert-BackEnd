using InstAlertPlatform.API.Payments.Application.Services.Snapshots;
using InstAlertPlatform.API.Payments.Application.Errors;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Payments.Interfaces.Rest.Transform;
public static class ActionResultFromResumeSubscriptionRenewalResultAssembler
{
    public static IActionResult ToActionResultFromResumeSubscriptionRenewalResult(Result<SubscriptionSnapshot, EResumeSubscriptionRenewalError> result, ControllerBase controller, IStringLocalizer<SharedResource> localizer, Func<SubscriptionSnapshot, IActionResult> onSuccess) => result switch
    {
        Result<SubscriptionSnapshot, EResumeSubscriptionRenewalError>.Success success => onSuccess(success.Value),
        Result<SubscriptionSnapshot, EResumeSubscriptionRenewalError>.Failure failure => ProblemDetailsAssembler.ToActionResult(controller, failure.Error switch
        {
            EResumeSubscriptionRenewalError.InvalidInput => "validation",
            EResumeSubscriptionRenewalError.Unauthenticated => "unauthenticated",
            EResumeSubscriptionRenewalError.Forbidden => "forbidden",
            EResumeSubscriptionRenewalError.NotFound => "not_found",
            EResumeSubscriptionRenewalError.Conflict => "conflict",
            EResumeSubscriptionRenewalError.ConcurrencyConflict => "concurrency",
            EResumeSubscriptionRenewalError.CapacityExceeded => "capacity",
            EResumeSubscriptionRenewalError.InvalidTransition => "transition",
            EResumeSubscriptionRenewalError.ProviderUnavailable => "provider_unavailable",
            EResumeSubscriptionRenewalError.UnexpectedError => "unexpected",
            _ => "unexpected"
        }, failure.Field, localizer),
        _ => ProblemDetailsAssembler.ToActionResult(controller, "unexpected", "", localizer)};
}
