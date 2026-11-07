using InstAlertPlatform.API.Notifications.Application.Services;
using System.Net;
using System.Net.Http.Json;
using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Notifications.Infrastructure;
public sealed class DeliveryGateway(IConfiguration configuration, IHttpClientFactory httpClients) : IDeliveryGateway
{
    public bool Configured(string channel) => channel switch
    {
        "email" => configuration.GetValue<bool>("Email:Enabled") && !string.IsNullOrWhiteSpace(configuration["Brevo:ApiKey"]) && !string.IsNullOrWhiteSpace(configuration["Email:FromAddress"]),
        _ => false
    };
    public async Task Send(DeliveryEnvelope message, CancellationToken token)
    {
        Guard.Require(Configured(message.Channel), "provider_unavailable");
        if (message.Channel == "email")
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "smtp/email");
            request.Headers.Add("api-key", configuration["Brevo:ApiKey"]);
            request.Content = JsonContent.Create(new { sender = new { name = configuration["Email:FromName"] ?? "InstAlert", email = configuration["Email:FromAddress"] }, to = new[] { new { email = message.Destination } }, subject = message.Subject, textContent = message.Text, htmlContent = "<p>" + WebUtility.HtmlEncode(message.Text).Replace("\n", "<br>") + "</p>", headers = new Dictionary<string, string> { ["X-InstAlert-Delivery-Id"] = message.Id } });
            using var client = httpClients.CreateClient("brevo");
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException("The email provider rejected the request.", null, response.StatusCode);
            var receipt = await response.Content.ReadFromJsonAsync<EmailReceipt>(token);
            Guard.Require(!string.IsNullOrWhiteSpace(receipt?.MessageId), "provider_unavailable");
            return;
        }
    }

    private sealed record EmailReceipt(string MessageId);
}
