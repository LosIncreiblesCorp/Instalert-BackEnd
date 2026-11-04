using InstAlertPlatform.API.Business.Application.Errors;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Business.Interfaces.Rest.Transform;
public static class ActionResultFromInviteStaffMemberResultAssembler
{
    public static IActionResult ToActionResultFromInviteStaffMemberResult(Result<InvitationSnapshot, EInviteStaffMemberError> result, ControllerBase controller, IStringLocalizer<SharedResource> localizer, Func<InvitationSnapshot, IActionResult> onSuccess) => result switch
    {
        Result<InvitationSnapshot, EInviteStaffMemberError>.Success success => onSuccess(success.Value),
        Result<InvitationSnapshot, EInviteStaffMemberError>.Failure failure => ProblemDetailsAssembler.ToActionResult(controller, failure.Error switch
        {
            EInviteStaffMemberError.InvalidInput => "validation",
            EInviteStaffMemberError.Unauthenticated => "unauthenticated",
            EInviteStaffMemberError.Forbidden => "forbidden",
            EInviteStaffMemberError.NotFound => "not_found",
            EInviteStaffMemberError.Conflict => "conflict",
            EInviteStaffMemberError.ConcurrencyConflict => "concurrency",
            EInviteStaffMemberError.CapacityExceeded => "capacity",
            EInviteStaffMemberError.InvalidTransition => "transition",
            EInviteStaffMemberError.ProviderUnavailable => "provider_unavailable",
            EInviteStaffMemberError.IdentityUnavailable => "identity_unavailable",
            EInviteStaffMemberError.UnexpectedError => "unexpected",
            _ => "unexpected"
        }, failure.Field, localizer),
        _ => ProblemDetailsAssembler.ToActionResult(controller, "unexpected", "", localizer)};
}
