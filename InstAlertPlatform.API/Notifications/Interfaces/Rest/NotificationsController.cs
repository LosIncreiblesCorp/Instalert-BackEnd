using InstAlertPlatform.API.Resources;
using Microsoft.Extensions.Localization;
using InstAlertPlatform.API.Notifications.Application.Services;
using InstAlertPlatform.API.Notifications.Domain.Model.Commands;
using InstAlertPlatform.API.Notifications.Domain.Model.Queries;
using InstAlertPlatform.API.Notifications.Interfaces.Rest.Resources;
using InstAlertPlatform.API.Notifications.Interfaces.Rest.Transform;
using InstAlertPlatform.API.Shared.Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace InstAlertPlatform.API.Notifications.Interfaces.Rest;
[ApiController, Route("api/v1/[controller]"), Tags("Notifications")]
[ProducesResponseType(typeof(ProblemDetails), 400), ProducesResponseType(typeof(ProblemDetails), 401), ProducesResponseType(typeof(ProblemDetails), 403), ProducesResponseType(typeof(ProblemDetails), 404), ProducesResponseType(typeof(ProblemDetails), 409), ProducesResponseType(typeof(ProblemDetails), 503)]
[ProducesResponseType(typeof(ProblemDetails), 500)]
public sealed class NotificationsController(INotificationQueryService notificationQueries, INotificationCommandService notificationCommands, INotificationPreferencesQueryService notificationPreferencesQueries, INotificationPreferencesCommandService notificationPreferencesCommands, IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<NotificationResource>), 200)]
    public async Task<IActionResult> List(string businessId, CancellationToken cancellationToken, int page = 1, int pageSize = 50)
    {
        var snapshot = await notificationQueries.Handle(new GetReceivedNotificationsQuery(businessId, page, pageSize), cancellationToken);
        return Ok(snapshot.Select(NotificationResourceFromSnapshotAssembler.ToResourceFromSnapshot).ToList());
    }

    [HttpGet("deliveries")]
    [ProducesResponseType(typeof(List<DeliveryStatusResource>), 200)]
    public async Task<IActionResult> Deliveries(CancellationToken cancellationToken)
    {
        var snapshot = await notificationQueries.Handle(new GetDeliveryStatusesQuery(), cancellationToken);
        return Ok(snapshot.Select(DeliveryStatusResourceFromSnapshotAssembler.ToResourceFromSnapshot).ToList());
    }

    [HttpPost("{id}/reading")]
    [ProducesResponseType(typeof(NotificationResource), 200)]
    public async Task<IActionResult> Read(string id, CancellationToken cancellationToken)
    {
        var result = await notificationCommands.Handle(new ReadNotificationCommand(id), cancellationToken);
        return ActionResultFromReadNotificationResultAssembler.ToActionResultFromReadNotificationResult(result, this, localizer, snapshot => Ok(NotificationResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot)));
    }

    [HttpGet("/api/v1/notification-preferences/{userId}")]
    [ProducesResponseType(typeof(NotificationPreferencesResource), 200)]
    public async Task<IActionResult> Preferences(string userId, string businessId, CancellationToken cancellationToken)
    {
        var snapshot = await notificationPreferencesQueries.Handle(new GetNotificationPreferencesQuery(userId, businessId), cancellationToken);
        return Ok(NotificationPreferencesResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot));
    }

    [HttpPut("/api/v1/notification-preferences/{userId}")]
    [ProducesResponseType(typeof(NotificationPreferencesResource), 200)]
    public async Task<IActionResult> Update(string userId, string businessId, ChangeNotificationPreferencesResource resource, CancellationToken cancellationToken)
    {
        var result = await notificationPreferencesCommands.Handle(ChangeNotificationPreferencesCommandFromResourceAssembler.ToCommandFromResource(resource, userId, businessId), cancellationToken);
        return ActionResultFromChangeNotificationPreferencesResultAssembler.ToActionResultFromChangeNotificationPreferencesResult(result, this, localizer, snapshot => Ok(NotificationPreferencesResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot)));
    }
}
