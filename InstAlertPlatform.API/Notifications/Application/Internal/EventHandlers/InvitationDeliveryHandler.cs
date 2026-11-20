using InstAlertPlatform.API.Business.Domain.Model.Events;
using InstAlertPlatform.API.Notifications.Application.CommandServices;
using InstAlertPlatform.API.Shared.Application.Internal.EventHandlers;
using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Notifications.Application.Errors;
using System.Text.Json;
using InstAlertPlatform.API.Notifications.Application.Services;
using InstAlertPlatform.API.Notifications.Domain.Model.Commands;
using InstAlertPlatform.API.Shared.Application.Patterns;

namespace InstAlertPlatform.API.Notifications.Application.Internal.EventHandlers;
public sealed class InvitationDeliveryHandler(IInvitationDeliveryCommandService commands) : IIntegrationEventHandler
{
    public string EventType => "business.invitation.requested.v1";

    public async Task Handle(string payload, CancellationToken token)
    {
        var fact = JsonSerializer.Deserialize<StaffInvitationRequestedEvent>(payload) ?? throw new InvalidOperationException("Invalid integration fact.");
        using var document = JsonDocument.Parse(payload);
        var credential = document.RootElement.TryGetProperty("Credential", out var protectedValue) ? protectedValue.GetString() : null;
        var invitationQueueResult = (await commands.Handle(new QueueInvitationDeliveryCommand(fact.InvitationId, fact.OwnerUserId, fact.Email, fact.Sequence, credential), token));
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
