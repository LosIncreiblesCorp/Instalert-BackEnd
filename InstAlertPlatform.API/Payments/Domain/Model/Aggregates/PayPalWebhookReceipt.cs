using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Payments.Domain.Model.Aggregates;
public sealed partial class PayPalWebhookReceipt : AuditedEntity
{
    private PayPalWebhookReceipt()
    {
    }

    public PayPalWebhookReceipt(string eventId, string eventType)
    {
        Id = Guard.Text(eventId, 160, "eventId");
        EventType = Guard.Text(eventType, 100, "eventType");
    }

    public string EventType { get; private set; } = "";
}
