using System.Text.Json;

namespace InstAlertPlatform.API.Shared.Domain.Model;
public sealed partial class OutboxMessage : AuditedEntity
{
    private OutboxMessage()
    {
    }

    public OutboxMessage(string eventType, object payload)
    {
        EventType = eventType;
        Payload = JsonSerializer.Serialize(payload);
    }

    public string EventType { get; private set; } = "";
    public string Payload { get; private set; } = "";
    public DateTimeOffset? ProcessedAt { get; private set; }
    public DateTimeOffset? LeaseUntil { get; private set; }
    public int Attempts { get; private set; }

    public void Claim(DateTimeOffset now)
    {
        LeaseUntil = now.AddMinutes(2);
        Attempts++;
    }

    public void Complete(DateTimeOffset now)
    {
        ProcessedAt = now;
        LeaseUntil = null;
    }

    public void Retry(DateTimeOffset now) => LeaseUntil = now.AddSeconds(Math.Min(300, Math.Pow(2, Math.Min(Attempts, 8))));
}
