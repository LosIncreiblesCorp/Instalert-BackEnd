using InstAlertPlatform.API.Notifications.Application.Services.Snapshots;
using InstAlertPlatform.API.Notifications.Domain.Model.Queries;

namespace InstAlertPlatform.API.Notifications.Application.QueryServices;
public interface INotificationPreferencesQueryService
{
    Task<PreferencesSnapshot> Handle(GetNotificationPreferencesQuery query, CancellationToken cancellationToken = default);
}
