using InstAlertPlatform.API.Payments.Domain.Model.Aggregates;
using InstAlertPlatform.API.Payments.Domain.Repositories;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Payments.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
public sealed class PaymentTransactionRepository(AppDbContext context) : BaseRepository<PaymentTransaction>(context), IPaymentTransactionRepository
{
    public Task<List<PaymentTransaction>> FindByBusinessIdAsync(string businessId, CancellationToken cancellationToken = default) => Context.Set<PaymentTransaction>().Where(entity => entity.BusinessId == businessId).ToListAsync(cancellationToken);
    public Task<List<PaymentTransaction>> FindByProviderTransactionIdAsync(string providerTransactionId, CancellationToken cancellationToken = default) => Context.Set<PaymentTransaction>().Where(entity => entity.ProviderTransactionId == providerTransactionId).ToListAsync(cancellationToken);
}
