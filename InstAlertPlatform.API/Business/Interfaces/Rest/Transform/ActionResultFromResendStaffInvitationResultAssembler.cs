using InstAlertPlatform.API.Business.Application.Errors;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Business.Interfaces.Rest.Transform;
public static class ActionResultFromResendStaffInvitationResultAssembler
{
    public static IActionResult ToActionResultFromResendStaffInvitationResult(Result<InvitationSnapshot, EResendStaffInvitationError> result, ControllerBase controller, IStringLocalizer<SharedResource> localizer, Func<InvitationSnapshot, IActionResult> onSuccess) => result switch
    {
        Result<InvitationSnapshot, EResendStaffInvitationError>.Success success => onSuccess(success.Value),
        Result<InvitationSnapshot, EResendStaffInvitationError>.Failure failure => ProblemDetailsAssembler.ToActionResult(controller, failure.Error switch
        {
            EResendStaffInvitationError.InvalidInput => "validation",
            EResendStaffInvitationError.Unauthenticated => "unauthenticated",
            EResendStaffInvitationError.Forbidden => "forbidden",
            EResendStaffInvitationError.NotFound => "not_found",
            EResendStaffInvitationError.Conflict => "conflict",
            EResendStaffInvitationError.ConcurrencyConflict => "concurrency",
            EResendStaffInvitationError.CapacityExceeded => "capacity",
            EResendStaffInvitationError.InvalidTransition => "transition",
            EResendStaffInvitationError.ProviderUnavailable => "provider_unavailable",
            EResendStaffInvitationError.IdentityUnavailable => "identity_unavailable",
            EResendStaffInvitationError.UnexpectedError => "unexpected",
            _ => "unexpected"
        }, failure.Field, localizer),
        _ => ProblemDetailsAssembler.ToActionResult(controller, "unexpected", "", localizer)};
}
