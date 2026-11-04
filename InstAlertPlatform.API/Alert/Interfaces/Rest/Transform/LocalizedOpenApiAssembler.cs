using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace InstAlertPlatform.API.Alert.Interfaces.Rest.Transform;
public static class LocalizedOpenApiAssembler
{
    private static readonly IReadOnlyDictionary<string, string> Operations = new Dictionary<string, string>
    {
        ["Alerts.List"] = "Alerts_List",
        ["Alerts.Get"] = "Alerts_Get",
        ["Alerts.Prepare"] = "Alerts_Prepare",
        ["Alerts.Confirm"] = "Alerts_Confirm",
        ["Alerts.Report"] = "Alerts_Report",
        ["Alerts.Location"] = "Alerts_Location",
        ["Alerts.Resolve"] = "Alerts_Resolve",
        ["Alerts.Draft"] = "Alerts_Draft",
        ["Alerts.Complete"] = "Alerts_Complete",
        ["Alerts.Preferences"] = "Alerts_Preferences",
        ["Alerts.ChangePreferences"] = "Alerts_ChangePreferences",
    };
    public static Task TransformOperationAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken) => OpenApiLocalizationAssembler.TransformOperationAsync(operation, context, Operations, cancellationToken);
}
