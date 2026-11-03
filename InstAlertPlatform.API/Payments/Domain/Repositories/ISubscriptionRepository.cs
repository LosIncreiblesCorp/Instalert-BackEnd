using InstAlertPlatform.API.Payments.Domain.Model.Aggregates;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Payments.Domain.Repositories;
public interface ISubscriptionRepository : IBaseRepository<Subscription>
{
    Task<List<Subscription>> FindByBusinessIdAsync(string businessId, CancellationToken cancellationToken = default);
    Task<List<Subscription>> FindByProviderSubscriptionIdAsync(string providerSubscriptionId, CancellationToken cancellationToken = default);
}
