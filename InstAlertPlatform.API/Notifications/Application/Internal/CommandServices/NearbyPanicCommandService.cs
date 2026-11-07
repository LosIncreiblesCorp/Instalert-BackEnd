using InstAlertPlatform.API.Contacts.Domain.Model.Queries;
using InstAlertPlatform.API.Contacts.Application.Services;
using InstAlertPlatform.API.Alert.Domain.Model.Queries;
using InstAlertPlatform.API.Alert.Application.Services;
using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Notifications.Domain.Repositories;
using System.Text.Json;
using InstAlertPlatform.API.Notifications.Domain.Model.Aggregates;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Shared.Domain.Repositories;
using InstAlertPlatform.API.Notifications.Application.Services;
using InstAlertPlatform.API.Notifications.Application.Errors;
using InstAlertPlatform.API.Notifications.Domain.Model.Commands;

namespace InstAlertPlatform.API.Notifications.Application.Internal.CommandServices;
public sealed class NearbyPanicCommandService(IReceivedNotificationRepository notifications, INotificationPreferencesRepository preferences, IDeliveryRequestRepository deliveries, IBusinessProfileQueryService business, IEmergencyContactQueryService contacts, IAlertQueryService alerts, TimeProvider clock) : INearbyPanicCommandService
{
    public async Task<Result<bool, EQueueNearbyPanicNotificationsError>> Handle(QueueNearbyPanicNotificationsCommand command, CancellationToken token = default)
    {
        try
        {
            var fact = command;
            if (fact.Kind != "panic" || fact.Status != "active")
                return new Result<bool, EQueueNearbyPanicNotificationsError>.Success(true);
            var current = (await alerts.Handle(new GetAllAlertFactsQuery(), token)).SingleOrDefault(x => x.Id == fact.Id);
            if (current?.Status == "active" && fact.Latitude.HasValue && fact.Longitude.HasValue)
                foreach (var recipient in await business.Handle(new GetBusinessAudienceQuery(), token))
                {
                    var p = await preferences.FindByIdAsync(recipient.BusinessId + ":" + recipient.UserId, token);
                    var distance = Geography.DistanceMeters(recipient.Latitude, recipient.Longitude, fact.Latitude.Value, fact.Longitude.Value);
                    if (p?.NearbyAlertsEnabled == false || distance > (p?.NearbyRadiusMeters.Value ?? 500))
                        continue;
                    var notification = new ReceivedNotification(fact.Id, recipient.UserId, recipient.BusinessId, distance, clock.GetUtcNow());
                    if (await notifications.FindByIdAsync(notification.Id, token)is null)
                        await notifications.AddAsync(notification, token);
                }

            if (fact.Revision == 1)
                foreach (var contact in await contacts.Handle(new GetEmergencyContactRecipientsQuery(fact.EmployeeId), token))
                {
                    var id = "email:" + fact.Id + ":" + contact.ContactId;
                    if (await deliveries.FindByIdAsync(id, token)is null)
                        await deliveries.AddAsync(new DeliveryRequest(id, fact.Id, "email", contact.Email, fact.EmployeeId), token);
                }

            return new Result<bool, EQueueNearbyPanicNotificationsError>.Success(true);
        }
        catch (DomainException error)
        {
            return new Result<bool, EQueueNearbyPanicNotificationsError>.Failure(error.Code switch
            {
                "validation" => EQueueNearbyPanicNotificationsError.InvalidInput,
                "forbidden" => EQueueNearbyPanicNotificationsError.Forbidden,
                "not_found" => EQueueNearbyPanicNotificationsError.NotFound,
                "conflict" => EQueueNearbyPanicNotificationsError.Conflict,
                "concurrency" => EQueueNearbyPanicNotificationsError.ConcurrencyConflict,
                "unexpected" => EQueueNearbyPanicNotificationsError.UnexpectedError,
                _ => EQueueNearbyPanicNotificationsError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }
}
