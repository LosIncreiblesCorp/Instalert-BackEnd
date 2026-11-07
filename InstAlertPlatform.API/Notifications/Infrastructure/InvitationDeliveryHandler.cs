using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Notifications.Application.Errors;
using System.Text.Json;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Notifications.Application.Services;
using InstAlertPlatform.API.Notifications.Domain.Model.Commands;
using InstAlertPlatform.API.Shared.Application.Patterns;

namespace InstAlertPlatform.API.Notifications.Infrastructure;
public sealed class InvitationDeliveryHandler(IInvitationDeliveryCommandService commands) : IIntegrationEventHandler
{
    public string EventType => "business.invitation.requested.v1";

    public async Task Handle(string payload, CancellationToken token)
    {
        var fact = JsonSerializer.Deserialize<InvitationDeliveryFact>(payload) ?? throw new InvalidOperationException("Invalid integration fact.");
        var invitationQueueResult = (await commands.Handle(new QueueInvitationDeliveryCommand(fact.InvitationId, fact.OwnerUserId, fact.Email, fact.Sequence), token));
        if (invitationQueueResult is Result<bool, EQueueInvitationDeliveryError>.Failure invitationQueueFailure)
            throw new DomainException(invitationQueueFailure.Error switch
            {
                EQueueInvitationDeliveryError.InvalidInput => "validation",
                EQueueInvitationDeliveryError.Forbidden => "forbidden",
                EQueueInvitationDeliveryError.NotFound => "not_found",
                EQueueInvitationDeliveryError.Conflict => "conflict",
                EQueueInvitationDeliveryError.ConcurrencyConflict => "concurrency",
                EQueueInvitationDeliveryError.UnexpectedError => "unexpected",
                _ => "unexpected"
            }, invitationQueueFailure.Field);
    }
}
