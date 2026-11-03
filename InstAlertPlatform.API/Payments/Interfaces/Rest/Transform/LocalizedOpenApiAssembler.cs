using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace InstAlertPlatform.API.Payments.Interfaces.Rest.Transform;
public static class LocalizedOpenApiAssembler
{
    private static readonly IReadOnlyDictionary<string, string> Operations = new Dictionary<string, string>
    {
        ["Plans.Get"] = "Plans_Get",
        ["Subscriptions.Get"] = "Subscriptions_Get",
        ["Subscriptions.Begin"] = "Subscriptions_Begin",
        ["Subscriptions.Verify"] = "Subscriptions_Verify",
        ["Subscriptions.Cancel"] = "Subscriptions_Cancel",
        ["Subscriptions.History"] = "Subscriptions_History",
        ["Subscriptions.Change"] = "Subscriptions_Change",
        ["Subscriptions.VerifyChange"] = "Subscriptions_VerifyChange",
        ["PayPalWebhook.Receive"] = "PayPalWebhook_Receive",
    };
    public static Task TransformOperationAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken) => OpenApiLocalizationAssembler.TransformOperationAsync(operation, context, Operations, cancellationToken);
}
