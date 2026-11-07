using InstAlertPlatform.API.Notifications.Domain.Model.Aggregates;
using InstAlertPlatform.API.Notifications.Domain.Repositories;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Notifications.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
public sealed class DeliveryRequestRepository(AppDbContext context) : BaseRepository<DeliveryRequest>(context), IDeliveryRequestRepository
{
    public Task<List<DeliveryRequest>> FindByUserIdAsync(string userId, CancellationToken cancellationToken = default) => Context.Set<DeliveryRequest>().Where(entity => entity.UserId == userId).ToListAsync(cancellationToken);
}
