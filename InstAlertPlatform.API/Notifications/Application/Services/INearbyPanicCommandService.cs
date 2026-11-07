using InstAlertPlatform.API.Notifications.Domain.Model.Commands;
using InstAlertPlatform.API.Notifications.Application.Errors;
using InstAlertPlatform.API.Shared.Application.Patterns;

namespace InstAlertPlatform.API.Notifications.Application.Services;
public interface INearbyPanicCommandService
{
    Task<Result<bool, EQueueNearbyPanicNotificationsError>> Handle(QueueNearbyPanicNotificationsCommand command, CancellationToken cancellationToken = default);
}
