using InstAlertPlatform.API.Payments.Application.Services;
using InstAlertPlatform.API.Payments.Domain.Model.Aggregates;
using InstAlertPlatform.API.Payments.Domain.Model.Queries;
using InstAlertPlatform.API.Payments.Domain.Repositories;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Payments.Application.Internal.QueryServices;
public sealed class PlanQueryService(IPlanRepository plans, ISubscriptionRepository subscriptions, TimeProvider clock, IHostEnvironment environment, IConfiguration configuration) : IPlanQueryService
{
    public async Task<List<PlanSnapshot>> Handle(GetPlansQuery query, CancellationToken cancellationToken = default)
    {
        return (await plans.ListAsync(cancellationToken)).OrderBy(p => p.Price).Select(p => new PlanSnapshot(p.Id, p.Name, p.Price, p.Price, p.Currency, p.BillingCycle, p.MaxEmployees, p.ProviderPlanId)).ToList();
    }

    public async Task<PlanLimitSnapshot> Handle(GetPlanLimitsQuery query, CancellationToken token = default)
    {
        var businessId = query.BusinessId;
        var active = (await subscriptions.FindByBusinessIdAsync(businessId, token)).SingleOrDefault();
        if (active?.ProviderSubscriptionId.StartsWith("synthetic-development-", StringComparison.Ordinal) == true &&
            ((!environment.IsDevelopment() && !configuration.GetValue<bool>("Demo:Enabled")) ||
             !configuration.GetValue<bool>("DevelopmentData:SeedOnStartup")))
            return new(0, null, false);
        if (active is null || !active.IsEntitled(clock.GetUtcNow()))
            return new(0, null, false);
        var plan = await plans.FindByIdAsync(active.PlanId, token);
        return plan is null ? new(0, null, false) : new(plan.MaxEmployees, plan.Name, true, active.CurrentPeriodEnd, active.CancelAtPeriodEnd);
    }
}
