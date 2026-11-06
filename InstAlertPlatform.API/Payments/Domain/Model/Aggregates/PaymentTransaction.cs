using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Payments.Domain.Model.Aggregates;
public sealed partial class PaymentTransaction : AuditedEntity
{
    private PaymentTransaction()
    {
    }

    public PaymentTransaction(string businessId, string providerTransactionId, decimal amount, string currency, DateTimeOffset paidAt)
    {
        BusinessId = businessId;
        ProviderTransactionId = providerTransactionId;
        Amount = amount;
        Currency = currency;
        PaidAt = paidAt;
    }

    public string BusinessId { get; private set; } = "";
    public string ProviderTransactionId { get; private set; } = "";
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "USD";
    public DateTimeOffset PaidAt { get; private set; }
}
