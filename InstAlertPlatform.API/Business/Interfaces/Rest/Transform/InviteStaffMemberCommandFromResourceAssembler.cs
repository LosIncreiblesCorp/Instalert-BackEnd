using InstAlertPlatform.API.Business.Domain.Model.Commands;
using InstAlertPlatform.API.Business.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Business.Interfaces.Rest.Transform;
public static class InviteStaffMemberCommandFromResourceAssembler
{
    public static InviteStaffMemberCommand ToCommandFromResource(InviteStaffMemberResource resource, string id) => new(id, resource.DisplayName, resource.Email);
}
