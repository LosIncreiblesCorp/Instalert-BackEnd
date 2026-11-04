using InstAlertPlatform.API.Business.Application.Errors;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Business.Interfaces.Rest.Transform;
public static class ActionResultFromUpdateStaffInvitationResultAssembler
{
    public static IActionResult ToActionResultFromUpdateStaffInvitationResult(Result<InvitationSnapshot, EUpdateStaffInvitationError> result, ControllerBase controller, IStringLocalizer<SharedResource> localizer, Func<InvitationSnapshot, IActionResult> onSuccess) => result switch
    {
        Result<InvitationSnapshot, EUpdateStaffInvitationError>.Success success => onSuccess(success.Value),
        Result<InvitationSnapshot, EUpdateStaffInvitationError>.Failure failure => ProblemDetailsAssembler.ToActionResult(controller, failure.Error switch
        {
            EUpdateStaffInvitationError.InvalidInput => "validation",
            EUpdateStaffInvitationError.Unauthenticated => "unauthenticated",
            EUpdateStaffInvitationError.Forbidden => "forbidden",
            EUpdateStaffInvitationError.NotFound => "not_found",
            EUpdateStaffInvitationError.Conflict => "conflict",
            EUpdateStaffInvitationError.ConcurrencyConflict => "concurrency",
            EUpdateStaffInvitationError.CapacityExceeded => "capacity",
            EUpdateStaffInvitationError.InvalidTransition => "transition",
            EUpdateStaffInvitationError.ProviderUnavailable => "provider_unavailable",
            EUpdateStaffInvitationError.IdentityUnavailable => "identity_unavailable",
            EUpdateStaffInvitationError.UnexpectedError => "unexpected",
            _ => "unexpected"
        }, failure.Field, localizer),
        _ => ProblemDetailsAssembler.ToActionResult(controller, "unexpected", "", localizer)};
}
