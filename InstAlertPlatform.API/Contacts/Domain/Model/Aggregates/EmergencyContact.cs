using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Contacts.Domain.Model.Aggregates;
public sealed partial class EmergencyContact : AuditedEntity
{
    private EmergencyContact()
    {
    }

    public EmergencyContact(string ownerId, string name, string relationship, string phone, string? email, string? notes)
    {
        EmployeeId = Guard.Text(ownerId, 80, "employeeId");
        Update(name, relationship, phone, email, notes);
    }

    public string EmployeeId { get; private set; } = "";
    public string FullName { get; private set; } = "";
    public string Relationship { get; private set; } = "";
    public string Phone { get; private set; } = "";
    public string? Email { get; private set; }
    public string Notes { get; private set; } = "";

    public void Update(string name, string relationship, string phone, string? email, string? notes)
    {
        var n = Guard.Text(name, 100, "fullName");
        var rel = Guard.Text(relationship, 60, "relationship");
        var ph = Guard.Phone(phone, true);
        var mail = string.IsNullOrWhiteSpace(email) ? null : Guard.Email(email);
        var text = notes?.Trim() ?? "";
        Guard.Require(text.Length <= 140, "validation", "notes");
        FullName = n;
        Relationship = rel;
        Phone = ph;
        Email = mail;
        Notes = text;
    }
}
