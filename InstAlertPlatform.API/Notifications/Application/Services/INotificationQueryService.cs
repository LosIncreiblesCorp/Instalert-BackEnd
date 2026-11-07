using InstAlertPlatform.API.Notifications.Application.Services;
using InstAlertPlatform.API.Notifications.Domain.Model.Queries;

namespace InstAlertPlatform.API.Notifications.Application.Services;
public interface INotificationQueryService
{
    Task<List<NotificationSnapshot>> Handle(GetReceivedNotificationsQuery query, CancellationToken cancellationToken = default);
    Task<List<DeliveryStatusSnapshot>> Handle(GetDeliveryStatusesQuery query, CancellationToken cancellationToken = default);
    Task<int> Handle(GetUnreadNotificationCountQuery query, CancellationToken cancellationToken = default);
}
