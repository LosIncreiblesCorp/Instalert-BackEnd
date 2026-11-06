namespace InstAlertPlatform.API.Payments.Application.Services;
public interface IPayPalSubscriptions
{
    Task<ProviderSubscription> Get(string id, CancellationToken token);
    Task Cancel(string id, CancellationToken token);
    Task ValidatePlan(string id, decimal amount, string currency, CancellationToken token);
    Task<List<ProviderTransaction>> Transactions(string id, DateTimeOffset start, DateTimeOffset end, CancellationToken token);
}
