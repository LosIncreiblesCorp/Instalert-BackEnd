using InstAlertPlatform.API.Payments.Application.Errors;
using InstAlertPlatform.API.Payments.Application.Services;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Payments.Interfaces.Rest.Transform;
public static class ActionResultFromReconcilePaymentResultAssembler
{
    public static IActionResult ToActionResultFromReconcilePaymentResult(Result<bool, EReconcilePaymentError> result, ControllerBase controller, IStringLocalizer<SharedResource> localizer, Func<bool, IActionResult> onSuccess) => result switch
    {
        Result<bool, EReconcilePaymentError>.Success success => onSuccess(success.Value),
        Result<bool, EReconcilePaymentError>.Failure failure => ProblemDetailsAssembler.ToActionResult(controller, failure.Error switch
        {
            EReconcilePaymentError.InvalidInput => "validation",
            EReconcilePaymentError.Unauthenticated => "unauthenticated",
            EReconcilePaymentError.Forbidden => "forbidden",
            EReconcilePaymentError.NotFound => "not_found",
            EReconcilePaymentError.Conflict => "conflict",
            EReconcilePaymentError.ConcurrencyConflict => "concurrency",
            EReconcilePaymentError.CapacityExceeded => "capacity",
            EReconcilePaymentError.InvalidTransition => "transition",
            EReconcilePaymentError.ProviderUnavailable => "provider_unavailable",
            EReconcilePaymentError.UnexpectedError => "unexpected",
            _ => "unexpected"
        }, failure.Field, localizer),
        _ => ProblemDetailsAssembler.ToActionResult(controller, "unexpected", "", localizer)};
}
