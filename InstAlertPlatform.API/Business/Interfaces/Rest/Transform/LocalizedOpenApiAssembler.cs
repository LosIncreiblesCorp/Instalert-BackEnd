using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace InstAlertPlatform.API.Business.Interfaces.Rest.Transform;
public static class LocalizedOpenApiAssembler
{
    private static readonly IReadOnlyDictionary<string, string> Operations = new Dictionary<string, string>
    {
        ["Businesses.List"] = "Businesses_List",
        ["Businesses.Get"] = "Businesses_Get",
        ["Businesses.Create"] = "Businesses_Create",
        ["Businesses.Update"] = "Businesses_Update",
        ["Businesses.Seats"] = "Businesses_Seats",
        ["Businesses.Members"] = "Businesses_Members",
        ["Businesses.Invitations"] = "Businesses_Invitations",
        ["Businesses.Invite"] = "Businesses_Invite",
        ["BusinessMembers.OwnMemberships"] = "BusinessMembers_OwnMemberships",
        ["BusinessMembers.Update"] = "BusinessMembers_Update",
        ["BusinessMembers.Activate"] = "BusinessMembers_Activate",
        ["BusinessMembers.Deactivate"] = "BusinessMembers_Deactivate",
        ["StaffInvitations.Update"] = "StaffInvitations_Update",
        ["StaffInvitations.Cancel"] = "StaffInvitations_Cancel",
        ["StaffInvitations.Resend"] = "StaffInvitations_Resend",
        ["StaffInvitations.Accept"] = "StaffInvitations_Accept",
    };
    public static Task TransformOperationAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken) => OpenApiLocalizationAssembler.TransformOperationAsync(operation, context, Operations, cancellationToken);
}
