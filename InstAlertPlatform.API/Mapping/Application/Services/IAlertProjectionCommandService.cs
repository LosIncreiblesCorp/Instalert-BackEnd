using InstAlertPlatform.API.Mapping.Domain.Model.Commands;
using InstAlertPlatform.API.Mapping.Application.Errors;
using InstAlertPlatform.API.Shared.Application.Patterns;

namespace InstAlertPlatform.API.Mapping.Application.Services;
public interface IAlertProjectionCommandService
{
    Task<Result<bool, EProjectAlertError>> Handle(ProjectAlertCommand command, CancellationToken cancellationToken = default);
}
