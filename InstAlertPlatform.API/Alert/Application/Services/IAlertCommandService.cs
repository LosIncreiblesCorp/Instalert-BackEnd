using InstAlertPlatform.API.Alert.Application.Errors;
using InstAlertPlatform.API.Alert.Application.Services;
using InstAlertPlatform.API.Alert.Domain.Model.Commands;
using InstAlertPlatform.API.Shared.Application.Patterns;

namespace InstAlertPlatform.API.Alert.Application.Services;
public interface IAlertCommandService
{
    Task<Result<AlertFact, ECreateIncidentReportError>> Handle(CreateIncidentReportCommand command, CancellationToken cancellationToken = default);
}
