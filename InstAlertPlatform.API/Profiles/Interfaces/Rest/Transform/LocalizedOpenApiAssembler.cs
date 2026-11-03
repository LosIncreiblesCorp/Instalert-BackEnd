using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace InstAlertPlatform.API.Profiles.Interfaces.Rest.Transform;
public static class LocalizedOpenApiAssembler
{
    private static readonly IReadOnlyDictionary<string, string> Operations = new Dictionary<string, string>
    {
        ["Profiles.Get"] = "Profiles_Get",
        ["Profiles.Update"] = "Profiles_Update",
        ["Profiles.Preferences"] = "Profiles_Preferences",
        ["Profiles.Locale"] = "Profiles_Locale",
    };
    public static Task TransformOperationAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken) => OpenApiLocalizationAssembler.TransformOperationAsync(operation, context, Operations, cancellationToken);
}
