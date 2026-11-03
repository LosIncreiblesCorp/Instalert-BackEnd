using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Business.Domain.Model.Aggregates;
public sealed partial class BusinessMember : AuditedEntity
{
    private BusinessMember()
    {
    }

    public BusinessMember(string businessId, string userId, string displayName, string email)
    {
        BusinessId = int.TryParse(businessId, out var key) && key > 0 ? key : throw new DomainException("validation", "businessId");
        UserId = userId;
        Update(displayName, email);
    }

    public int BusinessId { get; private set; }
    public string UserId { get; private set; } = "";
    public string DisplayName { get; private set; } = "";
    public string Email { get; private set; } = "";
    public string Role { get; private set; } = "Operative";
    public string Status { get; private set; } = "active";

    public void Update(string displayName, string email)
    {
        var n = Guard.Text(displayName, 100, "displayName");
        var e = Guard.Email(email);
        DisplayName = n;
        Email = e;
    }

    public void SetActive(bool active) => Status = active ? "active" : "inactive";
}
