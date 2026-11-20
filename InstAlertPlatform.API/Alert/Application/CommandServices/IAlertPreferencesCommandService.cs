using InstAlertPlatform.API.Alert.Application.Services.Snapshots;
using InstAlertPlatform.API.Alert.Application.Errors;
using InstAlertPlatform.API.Alert.Domain.Model.Commands;
using InstAlertPlatform.API.Shared.Application.Patterns;

namespace InstAlertPlatform.API.Alert.Application.CommandServices;
public interface IAlertPreferencesCommandService
{
    Task<Result<AlertPreferencesSnapshot, EChangePanicGracePeriodError>> Handle(ChangePanicGracePeriodCommand command, CancellationToken cancellationToken = default);
}
