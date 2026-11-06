using InstAlertPlatform.API.Payments.Application.Errors;
using InstAlertPlatform.API.Payments.Application.Services;
using InstAlertPlatform.API.Payments.Domain.Model.Commands;
using InstAlertPlatform.API.Shared.Application.Patterns;

namespace InstAlertPlatform.API.Payments.Application.Services;
public interface ISubscriptionCommandService
{
    Task<Result<SubscriptionCheckoutSnapshot, EBeginSubscriptionCheckoutError>> Handle(BeginSubscriptionCheckoutCommand command, CancellationToken cancellationToken = default);
    Task<Result<SubscriptionSnapshot, EVerifySubscriptionError>> Handle(VerifySubscriptionCommand command, CancellationToken cancellationToken = default);
    Task<Result<SubscriptionSnapshot, ECancelSubscriptionRenewalError>> Handle(CancelSubscriptionRenewalCommand command, CancellationToken cancellationToken = default);
    Task<Result<SubscriptionCheckoutSnapshot, EChangeSubscriptionPlanError>> Handle(ChangeSubscriptionPlanCommand command, CancellationToken cancellationToken = default);
    Task<Result<SubscriptionSnapshot, EVerifySubscriptionPlanChangeError>> Handle(VerifySubscriptionPlanChangeCommand command, CancellationToken cancellationToken = default);
}
