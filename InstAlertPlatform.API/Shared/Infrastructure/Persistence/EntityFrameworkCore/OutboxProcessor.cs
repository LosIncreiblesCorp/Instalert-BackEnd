using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore;
public sealed class OutboxProcessor(AppDbContext db, IEnumerable<IIntegrationEventHandler> handlers, TimeProvider clock, ILogger<OutboxProcessor> logger)
{
    public async Task<int> Process(CancellationToken token)
    {
        var now = clock.GetUtcNow();
        var candidates = await db.Set<OutboxMessage>().Where(x => x.ProcessedAt == null && (x.LeaseUntil == null || x.LeaseUntil <= now)).OrderBy(x => x.CreatedAt).ThenBy(x => x.Id).Take(100).ToListAsync(token);
        var done = 0;
        foreach (var id in candidates.Where(x => x.LeaseUntil is null || x.LeaseUntil <= now).OrderBy(x => x.CreatedAt).Take(25).Select(x => x.Id))
        {
            var message = await db.Set<OutboxMessage>().FindAsync([id], token);
            if (message is null || message.ProcessedAt is not null || message.LeaseUntil > now)
                continue;
            try
            {
                message.Claim(now);
                await db.SaveChangesAsync(token);
            }
            catch (DbUpdateConcurrencyException)
            {
                db.Entry(message).State = EntityState.Detached;
                continue;
            }

            try
            {
                var consumers = handlers.Where(h => h.EventType == message.EventType).ToList();
                Guard.Require(consumers.Count > 0, "provider_unavailable");
                foreach (var handler in consumers)
                    await handler.Handle(message.Payload, token);
                message.Complete(clock.GetUtcNow());
                await db.SaveChangesAsync(token);
                done++;
            }
            catch (OperationCanceledException)when (token.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception e)
            {
                logger.LogWarning("Outbox delivery failed for {MessageId}, type {ErrorType}", message.Id, e.GetType().Name);
                db.ChangeTracker.Clear();
                var retry = await db.Set<OutboxMessage>().FindAsync([message.Id], token);
                if (retry is not null)
                {
                    retry.Retry(clock.GetUtcNow());
                    await db.SaveChangesAsync(token);
                }
            }
        }

        return done;
    }
}
