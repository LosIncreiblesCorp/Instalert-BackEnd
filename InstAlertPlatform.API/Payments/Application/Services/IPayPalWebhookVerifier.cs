namespace InstAlertPlatform.API.Payments.Application.Services;
public interface IPayPalWebhookVerifier
{
    Task<bool> VerifyWebhook(System.Text.Json.JsonElement payload, IReadOnlyDictionary<string, string> headers, CancellationToken token);
}
