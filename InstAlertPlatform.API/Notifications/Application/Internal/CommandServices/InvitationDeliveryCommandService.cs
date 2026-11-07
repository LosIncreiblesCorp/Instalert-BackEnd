using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Notifications.Domain.Repositories;
using System.Text.Json;
using InstAlertPlatform.API.Notifications.Domain.Model.Aggregates;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Repositories;
using InstAlertPlatform.API.Notifications.Application.Services;
using InstAlertPlatform.API.Notifications.Application.Errors;
using InstAlertPlatform.API.Notifications.Domain.Model.Commands;

namespace InstAlertPlatform.API.Notifications.Application.Internal.CommandServices;
public sealed class InvitationDeliveryCommandService(IDeliveryRequestRepository deliveries, IBusinessProfileQueryService business) : IInvitationDeliveryCommandService
{
    public async Task<Result<bool, EQueueInvitationDeliveryError>> Handle(QueueInvitationDeliveryCommand command, CancellationToken token = default)
    {
        try
        {
            var fact = command;
            var current = await business.Handle(new GetInvitationDeliveryStateQuery(fact.InvitationId), token);
            if (current is null || !current.Pending || current.Email != fact.Email || current.Sequence != fact.Sequence)
                return new Result<bool, EQueueInvitationDeliveryError>.Success(true);
            var id = "invitation:" + fact.InvitationId + ":" + fact.Sequence;
            if (await deliveries.FindByIdAsync(id, token)is null)
                await deliveries.AddAsync(DeliveryRequest.Invitation(id, fact.InvitationId, fact.Email, fact.OwnerUserId), token);
            return new Result<bool, EQueueInvitationDeliveryError>.Success(true);
        }
        catch (DomainException error)
        {
            return new Result<bool, EQueueInvitationDeliveryError>.Failure(error.Code switch
            {
                "validation" => EQueueInvitationDeliveryError.InvalidInput,
                "forbidden" => EQueueInvitationDeliveryError.Forbidden,
                "not_found" => EQueueInvitationDeliveryError.NotFound,
                "conflict" => EQueueInvitationDeliveryError.Conflict,
                "concurrency" => EQueueInvitationDeliveryError.ConcurrencyConflict,
                "unexpected" => EQueueInvitationDeliveryError.UnexpectedError,
                _ => EQueueInvitationDeliveryError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }
}
