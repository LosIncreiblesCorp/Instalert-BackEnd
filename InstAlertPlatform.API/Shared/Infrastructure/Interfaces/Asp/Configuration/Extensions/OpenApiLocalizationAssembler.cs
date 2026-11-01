using InstAlertPlatform.API.Resources;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
public static class OpenApiLocalizationAssembler
{
    public static Task TransformOperationAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, IReadOnlyDictionary<string, string> operations, CancellationToken cancellationToken)
    {
        if (context.Description.ActionDescriptor is not ControllerActionDescriptor action || !operations.TryGetValue(action.ControllerName + "." + action.ActionName, out var key))
            return Task.CompletedTask;
        var localizer = context.ApplicationServices.GetRequiredService<IStringLocalizer<SharedResource>>();
        operation.OperationId = action.ControllerName + "_" + action.ActionName;
        operation.Summary = localizer[key + "_Summary"].Value;
        operation.Description = localizer[key + "_Description"].Value;
        if (operation.Responses is not null)
            foreach (var(status, response)in operation.Responses)
                if (response is OpenApiResponse concrete)
                    concrete.Description = localizer["Response" + status].Value;
        return Task.CompletedTask;
    }

    public static Task TransformSchemaAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        if (context.JsonPropertyInfo?.Name is not string name)
            return Task.CompletedTask;
        if (context.JsonPropertyInfo.AttributeProvider is PropertyInfo property)
        {
            var parameter = property.DeclaringType?.GetConstructors()
                .SelectMany(constructor => constructor.GetParameters())
                .FirstOrDefault(candidate => string.Equals(candidate.Name, property.Name, StringComparison.OrdinalIgnoreCase));
            foreach (var attribute in parameter?.GetCustomAttributes<ValidationAttribute>() ?? [])
            {
                switch (attribute)
                {
                    case StringLengthAttribute length:
                        schema.MaxLength = length.MaximumLength;
                        if (length.MinimumLength > 0) schema.MinLength = length.MinimumLength;
                        break;
                    case RegularExpressionAttribute expression:
                        schema.Pattern = expression.Pattern;
                        break;
                    case RangeAttribute range:
                        schema.Minimum = Convert.ToString(range.Minimum, CultureInfo.InvariantCulture);
                        schema.Maximum = Convert.ToString(range.Maximum, CultureInfo.InvariantCulture);
                        break;
                    case RequiredAttribute:
                        if (schema.Type.HasValue) schema.Type &= ~JsonSchemaType.Null;
                        break;
                }
            }
        }
        var localizer = context.ApplicationServices.GetRequiredService<IStringLocalizer<SharedResource>>();
        var description = localizer["Field_" + name];
        if (!description.ResourceNotFound)
            schema.Description = description.Value;
        return Task.CompletedTask;
    }
}
