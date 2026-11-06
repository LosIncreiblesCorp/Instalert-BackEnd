using InstAlertPlatform.API.Contacts.Domain.Model.Commands;
using InstAlertPlatform.API.Contacts.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Contacts.Interfaces.Rest.Transform;
public static class UpdateEmergencyContactCommandFromResourceAssembler
{
    public static UpdateEmergencyContactCommand ToCommandFromResource(UpdateEmergencyContactResource resource, string id) => new(id, resource.FullName, resource.Relationship, resource.Phone, resource.Email, resource.Notes);
}
