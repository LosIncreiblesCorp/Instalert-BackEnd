namespace InstAlertPlatform.API.Payments.Application.Internal.OutboundServices;
public interface IPayPalSubscriptions
{
    Task<ProviderSubscription> Get(string id, CancellationToken token);
    Task<string> Create(string planId, string checkoutId, CancellationToken token);
    Task Suspend(string id, CancellationToken token);
    Task Activate(string id, CancellationToken token);
    Task Cancel(string id, CancellationToken token);
    Task ValidatePlan(string id, decimal amount, string currency, CancellationToken token);
    Task<List<ProviderTransaction>> Transactions(string id, DateTimeOffset start, DateTimeOffset end, CancellationToken token);
}
