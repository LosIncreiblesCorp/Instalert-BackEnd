using InstAlertPlatform.API.Profiles.Application.Services.Snapshots;
using InstAlertPlatform.API.Profiles.Application.Errors;
using InstAlertPlatform.API.Profiles.Domain.Model.Commands;
using InstAlertPlatform.API.Shared.Application.Patterns;

namespace InstAlertPlatform.API.Profiles.Application.CommandServices;
public interface IProfileCommandService
{
    Task<Result<ProfileSnapshot, EUpdateUserProfileError>> Handle(UpdateUserProfileCommand command, CancellationToken cancellationToken = default);
    Task<Result<UserPreferencesSnapshot, EChangeUserLocaleError>> Handle(ChangeUserLocaleCommand command, CancellationToken cancellationToken = default);
}
