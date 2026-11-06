using InstAlertPlatform.API.Contacts.Application.Errors;
using InstAlertPlatform.API.Contacts.Application.Services;
using InstAlertPlatform.API.Contacts.Domain.Model.Commands;
using InstAlertPlatform.API.Shared.Application.Patterns;

namespace InstAlertPlatform.API.Contacts.Application.Services;
public interface IEmergencyContactCommandService
{
    Task<Result<ContactSnapshot, ECreateEmergencyContactError>> Handle(CreateEmergencyContactCommand command, CancellationToken cancellationToken = default);
    Task<Result<ContactSnapshot, EUpdateEmergencyContactError>> Handle(UpdateEmergencyContactCommand command, CancellationToken cancellationToken = default);
    Task<Result<bool, EDeleteEmergencyContactError>> Handle(DeleteEmergencyContactCommand command, CancellationToken cancellationToken = default);
}
