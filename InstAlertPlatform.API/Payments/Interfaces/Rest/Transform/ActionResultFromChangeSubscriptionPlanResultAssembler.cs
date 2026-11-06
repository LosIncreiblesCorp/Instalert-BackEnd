using InstAlertPlatform.API.Payments.Application.Errors;
using InstAlertPlatform.API.Payments.Application.Services;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Payments.Interfaces.Rest.Transform;
public static class ActionResultFromChangeSubscriptionPlanResultAssembler
{
    public static IActionResult ToActionResultFromChangeSubscriptionPlanResult(Result<SubscriptionCheckoutSnapshot, EChangeSubscriptionPlanError> result, ControllerBase controller, IStringLocalizer<SharedResource> localizer, Func<SubscriptionCheckoutSnapshot, IActionResult> onSuccess) => result switch
    {
        Result<SubscriptionCheckoutSnapshot, EChangeSubscriptionPlanError>.Success success => onSuccess(success.Value),
        Result<SubscriptionCheckoutSnapshot, EChangeSubscriptionPlanError>.Failure failure => ProblemDetailsAssembler.ToActionResult(controller, failure.Error switch
        {
            EChangeSubscriptionPlanError.InvalidInput => "validation",
            EChangeSubscriptionPlanError.Unauthenticated => "unauthenticated",
            EChangeSubscriptionPlanError.Forbidden => "forbidden",
            EChangeSubscriptionPlanError.NotFound => "not_found",
            EChangeSubscriptionPlanError.Conflict => "conflict",
            EChangeSubscriptionPlanError.ConcurrencyConflict => "concurrency",
            EChangeSubscriptionPlanError.CapacityExceeded => "capacity",
            EChangeSubscriptionPlanError.InvalidTransition => "transition",
            EChangeSubscriptionPlanError.ProviderUnavailable => "provider_unavailable",
            EChangeSubscriptionPlanError.UnexpectedError => "unexpected",
            _ => "unexpected"
        }, failure.Field, localizer),
        _ => ProblemDetailsAssembler.ToActionResult(controller, "unexpected", "", localizer)};
}
