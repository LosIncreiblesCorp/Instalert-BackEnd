namespace InstAlertPlatform.API.Contacts.Domain.Model.Commands;
public sealed record CreateEmergencyContactCommand(string FullName, string Relationship, string Phone, string? Email, string? Notes);
