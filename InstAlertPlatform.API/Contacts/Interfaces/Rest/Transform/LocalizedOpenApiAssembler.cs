using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace InstAlertPlatform.API.Contacts.Interfaces.Rest.Transform;
public static class LocalizedOpenApiAssembler
{
    private static readonly IReadOnlyDictionary<string, string> Operations = new Dictionary<string, string>
    {
        ["EmergencyContacts.List"] = "EmergencyContacts_List",
        ["EmergencyContacts.Get"] = "EmergencyContacts_Get",
        ["EmergencyContacts.Create"] = "EmergencyContacts_Create",
        ["EmergencyContacts.Update"] = "EmergencyContacts_Update",
        ["EmergencyContacts.Delete"] = "EmergencyContacts_Delete",
    };
    public static Task TransformOperationAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken) => OpenApiLocalizationAssembler.TransformOperationAsync(operation, context, Operations, cancellationToken);
}
