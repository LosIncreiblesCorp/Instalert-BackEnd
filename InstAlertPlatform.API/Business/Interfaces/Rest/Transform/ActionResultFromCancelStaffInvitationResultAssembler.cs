using InstAlertPlatform.API.Business.Application.Errors;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Business.Interfaces.Rest.Transform;
public static class ActionResultFromCancelStaffInvitationResultAssembler
{
    public static IActionResult ToActionResultFromCancelStaffInvitationResult(Result<InvitationSnapshot, ECancelStaffInvitationError> result, ControllerBase controller, IStringLocalizer<SharedResource> localizer, Func<InvitationSnapshot, IActionResult> onSuccess) => result switch
    {
        Result<InvitationSnapshot, ECancelStaffInvitationError>.Success success => onSuccess(success.Value),
        Result<InvitationSnapshot, ECancelStaffInvitationError>.Failure failure => ProblemDetailsAssembler.ToActionResult(controller, failure.Error switch
        {
            ECancelStaffInvitationError.InvalidInput => "validation",
            ECancelStaffInvitationError.Unauthenticated => "unauthenticated",
            ECancelStaffInvitationError.Forbidden => "forbidden",
            ECancelStaffInvitationError.NotFound => "not_found",
            ECancelStaffInvitationError.Conflict => "conflict",
            ECancelStaffInvitationError.ConcurrencyConflict => "concurrency",
            ECancelStaffInvitationError.CapacityExceeded => "capacity",
            ECancelStaffInvitationError.InvalidTransition => "transition",
            ECancelStaffInvitationError.ProviderUnavailable => "provider_unavailable",
            ECancelStaffInvitationError.IdentityUnavailable => "identity_unavailable",
            ECancelStaffInvitationError.UnexpectedError => "unexpected",
            _ => "unexpected"
        }, failure.Field, localizer),
        _ => ProblemDetailsAssembler.ToActionResult(controller, "unexpected", "", localizer)};
}
