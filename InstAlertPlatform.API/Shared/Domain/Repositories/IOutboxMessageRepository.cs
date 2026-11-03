using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Shared.Domain.Repositories;
public interface IOutboxMessageRepository : IBaseRepository<OutboxMessage>
{
    Task<List<OutboxMessage>> FindByEventTypeAsync(string eventType, CancellationToken cancellationToken = default);
}
