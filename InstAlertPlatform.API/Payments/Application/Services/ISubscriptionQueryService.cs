using InstAlertPlatform.API.Payments.Application.Services;
using InstAlertPlatform.API.Payments.Domain.Model.Queries;

namespace InstAlertPlatform.API.Payments.Application.Services;
public interface ISubscriptionQueryService
{
    Task<SubscriptionSnapshot?> Handle(GetCurrentSubscriptionQuery query, CancellationToken cancellationToken = default);
    Task<List<PaymentSnapshot>> Handle(GetBillingHistoryQuery query, CancellationToken cancellationToken = default);
}
