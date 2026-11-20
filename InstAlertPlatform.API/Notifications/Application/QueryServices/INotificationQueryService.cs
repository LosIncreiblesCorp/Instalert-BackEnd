using InstAlertPlatform.API.Notifications.Application.Services.Snapshots;
using InstAlertPlatform.API.Notifications.Domain.Model.Queries;

namespace InstAlertPlatform.API.Notifications.Application.QueryServices;
public interface INotificationQueryService
{
    Task<List<NotificationSnapshot>> Handle(GetReceivedNotificationsQuery query, CancellationToken cancellationToken = default);
    Task<List<DeliveryStatusSnapshot>> Handle(GetDeliveryStatusesQuery query, CancellationToken cancellationToken = default);
    Task<int> Handle(GetUnreadNotificationCountQuery query, CancellationToken cancellationToken = default);
}
