namespace InstAlertPlatform.API.Payments.Application.Internal.OutboundServices;
public interface IPayPalWebhookVerifier
{
    Task<bool> VerifyWebhook(System.Text.Json.JsonElement payload, IReadOnlyDictionary<string, string> headers, CancellationToken token);
}
