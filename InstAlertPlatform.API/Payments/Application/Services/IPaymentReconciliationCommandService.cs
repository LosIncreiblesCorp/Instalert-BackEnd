using InstAlertPlatform.API.Payments.Application.Errors;
using InstAlertPlatform.API.Payments.Domain.Model.Commands;
using InstAlertPlatform.API.Shared.Application.Patterns;

namespace InstAlertPlatform.API.Payments.Application.Services;
public interface IPaymentReconciliationCommandService
{
    Task<Result<bool, EReconcilePaymentError>> Handle(ReconcilePaymentCommand command, CancellationToken cancellationToken = default);
}
