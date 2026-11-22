using InstAlertPlatform.API.Payments.Domain.Model.Aggregates;
using InstAlertPlatform.API.Payments.Domain.Repositories;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Payments.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
public sealed class CheckoutIntentRepository(AppDbContext context) : BaseRepository<CheckoutIntent>(context), ICheckoutIntentRepository
{
    public Task<List<CheckoutIntent>> FindPendingAsync(CancellationToken token = default) => Context.Set<CheckoutIntent>()
        .Where(x => !x.Completed && x.ProviderSubscriptionId != null).OrderByDescending(x => x.CreatedAt).Take(100).ToListAsync(token);
}
