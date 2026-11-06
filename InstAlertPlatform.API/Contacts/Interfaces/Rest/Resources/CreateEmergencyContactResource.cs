namespace InstAlertPlatform.API.Contacts.Interfaces.Rest.Resources;
public sealed record CreateEmergencyContactResource(string FullName, string Relationship, string Phone, string? Email, string? Notes);
