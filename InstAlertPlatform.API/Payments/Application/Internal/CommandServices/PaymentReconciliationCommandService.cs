using InstAlertPlatform.API.Payments.Application.Errors;
using InstAlertPlatform.API.Payments.Application.Services;
using InstAlertPlatform.API.Payments.Domain.Model.Aggregates;
using InstAlertPlatform.API.Payments.Domain.Model.Commands;
using InstAlertPlatform.API.Payments.Domain.Model.Queries;
using InstAlertPlatform.API.Payments.Domain.Repositories;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Payments.Application.Internal.CommandServices;
public sealed class PaymentReconciliationCommandService(ISubscriptionRepository subscriptions, IPaymentTransactionRepository transactions, IPayPalWebhookReceiptRepository receipts, IPayPalSubscriptions provider, IUnitOfWork work, TimeProvider clock, IPlanRepository plans, IConfiguration configuration) : IPaymentReconciliationCommandService
{
    public async Task<Result<bool, EReconcilePaymentError>> Handle(ReconcilePaymentCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var eventId = command.EventId;
            var eventType = command.EventType;
            var providerSubscriptionId = command.ProviderSubscriptionId;
            if (!string.IsNullOrEmpty(eventId) && await receipts.FindByIdAsync(eventId, cancellationToken)is not null)
                return new Result<bool, EReconcilePaymentError>.Success(true);
            var subscription = (await subscriptions.FindByProviderSubscriptionIdAsync(providerSubscriptionId, cancellationToken)).SingleOrDefault();
            if (subscription is null)
                throw new DomainException("conflict");
            var current = await provider.Get(providerSubscriptionId, cancellationToken);
            var plan = await plans.FindByIdAsync(subscription.PlanId, cancellationToken) ?? throw new DomainException("not_found");
            Guard.Require(current.Id == providerSubscriptionId && current.PlanId == (configuration[$"PayPal:Plans:{plan.Id}"] ?? plan.ProviderPlanId), "conflict");
            var confirmed = await provider.Transactions(providerSubscriptionId, clock.GetUtcNow().AddDays(-30), clock.GetUtcNow(), cancellationToken);
            foreach (var payment in confirmed)
            {
                Guard.Require(payment.Amount > 0 && payment.Currency == "USD", "conflict");
                if ((await transactions.FindByProviderTransactionIdAsync(payment.Id, cancellationToken)).Count == 0)
                    await transactions.AddAsync(new PaymentTransaction(subscription.BusinessId, payment.Id, payment.Amount, payment.Currency, payment.PaidAt), cancellationToken);
            }

            if (current.Status == "ACTIVE" && current.PaidUntil > clock.GetUtcNow() && confirmed.Any(p => p.Amount == plan.Price && p.Currency == plan.Currency))
            {
                var last = confirmed.Where(p => p.Amount == plan.Price && p.Currency == plan.Currency).MaxBy(t => t.PaidAt)!;
                Guard.Require(current.PaidUntil <= last.PaidAt.AddMonths(1).AddDays(1), "conflict");
                subscription.Confirm(subscription.PlanId, current.PaidUntil.Value, clock.GetUtcNow());
            }

            if (current.Status == "CANCELLED" && subscription.Status == "active" && !subscription.CancelAtPeriodEnd)
                subscription.ScheduleCancellation();
            if (!string.IsNullOrEmpty(eventId)) await receipts.AddAsync(new PayPalWebhookReceipt(eventId, eventType), cancellationToken);
            await work.CompleteAsync(cancellationToken);
            return new Result<bool, EReconcilePaymentError>.Success(true);
        }
        catch (DomainException error)
        {
            return new Result<bool, EReconcilePaymentError>.Failure(error.Code switch
            {
                "validation" => EReconcilePaymentError.InvalidInput,
                "unauthenticated" => EReconcilePaymentError.Unauthenticated,
                "forbidden" => EReconcilePaymentError.Forbidden,
                "not_found" => EReconcilePaymentError.NotFound,
                "conflict" => EReconcilePaymentError.Conflict,
                "concurrency" => EReconcilePaymentError.ConcurrencyConflict,
                "capacity" => EReconcilePaymentError.CapacityExceeded,
                "transition" => EReconcilePaymentError.InvalidTransition,
                "provider_unavailable" => EReconcilePaymentError.ProviderUnavailable,
                "unexpected" => EReconcilePaymentError.UnexpectedError,
                _ => EReconcilePaymentError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }
}
