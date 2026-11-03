using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Business.Domain.Model.Aggregates;
public sealed partial class StaffInvitation : AuditedEntity
{
    private StaffInvitation()
    {
    }

    public StaffInvitation(string businessId, string name, string email, DateTimeOffset now)
    {
        BusinessId = int.TryParse(businessId, out var key) && key > 0 ? key : throw new DomainException("validation", "businessId");
        Update(name, email);
        LastRequestedAt = now;
    }

    public int BusinessId { get; private set; }
    public string DisplayName { get; private set; } = "";
    public string Email { get; private set; } = "";
    public string Status { get; private set; } = "pending";
    public int DeliverySequence { get; private set; } = 1;
    public DateTimeOffset LastRequestedAt { get; private set; }

    public void Update(string name, string email)
    {
        Guard.Require(Status == "pending", "transition");
        var n = Guard.Text(name, 100, "displayName");
        var e = Guard.Email(email);
        DisplayName = n;
        Email = e;
    }

    public void Cancel()
    {
        Guard.Require(Status == "pending", "transition");
        Status = "cancelled";
    }

    public void Resend(DateTimeOffset now)
    {
        Guard.Require(Status == "pending", "transition");
        LastRequestedAt = now;
        DeliverySequence++;
    }

    public void Accept()
    {
        Guard.Require(Status == "pending", "transition");
        Status = "accepted";
    }
}
