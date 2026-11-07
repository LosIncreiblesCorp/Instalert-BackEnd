using System.Security.Cryptography;
using System.Text;
using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Notifications.Domain.Model.Aggregates;
public sealed partial class ReceivedNotification : AuditedEntity
{
    private ReceivedNotification()
    {
    }

    public ReceivedNotification(string sourceAlertId, string userId, string businessId, double distance, DateTimeOffset now)
    {
        Id = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(sourceAlertId + "|" + userId + "|" + businessId)));
        SourceAlertId = sourceAlertId;
        RecipientUserId = userId;
        TargetBusinessId = businessId;
        DistanceMeters = distance;
        CreatedAt = now;
    }

    public string SourceAlertId { get; private set; } = "";
    public string RecipientUserId { get; private set; } = "";
    public string TargetBusinessId { get; private set; } = "";
    public string Type { get; private set; } = "nearby-panic";
    public string Title { get; private set; } = "Nearby emergency";
    public string Message { get; private set; } = "An active panic alert was reported near your business.";
    public double DistanceMeters { get; private set; }
    public DateTimeOffset? ReadAt { get; private set; }

    public void Read(DateTimeOffset now) => ReadAt ??= now;
}
