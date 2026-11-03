using InstAlertPlatform.API.Payments.Application.Services;
using InstAlertPlatform.API.Payments.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Payments.Interfaces.Rest.Transform;
public static class PlanResourceFromSnapshotAssembler
{
    public static PlanResource ToResourceFromSnapshot(PlanSnapshot snapshot) => new(snapshot.Id, snapshot.Name, snapshot.Price, snapshot.PriceUsd, snapshot.Currency, snapshot.BillingCycle, snapshot.MaxEmployees, snapshot.ProviderPlanId);
}
