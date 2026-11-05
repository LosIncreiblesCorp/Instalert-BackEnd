using InstAlertPlatform.API.Business.Application.Errors;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Business.Interfaces.Rest.Transform;
public static class ActionResultFromUpdateBusinessMemberResultAssembler
{
    public static IActionResult ToActionResultFromUpdateBusinessMemberResult(Result<MemberSnapshot, EUpdateBusinessMemberError> result, ControllerBase controller, IStringLocalizer<SharedResource> localizer, Func<MemberSnapshot, IActionResult> onSuccess) => result switch
    {
        Result<MemberSnapshot, EUpdateBusinessMemberError>.Success success => onSuccess(success.Value),
        Result<MemberSnapshot, EUpdateBusinessMemberError>.Failure failure => ProblemDetailsAssembler.ToActionResult(controller, failure.Error switch
        {
            EUpdateBusinessMemberError.InvalidInput => "validation",
            EUpdateBusinessMemberError.Unauthenticated => "unauthenticated",
            EUpdateBusinessMemberError.Forbidden => "forbidden",
            EUpdateBusinessMemberError.NotFound => "not_found",
            EUpdateBusinessMemberError.Conflict => "conflict",
            EUpdateBusinessMemberError.ConcurrencyConflict => "concurrency",
            EUpdateBusinessMemberError.CapacityExceeded => "capacity",
            EUpdateBusinessMemberError.InvalidTransition => "transition",
            EUpdateBusinessMemberError.ProviderUnavailable => "provider_unavailable",
            EUpdateBusinessMemberError.IdentityUnavailable => "identity_unavailable",
            EUpdateBusinessMemberError.UnexpectedError => "unexpected",
            _ => "unexpected"
        }, failure.Field, localizer),
        _ => ProblemDetailsAssembler.ToActionResult(controller, "unexpected", "", localizer)};
}
