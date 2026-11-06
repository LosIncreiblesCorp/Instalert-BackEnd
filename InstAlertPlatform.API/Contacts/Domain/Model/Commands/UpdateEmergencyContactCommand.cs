namespace InstAlertPlatform.API.Contacts.Domain.Model.Commands;
public sealed record UpdateEmergencyContactCommand(string Id, string FullName, string Relationship, string Phone, string? Email, string? Notes);
