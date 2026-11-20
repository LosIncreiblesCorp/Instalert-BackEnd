using InstAlertPlatform.API.Business.Application.QueryServices;
using InstAlertPlatform.API.Iam.Interfaces.Acl;
using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Notifications.Application.Services;
using InstAlertPlatform.API.Notifications.Domain.Model.Aggregates;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Notifications.Infrastructure;
public sealed class DeliveryProcessor(AppDbContext db, IDeliveryGateway gateway, IBusinessProfileQueryService business, TimeProvider clock, ILogger<DeliveryProcessor> logger, IIamContextFacade identity, IConfiguration configuration)
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
                string? invitationText = null;
                if (d.SourceInvitationId is not null)
                {
                    var invitation = await business.Handle(new GetInvitationDeliveryStateQuery(d.SourceInvitationId), token);
                    if (invitation is null || !invitation.Pending || invitation.Email != d.Destination || invitation.Sequence != d.InvitationSequence)
                    {
                        d.Cancel();
                        await db.SaveChangesAsync(token);
                        continue;
                    }
                    string? password = null;
                    if (d.InvitationCredential is not null)
                    {
                        password = await identity.ReadInvitationCredential(d.InvitationCredential, d.Destination, token);
                        if (password is null) { d.Cancel(); await db.SaveChangesAsync(token); continue; }
                    }
                    var publicUrl = configuration["Application:PublicUrl"]?.TrimEnd('/');
                    if (!Uri.TryCreate(publicUrl, UriKind.Absolute, out var address) || address.Scheme is not ("http" or "https"))
                        throw new InvalidOperationException("Configure the public application URL before delivering invitations.");
                    invitationText = "You have been invited to InstAlert. Sign in at " + publicUrl + "/sign-in\nEmail: " + d.Destination +
                        (password is null ? "\nUse your personal password to review the invitation." :
                        "\nTemporary password: " + password + "\nThis password expires 24 hours after it was issued. Change it before completing your profile and accepting the invitation.");
                }

                await gateway.Send(new(d.Id, d.Channel, d.Destination, d.SourceInvitationId is null ? "InstAlert emergency" : "InstAlert business invitation", d.SourceInvitationId is null ? "A panic alert was confirmed. Contact the person who designated you as an emergency contact." : invitationText!, d.SourceAlertId), token);
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
