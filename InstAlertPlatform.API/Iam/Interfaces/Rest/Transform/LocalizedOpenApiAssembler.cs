using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace InstAlertPlatform.API.Iam.Interfaces.Rest.Transform;
public static class LocalizedOpenApiAssembler
{
    private static readonly IReadOnlyDictionary<string, string> Operations = new Dictionary<string, string>
    {
        ["Users.Register"] = "Identity_Register",
        ["Users.ChangePassword"] = "Identity_ChangePassword",
        ["Tokens.SignIn"] = "Identity_SignIn",
        ["Tokens.SignOut"] = "Identity_SignOut",
        ["Sessions.GetCurrent"] = "Identity_Current"
    };
    public static Task TransformOperationAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken) =>
        OpenApiLocalizationAssembler.TransformOperationAsync(operation, context, Operations, cancellationToken);
}
