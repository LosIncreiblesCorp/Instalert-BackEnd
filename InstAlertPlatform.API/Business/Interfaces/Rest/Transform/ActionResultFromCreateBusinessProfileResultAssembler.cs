using InstAlertPlatform.API.Business.Application.Errors;
using InstAlertPlatform.API.Business.Domain.Model.Aggregates;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Business.Interfaces.Rest.Transform;

public static class ActionResultFromCreateBusinessProfileResultAssembler
{
    public static ActionResult ToActionResultFromCreateBusinessProfileResult(
        Result<BusinessProfile, ECreateBusinessProfileError> result, ControllerBase controller,
        IStringLocalizer<SharedResource> localizer, string getByIdActionName) => result switch
    {
        Result<BusinessProfile, ECreateBusinessProfileError>.Success success =>
            controller.CreatedAtAction(getByIdActionName, new { id = success.Value.Id },
                BusinessResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
        Result<BusinessProfile, ECreateBusinessProfileError>.Failure failure => failure.Error switch
        {
            ECreateBusinessProfileError.InvalidDetails => ToProblem(controller, localizer, 400, "validation", "BusinessDetailsInvalid"),
            ECreateBusinessProfileError.DuplicateTaxIdentifier => ToProblem(controller, localizer, 409, "duplicate_tax_id", "BusinessTaxIdDuplicate"),
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
