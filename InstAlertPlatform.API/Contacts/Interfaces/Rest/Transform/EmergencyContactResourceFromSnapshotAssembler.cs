using InstAlertPlatform.API.Contacts.Application.Services;
using InstAlertPlatform.API.Contacts.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Contacts.Interfaces.Rest.Transform;
public static class EmergencyContactResourceFromSnapshotAssembler
{
    public static EmergencyContactResource ToResourceFromSnapshot(ContactSnapshot snapshot) => new(snapshot.Id, snapshot.EmployeeId, snapshot.FullName, snapshot.Relationship, snapshot.Phone, snapshot.Email, snapshot.Notes, snapshot.Version);
}
