using InstAlertPlatform.API.Notifications.Domain.Model.Aggregates;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Notifications.Domain.Repositories;
public interface INotificationPreferencesRepository : IBaseRepository<NotificationPreferences>
{
}
