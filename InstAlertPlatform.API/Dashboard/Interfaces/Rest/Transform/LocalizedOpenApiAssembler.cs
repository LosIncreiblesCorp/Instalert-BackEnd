using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace InstAlertPlatform.API.Dashboard.Interfaces.Rest.Transform;
public static class LocalizedOpenApiAssembler
{
    private static readonly IReadOnlyDictionary<string, string> Operations = new Dictionary<string, string>
    {
        ["Dashboard.Current"] = "Dashboard_Current",
        ["Dashboard.List"] = "Dashboard_List",
        ["Dashboard.Get"] = "Dashboard_Get",
        ["Dashboard.Generate"] = "Dashboard_Generate",
    };
    public static Task TransformOperationAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken) => OpenApiLocalizationAssembler.TransformOperationAsync(operation, context, Operations, cancellationToken);
}
