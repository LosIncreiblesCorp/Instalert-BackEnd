using InstAlertPlatform.API.Payments.Domain.Model.Aggregates;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Payments.Domain.Repositories;
public interface IPaymentTransactionRepository : IBaseRepository<PaymentTransaction>
{
    Task<List<PaymentTransaction>> FindByBusinessIdAsync(string businessId, CancellationToken cancellationToken = default);
    Task<List<PaymentTransaction>> FindByProviderTransactionIdAsync(string providerTransactionId, CancellationToken cancellationToken = default);
}
