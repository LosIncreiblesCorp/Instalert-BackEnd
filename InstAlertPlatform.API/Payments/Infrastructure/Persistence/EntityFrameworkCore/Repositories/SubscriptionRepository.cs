using InstAlertPlatform.API.Payments.Domain.Model.Aggregates;
using InstAlertPlatform.API.Payments.Domain.Repositories;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Payments.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
public sealed class SubscriptionRepository(AppDbContext context) : BaseRepository<Subscription>(context), ISubscriptionRepository
{
    public Task<List<Subscription>> FindByBusinessIdAsync(string businessId, CancellationToken cancellationToken = default) => Context.Set<Subscription>().Where(entity => entity.BusinessId == businessId).ToListAsync(cancellationToken);
    public Task<List<Subscription>> FindByProviderSubscriptionIdAsync(string providerSubscriptionId, CancellationToken cancellationToken = default) => Context.Set<Subscription>().Where(entity => entity.ProviderSubscriptionId == providerSubscriptionId).ToListAsync(cancellationToken);
}
