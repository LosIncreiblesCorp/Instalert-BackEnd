using InstAlertPlatform.API.Profiles.Domain.Model.ValueObjects;
using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Profiles.Domain.Model.Aggregates;
public sealed partial class UserProfile : AuditedEntity
{
    private UserProfile()
    {
    }

    public UserProfile(string userId, string firstName, string lastName, string dni, string? phone)
    {
        Id = Guard.Text(userId, 80, "userId");
        Update(firstName, lastName, dni, phone);
    }

    public string FirstName { get; private set; } = "";
    public string LastName { get; private set; } = "";
    public NationalIdentityNumber Dni { get; private set; } = null !;
    public string? Phone { get; private set; }

    public void Update(string firstName, string lastName, string dni, string? phone)
    {
        var first = Guard.Text(firstName, 100, "firstName");
        var last = Guard.Text(lastName, 100, "lastName");
        var document = new NationalIdentityNumber(dni);
        var contact = Guard.Phone(phone);
        FirstName = first;
        LastName = last;
        Dni = document;
        Phone = contact.Length == 0 ? null : contact;
    }

}
