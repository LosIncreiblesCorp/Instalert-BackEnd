using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Payments.Domain.Model.Aggregates;
public sealed partial class Plan : AuditedEntity
{
    private Plan()
    {
    }

    public Plan(string id, string name, decimal price, int maxEmployees, string providerPlanId)
    {
        Id = id;
        Name = name;
        Price = price;
        MaxEmployees = maxEmployees;
        ProviderPlanId = providerPlanId;
    }

    public string Name { get; private set; } = "";
    public decimal Price { get; private set; }
    public string Currency { get; private set; } = "USD";
    public string BillingCycle { get; private set; } = "monthly";
    public int MaxEmployees { get; private set; }
    public string ProviderPlanId { get; private set; } = "";
}
