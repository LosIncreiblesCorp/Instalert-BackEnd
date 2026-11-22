namespace InstAlertPlatform.API.Contacts.Application.Services.Snapshots;
public sealed record ContactSnapshot(string Id, string EmployeeId, string FullName, string Relationship, string Phone, string? Email, string Notes, string Version);
