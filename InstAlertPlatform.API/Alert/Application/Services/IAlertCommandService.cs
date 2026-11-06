using InstAlertPlatform.API.Alert.Application.Errors;
using InstAlertPlatform.API.Alert.Application.Services;
using InstAlertPlatform.API.Alert.Domain.Model.Commands;
using InstAlertPlatform.API.Shared.Application.Patterns;

namespace InstAlertPlatform.API.Alert.Application.Services;
public interface IAlertCommandService
{
    Task<Result<PanicPreparationSnapshot, EPreparePanicError>> Handle(PreparePanicCommand command, CancellationToken cancellationToken = default);
    Task<Result<AlertFact, EConfirmPanicError>> Handle(ConfirmPanicCommand command, CancellationToken cancellationToken = default);
    Task<Result<AlertFact, ECreateIncidentReportError>> Handle(CreateIncidentReportCommand command, CancellationToken cancellationToken = default);
    Task<Result<AlertFact, EUpdateAlertLocationError>> Handle(UpdateAlertLocationCommand command, CancellationToken cancellationToken = default);
    Task<Result<AlertFact, EResolvePanicAlertError>> Handle(ResolvePanicAlertCommand command, CancellationToken cancellationToken = default);
    Task<Result<AlertFact, EUpdatePanicReportDraftError>> Handle(UpdatePanicReportDraftCommand command, CancellationToken cancellationToken = default);
    Task<Result<AlertFact, ECompletePanicReportError>> Handle(CompletePanicReportCommand command, CancellationToken cancellationToken = default);
}
