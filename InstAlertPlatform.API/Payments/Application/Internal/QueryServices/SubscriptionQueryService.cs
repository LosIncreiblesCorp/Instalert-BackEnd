using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Payments.Application.Services;
using InstAlertPlatform.API.Payments.Domain.Model.Aggregates;
using InstAlertPlatform.API.Payments.Domain.Model.Queries;
using InstAlertPlatform.API.Payments.Domain.Repositories;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Payments.Application.Internal.QueryServices;
public sealed class SubscriptionQueryService(ISubscriptionRepository subscriptions, IPaymentTransactionRepository transactions, IBusinessProfileQueryService business, ICurrentActor actor, TimeProvider clock, IHostEnvironment environment, IConfiguration configuration) : ISubscriptionQueryService
{
    public async Task<SubscriptionSnapshot?> Handle(GetCurrentSubscriptionQuery query, CancellationToken cancellationToken = default)
    {
        var id = query.Id;
        await business.Handle(new RequireBusinessAccessQuery(id, actor.UserId, true), cancellationToken);
        var s = (await subscriptions.FindByBusinessIdAsync(id, cancellationToken)).SingleOrDefault();
        if (s?.ProviderSubscriptionId.StartsWith("synthetic-development-", StringComparison.Ordinal) == true &&
            ((!environment.IsDevelopment() && !configuration.GetValue<bool>("Demo:Enabled")) ||
             !configuration.GetValue<bool>("DevelopmentData:SeedOnStartup")))
            return null;
        return s is null ? null : ToSubscriptionSnapshot(clock.GetUtcNow(), s);
    }

    public async Task<List<PaymentSnapshot>> Handle(GetBillingHistoryQuery query, CancellationToken cancellationToken = default)
    {
        var businessId = query.BusinessId;
        await business.Handle(new RequireBusinessAccessQuery(businessId, actor.UserId, true), cancellationToken);
        return (await transactions.FindByBusinessIdAsync(businessId, cancellationToken)).OrderByDescending(p => p.PaidAt).Select(p => new PaymentSnapshot(p.Id, p.BusinessId, p.ProviderTransactionId, p.PaidAt, p.Amount, p.Currency, "paid")).ToList();
    }

    private static SubscriptionSnapshot ToSubscriptionSnapshot(DateTimeOffset now, Subscription s) => new(s.Id, s.BusinessId, s.PlanId, s.Status == "active" && !s.IsEntitled(now) ? "expired" : s.Status, s.CurrentPeriodEnd, s.CancelAtPeriodEnd, s.Version, s.ProviderSubscriptionId.StartsWith("synthetic-development-", StringComparison.Ordinal));
}
