using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Notifications.Domain.Model.Aggregates;
public sealed partial class DeliveryRequest : AuditedEntity
{
    private DeliveryRequest()
    {
    }

    public DeliveryRequest(string id, string sourceId, string channel, string destination, string userId)
    {
        Id = id;
        SourceAlertId = sourceId;
        Channel = channel;
        Destination = destination;
        UserId = userId;
    }

    public static DeliveryRequest Invitation(string id, string invitationId, string email, string ownerUserId) => new(id, "", "email", email, ownerUserId)
    {
        SourceInvitationId = invitationId
    };
    public string? SourceInvitationId { get; private set; }
    public string SourceAlertId { get; private set; } = "";
    public string Channel { get; private set; } = "";
    public string Destination { get; private set; } = "";
    public string UserId { get; private set; } = "";
    public string Status { get; private set; } = "queued";
    public int Attempts { get; private set; }
    public DateTimeOffset? SentAt { get; private set; }
    public DateTimeOffset? LeaseUntil { get; private set; }

    public void Claim(DateTimeOffset now)
    {
        Status = "processing";
        LeaseUntil = now.AddMinutes(2);
        Attempts++;
    }

    public void Sent(DateTimeOffset now)
    {
        Status = "sent";
        SentAt = now;
        LeaseUntil = null;
    }

    public void Cancel()
    {
        Status = "cancelled";
        LeaseUntil = null;
    }

    public void Failed(DateTimeOffset now)
    {
        Status = Attempts >= 5 ? "failed" : "queued";
        LeaseUntil = now.AddSeconds(Math.Pow(2, Attempts) * 10);
    }
}
