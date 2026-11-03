using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Payments.Domain.Model.Aggregates;
public sealed partial class Subscription : AuditedEntity
{
    private Subscription()
    {
    }

    public Subscription(string businessId, string planId, string providerId)
    {
        BusinessId = businessId;
        PlanId = planId;
        ProviderSubscriptionId = Guard.Text(providerId, 100, "subscriptionId");
    }

    public string BusinessId { get; private set; } = "";
    public string PlanId { get; private set; } = "";
    public string ProviderSubscriptionId { get; private set; } = "";
    public string Status { get; private set; } = "pending";
    public DateTimeOffset? CurrentPeriodEnd { get; private set; }
    public bool CancelAtPeriodEnd { get; private set; }

    public void Confirm(string planId, DateTimeOffset paidUntil, DateTimeOffset now)
    {
        Guard.Require(paidUntil > now, "validation", "period");
        PlanId = planId;
        Status = "active";
        CurrentPeriodEnd = paidUntil;
    }

    public void ScheduleCancellation()
    {
        Guard.Require(Status == "active", "transition");
        CancelAtPeriodEnd = true;
    }

    public void ReplaceDevelopmentSubscription(string planId, string providerId, DateTimeOffset paidUntil, DateTimeOffset now)
    {
        Guard.Require(ProviderSubscriptionId.StartsWith("synthetic-development-", StringComparison.Ordinal), "transition");
        var replacement = Guard.Text(providerId, 100, "subscriptionId");
        Guard.Require(!replacement.StartsWith("synthetic-", StringComparison.Ordinal), "validation", "subscriptionId");
        Confirm(planId, paidUntil, now);
        ProviderSubscriptionId = replacement;
        CancelAtPeriodEnd = false;
    }

    public bool IsEntitled(DateTimeOffset now) => Status == "active" && CurrentPeriodEnd > now;
}
