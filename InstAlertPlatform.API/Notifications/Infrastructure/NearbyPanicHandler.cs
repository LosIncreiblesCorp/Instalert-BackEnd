using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Notifications.Application.Errors;
using System.Text.Json;
using InstAlertPlatform.API.Alert.Application.Services;
using InstAlertPlatform.API.Notifications.Application.Services;
using InstAlertPlatform.API.Notifications.Domain.Model.Commands;
using InstAlertPlatform.API.Shared.Application.Patterns;

namespace InstAlertPlatform.API.Notifications.Infrastructure;
public sealed class NearbyPanicHandler(INearbyPanicCommandService commands) : IIntegrationEventHandler
{
    public string EventType => "alert.changed.v1";

    public async Task Handle(string payload, CancellationToken token)
    {
        var fact = JsonSerializer.Deserialize<AlertFact>(payload) ?? throw new InvalidOperationException("Invalid integration fact.");
        var notificationQueueResult = (await commands.Handle(new QueueNearbyPanicNotificationsCommand(fact.Id, fact.EmployeeId, fact.Kind, fact.Status, fact.Latitude, fact.Longitude, fact.Revision), token));
        if (notificationQueueResult is Result<bool, EQueueNearbyPanicNotificationsError>.Failure notificationQueueFailure)
            throw new DomainException(notificationQueueFailure.Error switch
            {
                EQueueNearbyPanicNotificationsError.InvalidInput => "validation",
                EQueueNearbyPanicNotificationsError.Forbidden => "forbidden",
                EQueueNearbyPanicNotificationsError.NotFound => "not_found",
                EQueueNearbyPanicNotificationsError.Conflict => "conflict",
                EQueueNearbyPanicNotificationsError.ConcurrencyConflict => "concurrency",
                EQueueNearbyPanicNotificationsError.UnexpectedError => "unexpected",
                _ => "unexpected"
            }, notificationQueueFailure.Field);
    }
}
