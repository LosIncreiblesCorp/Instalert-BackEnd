using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace InstAlertPlatform.API.Mapping.Interfaces.Rest.Transform;
public static class LocalizedOpenApiAssembler
{
    private static readonly IReadOnlyDictionary<string, string> Operations = new Dictionary<string, string>
    {
        ["Mapping.ActiveAlerts"] = "Mapping_ActiveAlerts",
        ["Mapping.Incidents"] = "Mapping_Incidents",
        ["Mapping.Incident"] = "Mapping_Incident",
        ["Mapping.Businesses"] = "Mapping_Businesses",
        ["Mapping.Zones"] = "Mapping_Zones",
        ["Mapping.Zone"] = "Mapping_Zone",
    };
    public static Task TransformOperationAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken) => OpenApiLocalizationAssembler.TransformOperationAsync(operation, context, Operations, cancellationToken);
}
