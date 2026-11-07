using InstAlertPlatform.API.Notifications.Application.Errors;
using InstAlertPlatform.API.Notifications.Application.Services;
using InstAlertPlatform.API.Notifications.Domain.Model.Commands;
using InstAlertPlatform.API.Shared.Application.Patterns;

namespace InstAlertPlatform.API.Notifications.Application.Services;
public interface INotificationPreferencesCommandService
{
    Task<Result<PreferencesSnapshot, EChangeNotificationPreferencesError>> Handle(ChangeNotificationPreferencesCommand command, CancellationToken cancellationToken = default);
}
