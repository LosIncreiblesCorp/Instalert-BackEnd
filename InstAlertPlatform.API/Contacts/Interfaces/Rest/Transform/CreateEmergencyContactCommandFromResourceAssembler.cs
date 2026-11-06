using InstAlertPlatform.API.Contacts.Domain.Model.Commands;
using InstAlertPlatform.API.Contacts.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Contacts.Interfaces.Rest.Transform;
public static class CreateEmergencyContactCommandFromResourceAssembler
{
    public static CreateEmergencyContactCommand ToCommandFromResource(CreateEmergencyContactResource resource) => new(resource.FullName, resource.Relationship, resource.Phone, resource.Email, resource.Notes);
}
