using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Notifications.Application.Services;
using InstAlertPlatform.API.Notifications.Domain.Model.Aggregates;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Notifications.Infrastructure;
public sealed class DeliveryProcessor(AppDbContext db, IDeliveryGateway gateway, IBusinessProfileQueryService business, TimeProvider clock, ILogger<DeliveryProcessor> logger)
{
    public async Task Process(CancellationToken token)
    {
        var now = clock.GetUtcNow();
        var pending = await db.Set<DeliveryRequest>().Where(d => (d.Status == "queued" || d.Status == "processing") && d.Attempts < 5 && (d.LeaseUntil == null || d.LeaseUntil <= now)).OrderBy(d => d.CreatedAt).ThenBy(d => d.Id).Take(25).ToListAsync(token);
        foreach (var d in pending.Where(d => d.LeaseUntil is null || d.LeaseUntil <= clock.GetUtcNow()))
        {
            if (!gateway.Configured(d.Channel))
                continue;
            try
            {
                d.Claim(clock.GetUtcNow());
                await db.SaveChangesAsync(token);
            }
            catch (DbUpdateConcurrencyException)
            {
                db.Entry(d).State = EntityState.Detached;
                continue;
            }

            try
            {
                if (d.SourceInvitationId is not null)
                {
                    var invitation = await business.Handle(new GetInvitationDeliveryStateQuery(d.SourceInvitationId), token);
                    if (invitation is null || !invitation.Pending || invitation.Email != d.Destination)
                    {
                        d.Cancel();
                        await db.SaveChangesAsync(token);
                        continue;
                    }
                }

                await gateway.Send(new(d.Id, d.Channel, d.Destination, d.SourceInvitationId is null ? "InstAlert emergency" : "InstAlert business invitation", d.SourceInvitationId is null ? "A panic alert was confirmed. Contact the person who designated you as an emergency contact." : "A business invited you to join InstAlert. Review your pending invitation in InstAlert.", d.SourceAlertId), token);
                d.Sent(clock.GetUtcNow());
            }
            catch (OperationCanceledException)when (token.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception e)
            {
                d.Failed(clock.GetUtcNow());
                logger.LogWarning("Delivery {Id} failed with {ErrorType}", d.Id, e.GetType().Name);
            }

            await db.SaveChangesAsync(token);
        }
    }
}
