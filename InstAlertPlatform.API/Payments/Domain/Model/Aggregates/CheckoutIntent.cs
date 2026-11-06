using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Payments.Domain.Model.Aggregates;
public sealed partial class CheckoutIntent : AuditedEntity
{
    private CheckoutIntent()
    {
    }

    public CheckoutIntent(string businessId, string planId, DateTimeOffset now)
    {
        BusinessId = businessId;
        PlanId = planId;
        ExpiresAt = now.AddHours(1);
    }

    public string BusinessId { get; private set; } = "";
    public string PlanId { get; private set; } = "";
    public DateTimeOffset ExpiresAt { get; private set; }
    public bool Completed { get; private set; }

    public void Complete(DateTimeOffset now)
    {
        Guard.Require(now < ExpiresAt && !Completed, "transition");
        Completed = true;
    }
}
