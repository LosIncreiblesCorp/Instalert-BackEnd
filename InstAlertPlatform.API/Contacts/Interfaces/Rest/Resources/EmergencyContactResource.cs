namespace InstAlertPlatform.API.Contacts.Interfaces.Rest.Resources;
public sealed record EmergencyContactResource(string Id, string EmployeeId, string FullName, string Relationship, string Phone, string? Email, string Notes, string Version);
