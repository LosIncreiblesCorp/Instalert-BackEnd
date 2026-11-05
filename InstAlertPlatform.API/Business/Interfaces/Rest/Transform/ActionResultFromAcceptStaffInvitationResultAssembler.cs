using InstAlertPlatform.API.Business.Application.Errors;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Business.Interfaces.Rest.Transform;
public static class ActionResultFromAcceptStaffInvitationResultAssembler
{
    public static IActionResult ToActionResultFromAcceptStaffInvitationResult(Result<MemberSnapshot, EAcceptStaffInvitationError> result, ControllerBase controller, IStringLocalizer<SharedResource> localizer, Func<MemberSnapshot, IActionResult> onSuccess) => result switch
    {
        Result<MemberSnapshot, EAcceptStaffInvitationError>.Success success => onSuccess(success.Value),
        Result<MemberSnapshot, EAcceptStaffInvitationError>.Failure failure => ProblemDetailsAssembler.ToActionResult(controller, failure.Error switch
        {
            EAcceptStaffInvitationError.InvalidInput => "validation",
            EAcceptStaffInvitationError.Unauthenticated => "unauthenticated",
            EAcceptStaffInvitationError.Forbidden => "forbidden",
            EAcceptStaffInvitationError.NotFound => "not_found",
            EAcceptStaffInvitationError.Conflict => "conflict",
            EAcceptStaffInvitationError.ConcurrencyConflict => "concurrency",
            EAcceptStaffInvitationError.CapacityExceeded => "capacity",
            EAcceptStaffInvitationError.InvalidTransition => "transition",
            EAcceptStaffInvitationError.ProviderUnavailable => "provider_unavailable",
            EAcceptStaffInvitationError.IdentityUnavailable => "identity_unavailable",
            EAcceptStaffInvitationError.UnexpectedError => "unexpected",
            _ => "unexpected"
        }, failure.Field, localizer),
        _ => ProblemDetailsAssembler.ToActionResult(controller, "unexpected", "", localizer)};
}
