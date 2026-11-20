using InstAlertPlatform.API.Payments.Application.Internal.CommandServices;
using InstAlertPlatform.API.Payments.Application.Services;
using InstAlertPlatform.API.Payments.Domain.Model.Commands;
using InstAlertPlatform.API.Payments.Domain.Repositories;
using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Payments.Infrastructure;

public sealed class PaymentReconciliationWorker(IServiceScopeFactory scopes, IConfiguration configuration,
    ILogger<PaymentReconciliationWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!configuration.GetValue<bool>("Payments:Reconciliation:Enabled")) return;
        var interval = TimeSpan.FromSeconds(Math.Clamp(configuration.GetValue("Payments:Reconciliation:IntervalSeconds", 300), 30, 3600));
        while (!stoppingToken.IsCancellationRequested)
        {
            try { await Reconcile(stoppingToken); }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            catch (Exception error) { logger.LogWarning("Payment reconciliation failed: {ErrorType}", error.GetType().Name); }
            await Task.Delay(interval, stoppingToken);
        }
    }

    internal async Task Reconcile(CancellationToken token)
    {
        List<string> pending;
        List<string> subscriptions;
        using (var scope = scopes.CreateScope())
        {
            pending = (await scope.ServiceProvider.GetRequiredService<ICheckoutIntentRepository>().FindPendingAsync(token)).Select(x => x.Id).ToList();
            subscriptions = (await scope.ServiceProvider.GetRequiredService<ISubscriptionRepository>().ListAsync(token))
                .Where(x => !x.ProviderSubscriptionId.StartsWith("synthetic-", StringComparison.Ordinal)).Select(x => x.Id).ToList();
        }
        foreach (var id in pending)
        {
            try
            {
                using var scope = scopes.CreateScope();
                var services = scope.ServiceProvider;
                var intent = await services.GetRequiredService<ICheckoutIntentRepository>().FindByIdAsync(id, token);
                if (intent is null || intent.Completed || intent.ProviderSubscriptionId is null) continue;
                var remote = await services.GetRequiredService<IPayPalSubscriptions>().Get(intent.ProviderSubscriptionId, token);
                if (remote.Status != "ACTIVE") continue;
                var commands = services.GetRequiredService<SubscriptionCommandService>();
                if (remote.CustomId == intent.Id) await commands.VerifyPayment(intent.BusinessId, intent.Id, intent.ProviderSubscriptionId, token);
                else await commands.VerifyPlanChange(intent.BusinessId, intent.Id, token);
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested) { throw; }
            catch (DomainException) { /* Approvals and payments can remain pending between cycles. */ }
            catch (Exception error) { logger.LogWarning("Pending payment reconciliation failed: {ErrorType}", error.GetType().Name); }
        }
        foreach (var id in subscriptions)
        {
            try
            {
                using var scope = scopes.CreateScope();
                var services = scope.ServiceProvider;
                var subscription = await services.GetRequiredService<ISubscriptionRepository>().FindByIdAsync(id, token);
                if (subscription is null) continue;
                var provider = services.GetRequiredService<IPayPalSubscriptions>();
                var now = services.GetRequiredService<TimeProvider>().GetUtcNow();
                if (subscription.CancelAtPeriodEnd && !subscription.IsEntitled(now))
                {
                    var remote = await provider.Get(subscription.ProviderSubscriptionId, token);
                    if (remote.Status is "ACTIVE" or "SUSPENDED") await provider.Cancel(subscription.ProviderSubscriptionId, token);
                }
                await services.GetRequiredService<IPaymentReconciliationCommandService>().Handle(
                    new ReconcilePaymentCommand("", "poll", subscription.ProviderSubscriptionId), token);
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested) { throw; }
            catch (Exception error) { logger.LogWarning("Subscription reconciliation failed: {ErrorType}", error.GetType().Name); }
        }
    }
}
