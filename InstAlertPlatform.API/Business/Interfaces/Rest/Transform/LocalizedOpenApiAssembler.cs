using InstAlertPlatform.API.Business.Interfaces.Rest.Resources;
using InstAlertPlatform.API.Resources;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi;
using System.Reflection;

namespace InstAlertPlatform.API.Business.Interfaces.Rest.Transform;

public static class LocalizedOpenApiAssembler
{
    private static readonly IReadOnlyDictionary<string, string> ResponseResources = new Dictionary<string, string>
    {
        ["200"] = "BusinessResponseSuccess",
        ["201"] = "BusinessResponseCreated",
        ["400"] = "BusinessResponseInvalid",
        ["401"] = "BusinessUnauthenticated",
        ["403"] = "BusinessForbidden",
        ["404"] = "BusinessNotFound",
        ["409"] = "BusinessResponseConflict",
        ["500"] = "UnexpectedErrorProcessingRequest"
    };

    public static Task TransformOperationAsync(OpenApiOperation operation,
        OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        if (context.Description.ActionDescriptor is not ControllerActionDescriptor action ||
            action.ControllerTypeInfo.AsType() != typeof(BusinessesController))
            return Task.CompletedTask;

        var localizer = context.ApplicationServices.GetRequiredService<IStringLocalizer<SharedResource>>();
        operation.OperationId = action.ActionName;
        operation.Summary = localizer[action.ActionName + "Summary"].Value;
        operation.Description = localizer[action.ActionName + "Description"].Value;
        if (operation.Parameters is not null)
            foreach (var parameter in operation.Parameters)
                if (parameter is OpenApiParameter header && header.Name == "User-Id")
                    header.Description = localizer["BusinessCallerHeader"].Value;

        if (operation.Responses is not null)
            foreach (var (status, key) in ResponseResources)
                if (operation.Responses.TryGetValue(status, out var response) && response is OpenApiResponse value)
                    value.Description = localizer[key].Value;

        return Task.CompletedTask;
    }

    public static Task TransformSchemaAsync(OpenApiSchema schema,
        OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        var localizer = context.ApplicationServices.GetRequiredService<IStringLocalizer<SharedResource>>();
        var type = context.JsonTypeInfo.Type;
        var declaringType = (context.JsonPropertyInfo?.AttributeProvider as MemberInfo)?.DeclaringType;
        if (!IsBusinessResource(type) && !IsBusinessResource(declaringType)) return Task.CompletedTask;
        if (type == typeof(BusinessResource)) schema.Description = localizer["BusinessResourceDescription"].Value;
        if (type == typeof(CreateBusinessResource)) schema.Description = localizer["CreateBusinessResourceDescription"].Value;
        if (type == typeof(UpdateBusinessResource)) schema.Description = localizer["UpdateBusinessResourceDescription"].Value;

        var key = context.JsonPropertyInfo?.Name switch
        {
            "id" => "BusinessIdDescription",
            "taxId" => "BusinessTaxIdDescription",
            "name" => "BusinessNameDescription",
            "address" => "BusinessAddressDescription",
            "latitude" => "BusinessLatitudeDescription",
            "longitude" => "BusinessLongitudeDescription",
            "createdAt" => "BusinessCreatedAtDescription",
            "updatedAt" => "BusinessUpdatedAtDescription",
            _ => null
        };
        if (key is not null) schema.Description = localizer[key].Value;
        return Task.CompletedTask;
    }

    private static bool IsBusinessResource(Type? type) => type == typeof(BusinessResource) ||
        type == typeof(CreateBusinessResource) || type == typeof(UpdateBusinessResource);
}
