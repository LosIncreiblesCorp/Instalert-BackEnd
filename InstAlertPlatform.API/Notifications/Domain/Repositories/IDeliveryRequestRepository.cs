using InstAlertPlatform.API.Notifications.Domain.Model.Aggregates;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Notifications.Domain.Repositories;
public interface IDeliveryRequestRepository : IBaseRepository<DeliveryRequest>
{
    Task<List<DeliveryRequest>> FindByUserIdAsync(string userId, CancellationToken cancellationToken = default);
}
