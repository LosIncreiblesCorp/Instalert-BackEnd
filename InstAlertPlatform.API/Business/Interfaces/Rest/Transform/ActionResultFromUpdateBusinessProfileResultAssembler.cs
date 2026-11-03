using InstAlertPlatform.API.Business.Application.Errors;
using InstAlertPlatform.API.Business.Domain.Model.Aggregates;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Business.Interfaces.Rest.Transform;

public static class ActionResultFromUpdateBusinessProfileResultAssembler
{
    public static ActionResult ToActionResultFromUpdateBusinessProfileResult(
        Result<BusinessProfile, EUpdateBusinessProfileError> result, ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) => result switch
    {
        Result<BusinessProfile, EUpdateBusinessProfileError>.Success success =>
            controller.Ok(BusinessResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
        Result<BusinessProfile, EUpdateBusinessProfileError>.Failure failure => failure.Error switch
        {
            EUpdateBusinessProfileError.InvalidDetails => ToProblem(controller, localizer, 400, "validation", "BusinessDetailsInvalid"),
            EUpdateBusinessProfileError.NotFound => ToProblem(controller, localizer, 404, "not_found", "BusinessNotFound"),
            EUpdateBusinessProfileError.Forbidden => ToProblem(controller, localizer, 403, "forbidden", "BusinessForbidden"),
            EUpdateBusinessProfileError.TaxIdentifierReadOnly => ToProblem(controller, localizer, 409, "tax_id_read_only", "BusinessTaxIdReadOnly"),
            _ => ToProblem(controller, localizer, 500, "unexpected", "UnexpectedErrorProcessingRequest")
        },
        _ => ToProblem(controller, localizer, 500, "unexpected", "UnexpectedErrorProcessingRequest")
    };

    private static ObjectResult ToProblem(ControllerBase controller, IStringLocalizer<SharedResource> localizer,
        int status, string code, string message)
    {
        var result = controller.Problem(statusCode: status, title: localizer["BusinessRequestFailed"].Value,
            detail: localizer[message].Value);
        if (result.Value is ProblemDetails problem) problem.Extensions["code"] = code;
        return result;
    }
}
