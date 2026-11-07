using InstAlertPlatform.API.Notifications.Domain.Model.Aggregates;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Notifications.Infrastructure;
public sealed class DeliveryWorker(IServiceScopeFactory scopes, IConfiguration configuration, ILogger<DeliveryWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        if (!configuration.GetValue<bool>("Workers:Enabled"))
            return;
        while (!token.IsCancellationRequested)
        {
            try
            {
                using var scope = scopes.CreateScope();
                await scope.ServiceProvider.GetRequiredService<DeliveryProcessor>().Process(token);
            }
            catch (OperationCanceledException)when (token.IsCancellationRequested)
            {
                break;
            }
            catch (Exception e)
            {
                logger.LogWarning("Delivery cycle failed with {ErrorType}", e.GetType().Name);
            }

            await Task.Delay(TimeSpan.FromSeconds(10), token);
        }
    }
}
