namespace InstAlertPlatform.API.Contacts.Interfaces.Rest.Resources;
public sealed record UpdateEmergencyContactResource(string FullName, string Relationship, string Phone, string? Email, string? Notes);
