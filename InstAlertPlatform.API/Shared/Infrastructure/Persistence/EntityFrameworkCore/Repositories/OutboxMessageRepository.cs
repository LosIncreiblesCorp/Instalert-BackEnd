using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Shared.Domain.Repositories;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
public sealed class OutboxMessageRepository(AppDbContext context) : BaseRepository<OutboxMessage>(context), IOutboxMessageRepository
{
    public Task<List<OutboxMessage>> FindByEventTypeAsync(string eventType, CancellationToken cancellationToken = default) => Context.Set<OutboxMessage>().Where(entity => entity.EventType == eventType).ToListAsync(cancellationToken);
}
