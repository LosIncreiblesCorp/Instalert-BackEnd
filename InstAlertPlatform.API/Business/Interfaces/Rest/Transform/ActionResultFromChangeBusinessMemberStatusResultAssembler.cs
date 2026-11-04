using InstAlertPlatform.API.Business.Application.Errors;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Business.Interfaces.Rest.Transform;
public static class ActionResultFromChangeBusinessMemberStatusResultAssembler
{
    public static IActionResult ToActionResultFromChangeBusinessMemberStatusResult(Result<MemberSnapshot, EChangeBusinessMemberStatusError> result, ControllerBase controller, IStringLocalizer<SharedResource> localizer, Func<MemberSnapshot, IActionResult> onSuccess) => result switch
    {
        Result<MemberSnapshot, EChangeBusinessMemberStatusError>.Success success => onSuccess(success.Value),
        Result<MemberSnapshot, EChangeBusinessMemberStatusError>.Failure failure => ProblemDetailsAssembler.ToActionResult(controller, failure.Error switch
        {
            EChangeBusinessMemberStatusError.InvalidInput => "validation",
            EChangeBusinessMemberStatusError.Unauthenticated => "unauthenticated",
            EChangeBusinessMemberStatusError.Forbidden => "forbidden",
            EChangeBusinessMemberStatusError.NotFound => "not_found",
            EChangeBusinessMemberStatusError.Conflict => "conflict",
            EChangeBusinessMemberStatusError.ConcurrencyConflict => "concurrency",
            EChangeBusinessMemberStatusError.CapacityExceeded => "capacity",
            EChangeBusinessMemberStatusError.InvalidTransition => "transition",
            EChangeBusinessMemberStatusError.ProviderUnavailable => "provider_unavailable",
            EChangeBusinessMemberStatusError.IdentityUnavailable => "identity_unavailable",
            EChangeBusinessMemberStatusError.UnexpectedError => "unexpected",
            _ => "unexpected"
        }, failure.Field, localizer),
        _ => ProblemDetailsAssembler.ToActionResult(controller, "unexpected", "", localizer)};
}
