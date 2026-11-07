using InstAlertPlatform.API.Notifications.Application.Services;
using InstAlertPlatform.API.Notifications.Domain.Model.Queries;

namespace InstAlertPlatform.API.Notifications.Application.Services;
public interface INotificationPreferencesQueryService
{
    Task<PreferencesSnapshot> Handle(GetNotificationPreferencesQuery query, CancellationToken cancellationToken = default);
}
