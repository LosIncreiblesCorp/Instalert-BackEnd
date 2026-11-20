using InstAlertPlatform.API.Profiles.Application.Services;
using InstAlertPlatform.API.Profiles.Domain.Model.Queries;
using InstAlertPlatform.API.Business.Application.Errors;
using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Business.Domain.Model.Commands;
using InstAlertPlatform.API.Business.Application.Services;
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
public sealed class SubscriptionCommandService(IPlanRepository plans, ISubscriptionRepository subscriptions, ICheckoutIntentRepository intents, IPaymentTransactionRepository transactions, IPayPalSubscriptions provider, IBusinessProfileQueryService business, ICurrentActor actor, IUnitOfWork work, TimeProvider clock, IBusinessRevisionCommandService revisions, IConfiguration configuration, IProfileQueryService profiles) : ISubscriptionCommandService
{
    public async Task<Result<SubscriptionCheckoutSnapshot, EBeginSubscriptionCheckoutError>> Handle(BeginSubscriptionCheckoutCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var id = command.Id;
            var planId = command.PlanId;
            await business.Handle(new RequireBusinessAccessQuery(id, actor.UserId, true), cancellationToken);
            var profile = await profiles.Handle(new GetUserProfileQuery(actor.UserId), cancellationToken);
            Guard.Require(!string.IsNullOrWhiteSpace(profile.FirstName) && !string.IsNullOrWhiteSpace(profile.LastName) && profile.Dni.Length == 8, "forbidden");
            var plan = await plans.FindByIdAsync(planId, cancellationToken) ?? throw new DomainException("not_found");
            var existing = (await subscriptions.FindByBusinessIdAsync(id, cancellationToken)).SingleOrDefault();
            Guard.Require(!IsDevelopmentSubscription(existing), "policy_unavailable");
            Guard.Require(existing is null || !existing.IsEntitled(clock.GetUtcNow()), "policy_unavailable");
            if (existing is not null)
            {
                var old = await provider.Get(existing.ProviderSubscriptionId, cancellationToken);
                Guard.Require(!(old.Status == "ACTIVE" && old.PaidUntil > clock.GetUtcNow()), "conflict");
                if (old.Status is "ACTIVE" or "SUSPENDED") await provider.Cancel(existing.ProviderSubscriptionId, cancellationToken);
            }
            await provider.ValidatePlan((configuration[$"PayPal:Plans:{plan.Id}"] ?? plan.ProviderPlanId), plan.Price, plan.Currency, cancellationToken);
            var intent = new CheckoutIntent(id, plan.Id, clock.GetUtcNow());
            await intents.AddAsync(intent, cancellationToken);
            await work.CompleteAsync(cancellationToken);
            intent.LinkSubscription(await provider.Create((configuration[$"PayPal:Plans:{plan.Id}"] ?? plan.ProviderPlanId), intent.Id, cancellationToken));
            await work.CompleteAsync(cancellationToken);
            return new Result<SubscriptionCheckoutSnapshot, EBeginSubscriptionCheckoutError>.Success(new(intent.Id, plan.Id, (configuration[$"PayPal:Plans:{plan.Id}"] ?? plan.ProviderPlanId), intent.Id, intent.ExpiresAt, intent.ProviderSubscriptionId));
        }
        catch (DomainException error)
        {
            return new Result<SubscriptionCheckoutSnapshot, EBeginSubscriptionCheckoutError>.Failure(error.Code switch
            {
                "validation" => EBeginSubscriptionCheckoutError.InvalidInput,
                "unauthenticated" => EBeginSubscriptionCheckoutError.Unauthenticated,
                "forbidden" => EBeginSubscriptionCheckoutError.Forbidden,
                "not_found" => EBeginSubscriptionCheckoutError.NotFound,
                "conflict" => EBeginSubscriptionCheckoutError.Conflict,
                "concurrency" => EBeginSubscriptionCheckoutError.ConcurrencyConflict,
                "capacity" => EBeginSubscriptionCheckoutError.CapacityExceeded,
                "transition" => EBeginSubscriptionCheckoutError.InvalidTransition,
                "provider_unavailable" => EBeginSubscriptionCheckoutError.ProviderUnavailable,
                "policy_unavailable" => EBeginSubscriptionCheckoutError.PolicyUnavailable,
                "unexpected" => EBeginSubscriptionCheckoutError.UnexpectedError,
                _ => EBeginSubscriptionCheckoutError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }

    public async Task<Result<SubscriptionSnapshot, EVerifySubscriptionError>> Handle(VerifySubscriptionCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var businessId = command.BusinessId;
            var checkoutId = command.CheckoutId;
            var subscriptionId = command.SubscriptionId;
            await business.Handle(new RequireBusinessAccessQuery(businessId, actor.UserId, true), cancellationToken);
            return new Result<SubscriptionSnapshot, EVerifySubscriptionError>.Success(
                await VerifyPayment(businessId, checkoutId, subscriptionId, cancellationToken));
        }
        catch (DomainException error)
        {
            return new Result<SubscriptionSnapshot, EVerifySubscriptionError>.Failure(error.Code switch
            {
                "validation" => EVerifySubscriptionError.InvalidInput,
                "unauthenticated" => EVerifySubscriptionError.Unauthenticated,
                "forbidden" => EVerifySubscriptionError.Forbidden,
                "not_found" => EVerifySubscriptionError.NotFound,
                "conflict" => EVerifySubscriptionError.Conflict,
                "concurrency" => EVerifySubscriptionError.ConcurrencyConflict,
                "capacity" => EVerifySubscriptionError.CapacityExceeded,
                "transition" => EVerifySubscriptionError.InvalidTransition,
                "provider_unavailable" => EVerifySubscriptionError.ProviderUnavailable,
                "unexpected" => EVerifySubscriptionError.UnexpectedError,
                _ => EVerifySubscriptionError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }

    public async Task<Result<SubscriptionSnapshot, ECancelSubscriptionRenewalError>> Handle(CancelSubscriptionRenewalCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var businessId = command.BusinessId;
            await business.Handle(new RequireBusinessAccessQuery(businessId, actor.UserId, true), cancellationToken);
            var s = (await subscriptions.FindByBusinessIdAsync(businessId, cancellationToken)).SingleOrDefault() ?? throw new DomainException("not_found");
            if (s.CancelAtPeriodEnd)
                return new Result<SubscriptionSnapshot, ECancelSubscriptionRenewalError>.Success(ToSubscriptionSnapshot(clock.GetUtcNow(), s));
            Guard.Require(s.IsEntitled(clock.GetUtcNow()), "transition");
            var current = await provider.Get(s.ProviderSubscriptionId, cancellationToken);
            if (current.Status == "ACTIVE") await provider.Suspend(s.ProviderSubscriptionId, cancellationToken);
            else Guard.Require(current.Status is "SUSPENDED" or "CANCELLED", "transition");
            s.ScheduleCancellation();
            await work.CompleteAsync(cancellationToken);
            return new Result<SubscriptionSnapshot, ECancelSubscriptionRenewalError>.Success(ToSubscriptionSnapshot(clock.GetUtcNow(), s));
        }
        catch (DomainException error)
        {
            return new Result<SubscriptionSnapshot, ECancelSubscriptionRenewalError>.Failure(error.Code switch
            {
                "validation" => ECancelSubscriptionRenewalError.InvalidInput,
                "unauthenticated" => ECancelSubscriptionRenewalError.Unauthenticated,
                "forbidden" => ECancelSubscriptionRenewalError.Forbidden,
                "not_found" => ECancelSubscriptionRenewalError.NotFound,
                "conflict" => ECancelSubscriptionRenewalError.Conflict,
                "concurrency" => ECancelSubscriptionRenewalError.ConcurrencyConflict,
                "capacity" => ECancelSubscriptionRenewalError.CapacityExceeded,
                "transition" => ECancelSubscriptionRenewalError.InvalidTransition,
                "provider_unavailable" => ECancelSubscriptionRenewalError.ProviderUnavailable,
                "unexpected" => ECancelSubscriptionRenewalError.UnexpectedError,
                _ => ECancelSubscriptionRenewalError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }

    public async Task<Result<SubscriptionCheckoutSnapshot, EChangeSubscriptionPlanError>> Handle(ChangeSubscriptionPlanCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var businessId = command.BusinessId;
            var planId = command.PlanId;
            await business.Handle(new RequireBusinessAccessQuery(businessId, actor.UserId, true), cancellationToken);
            var current = (await subscriptions.FindByBusinessIdAsync(businessId, cancellationToken)).SingleOrDefault() ?? throw new DomainException("not_found");
            Guard.Require(!IsDevelopmentSubscription(current), "transition");
            Guard.Require(current.IsEntitled(clock.GetUtcNow()) && !current.CancelAtPeriodEnd, "transition");
            var plan = await plans.FindByIdAsync(planId, cancellationToken) ?? throw new DomainException("not_found");
            Guard.Require(plan.Id != current.PlanId, "conflict");
            var usage = await business.Handle(new GetBusinessSeatUsageQuery(businessId), cancellationToken);
            Guard.Require(usage.ActiveEmployees + usage.ReservedSeats <= plan.MaxEmployees, "capacity");
            await provider.ValidatePlan((configuration[$"PayPal:Plans:{plan.Id}"] ?? plan.ProviderPlanId), plan.Price, plan.Currency, cancellationToken);
            var intent = new CheckoutIntent(businessId, plan.Id, clock.GetUtcNow());
            intent.LinkSubscription(current.ProviderSubscriptionId);
            await intents.AddAsync(intent, cancellationToken);
            await work.CompleteAsync(cancellationToken);
            return new Result<SubscriptionCheckoutSnapshot, EChangeSubscriptionPlanError>.Success(new(intent.Id, plan.Id, (configuration[$"PayPal:Plans:{plan.Id}"] ?? plan.ProviderPlanId), null, intent.ExpiresAt, current.ProviderSubscriptionId));
        }
        catch (DomainException error)
        {
            return new Result<SubscriptionCheckoutSnapshot, EChangeSubscriptionPlanError>.Failure(error.Code switch
            {
                "validation" => EChangeSubscriptionPlanError.InvalidInput,
                "unauthenticated" => EChangeSubscriptionPlanError.Unauthenticated,
                "forbidden" => EChangeSubscriptionPlanError.Forbidden,
                "not_found" => EChangeSubscriptionPlanError.NotFound,
                "conflict" => EChangeSubscriptionPlanError.Conflict,
                "concurrency" => EChangeSubscriptionPlanError.ConcurrencyConflict,
                "capacity" => EChangeSubscriptionPlanError.CapacityExceeded,
                "transition" => EChangeSubscriptionPlanError.InvalidTransition,
                "provider_unavailable" => EChangeSubscriptionPlanError.ProviderUnavailable,
                "unexpected" => EChangeSubscriptionPlanError.UnexpectedError,
                _ => EChangeSubscriptionPlanError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }

    public async Task<Result<SubscriptionSnapshot, EVerifySubscriptionPlanChangeError>> Handle(VerifySubscriptionPlanChangeCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var businessId = command.BusinessId;
            var checkoutId = command.CheckoutId;
            await business.Handle(new RequireBusinessAccessQuery(businessId, actor.UserId, true), cancellationToken);
            return new Result<SubscriptionSnapshot, EVerifySubscriptionPlanChangeError>.Success(
                await VerifyPlanChange(businessId, checkoutId, cancellationToken));
        }
        catch (DomainException error)
        {
            return new Result<SubscriptionSnapshot, EVerifySubscriptionPlanChangeError>.Failure(error.Code switch
            {
                "validation" => EVerifySubscriptionPlanChangeError.InvalidInput,
                "unauthenticated" => EVerifySubscriptionPlanChangeError.Unauthenticated,
                "forbidden" => EVerifySubscriptionPlanChangeError.Forbidden,
                "not_found" => EVerifySubscriptionPlanChangeError.NotFound,
                "conflict" => EVerifySubscriptionPlanChangeError.Conflict,
                "concurrency" => EVerifySubscriptionPlanChangeError.ConcurrencyConflict,
                "capacity" => EVerifySubscriptionPlanChangeError.CapacityExceeded,
                "transition" => EVerifySubscriptionPlanChangeError.InvalidTransition,
                "provider_unavailable" => EVerifySubscriptionPlanChangeError.ProviderUnavailable,
                "unexpected" => EVerifySubscriptionPlanChangeError.UnexpectedError,
                _ => EVerifySubscriptionPlanChangeError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }

    public async Task<Result<SubscriptionSnapshot, EResumeSubscriptionRenewalError>> Handle(ResumeSubscriptionRenewalCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            await business.Handle(new RequireBusinessAccessQuery(command.BusinessId, actor.UserId, true), cancellationToken);
            var s = (await subscriptions.FindByBusinessIdAsync(command.BusinessId, cancellationToken)).SingleOrDefault() ?? throw new DomainException("not_found");
            Guard.Require(s.IsEntitled(clock.GetUtcNow()), "transition");
            var current = await provider.Get(s.ProviderSubscriptionId, cancellationToken);
            if (current.Status == "SUSPENDED") await provider.Activate(s.ProviderSubscriptionId, cancellationToken);
            else Guard.Require(current.Status == "ACTIVE", "transition");
            if (s.CancelAtPeriodEnd) s.ResumeRenewal(clock.GetUtcNow());
            await work.CompleteAsync(cancellationToken);
            return new Result<SubscriptionSnapshot, EResumeSubscriptionRenewalError>.Success(ToSubscriptionSnapshot(clock.GetUtcNow(), s));
        }
        catch (DomainException error)
        {
            return new Result<SubscriptionSnapshot, EResumeSubscriptionRenewalError>.Failure(error.Code switch
            {
                "forbidden" => EResumeSubscriptionRenewalError.Forbidden,
                "not_found" => EResumeSubscriptionRenewalError.NotFound,
                "transition" => EResumeSubscriptionRenewalError.InvalidTransition,
                "provider_unavailable" => EResumeSubscriptionRenewalError.ProviderUnavailable,
                _ => EResumeSubscriptionRenewalError.UnexpectedError
            });
        }
    }

    internal async Task<SubscriptionSnapshot> VerifyPlanChange(string businessId, string checkoutId, CancellationToken cancellationToken)
    {
            var intent = await intents.FindByIdAsync(checkoutId, cancellationToken) ?? throw new DomainException("not_found");
            Guard.Require(intent.BusinessId == businessId, "forbidden");
            var current = (await subscriptions.FindByBusinessIdAsync(businessId, cancellationToken)).SingleOrDefault() ?? throw new DomainException("not_found");
            if (intent.Completed && current.PlanId == intent.PlanId)
                return ToSubscriptionSnapshot(clock.GetUtcNow(), current);
            Guard.Require(intent.ProviderSubscriptionId == current.ProviderSubscriptionId, "conflict");
            var plan = await plans.FindByIdAsync(intent.PlanId, cancellationToken) ?? throw new DomainException("not_found");
            var response = await provider.Get(current.ProviderSubscriptionId, cancellationToken);
            Guard.Require(response.Id == current.ProviderSubscriptionId && response.Status == "ACTIVE" && response.PlanId == (configuration[$"PayPal:Plans:{plan.Id}"] ?? plan.ProviderPlanId) && response.PaidUntil > clock.GetUtcNow(), "conflict");
            var usage = await business.Handle(new GetBusinessSeatUsageQuery(businessId), cancellationToken);
            Guard.Require(usage.ActiveEmployees + usage.ReservedSeats <= plan.MaxEmployees, "capacity");
            current.ChangePlan(plan.Id);
            intent.Complete(clock.GetUtcNow());
            var seatRevisionResult = (await revisions.Handle(new TouchBusinessSeatsCommand(businessId), cancellationToken));
            if (seatRevisionResult is Result<bool, ETouchBusinessSeatsError>.Failure seatRevisionFailure)
                throw new DomainException(seatRevisionFailure.Error switch
                {
                    ETouchBusinessSeatsError.NotFound => "not_found",
                    ETouchBusinessSeatsError.UnexpectedError => "unexpected",
                    _ => "unexpected"
                }, seatRevisionFailure.Field);
            await work.CompleteAsync(cancellationToken);
            return ToSubscriptionSnapshot(clock.GetUtcNow(), current);
    }

    internal async Task<SubscriptionSnapshot> VerifyPayment(string businessId, string checkoutId, string subscriptionId, CancellationToken cancellationToken)
    {
            var intent = await intents.FindByIdAsync(checkoutId, cancellationToken) ?? throw new DomainException("not_found");
            Guard.Require(intent.BusinessId == businessId && intent.ProviderSubscriptionId == subscriptionId, "forbidden");
            var previous = (await subscriptions.FindByBusinessIdAsync(businessId, cancellationToken)).SingleOrDefault();
            if (intent.Completed && previous?.ProviderSubscriptionId == subscriptionId)
                return ToSubscriptionSnapshot(clock.GetUtcNow(), previous);
            var plan = await plans.FindByIdAsync(intent.PlanId, cancellationToken) ?? throw new DomainException("not_found");
            await provider.ValidatePlan((configuration[$"PayPal:Plans:{plan.Id}"] ?? plan.ProviderPlanId), plan.Price, plan.Currency, cancellationToken);
            var result = await provider.Get(subscriptionId, cancellationToken);
            Guard.Require(result.Id == subscriptionId && result.PlanId == (configuration[$"PayPal:Plans:{plan.Id}"] ?? plan.ProviderPlanId) && result.CustomId == intent.Id && result.Status == "ACTIVE" && result.HasConfirmedPayment && result.PaidUntil > clock.GetUtcNow(), "conflict");
            var paid = await provider.Transactions(subscriptionId, clock.GetUtcNow().AddDays(-31), clock.GetUtcNow(), cancellationToken);
            var confirmedPayment = paid.Where(p => p.Currency == plan.Currency && p.Amount == plan.Price).MaxBy(p => p.PaidAt);
            Guard.Require(confirmedPayment is not null && result.PaidUntil <= confirmedPayment.PaidAt.AddMonths(1).AddDays(1), "conflict");
            var usage = await business.Handle(new GetBusinessSeatUsageQuery(businessId), cancellationToken);
            Guard.Require(usage.ActiveEmployees + usage.ReservedSeats <= plan.MaxEmployees, "capacity");
            if (previous is not null && previous.ProviderSubscriptionId != subscriptionId)
            {
                Guard.Require(!previous.IsEntitled(clock.GetUtcNow()), "conflict");
                var old = await provider.Get(previous.ProviderSubscriptionId, cancellationToken);
                Guard.Require(old.Status is "CANCELLED" or "EXPIRED", "conflict");

            }
            var s = previous ?? new Subscription(businessId, plan.Id, subscriptionId);
            if (s.ProviderSubscriptionId != subscriptionId)
                s.ReplaceExpiredSubscription(plan.Id, subscriptionId, result.PaidUntil!.Value, clock.GetUtcNow());
            else
                s.Confirm(plan.Id, result.PaidUntil!.Value, clock.GetUtcNow());
            if (previous is null)
                await subscriptions.AddAsync(s, cancellationToken);
            foreach (var payment in paid)
                if ((await transactions.FindByProviderTransactionIdAsync(payment.Id, cancellationToken)).Count == 0)
                    await transactions.AddAsync(new PaymentTransaction(businessId, payment.Id, payment.Amount, payment.Currency, payment.PaidAt), cancellationToken);
            intent.Complete(clock.GetUtcNow());
            var seatRevisionResult = (await revisions.Handle(new TouchBusinessSeatsCommand(businessId), cancellationToken));
            if (seatRevisionResult is Result<bool, ETouchBusinessSeatsError>.Failure seatRevisionFailure)
                throw new DomainException(seatRevisionFailure.Error switch
                {
                    ETouchBusinessSeatsError.NotFound => "not_found",
                    ETouchBusinessSeatsError.UnexpectedError => "unexpected",
                    _ => "unexpected"
                }, seatRevisionFailure.Field);
            await work.CompleteAsync(cancellationToken);
            return ToSubscriptionSnapshot(clock.GetUtcNow(), s);
    }

    private static bool IsDevelopmentSubscription(Subscription? subscription) => subscription?.ProviderSubscriptionId.StartsWith("synthetic-development-", StringComparison.Ordinal) == true;


    private static SubscriptionSnapshot ToSubscriptionSnapshot(DateTimeOffset now, Subscription s) => new(s.Id, s.BusinessId, s.PlanId, s.Status == "active" && !s.IsEntitled(now) ? "expired" : s.Status, s.CurrentPeriodEnd, s.CancelAtPeriodEnd, s.Version, IsDevelopmentSubscription(s));
}
