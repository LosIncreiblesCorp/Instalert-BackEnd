using InstAlertPlatform.API.Payments.Application.Services;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Payments.Infrastructure;
public sealed class PayPalSubscriptions(HttpClient client, IConfiguration configuration) : IPayPalSubscriptions, IPayPalWebhookVerifier
{
    private async Task Authorize(CancellationToken token)
    {
        var id = configuration["PayPal:ClientId"];
        var secret = configuration["PayPal:ClientSecret"];
        Guard.Require(!string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(secret), "provider_unavailable");
        using var request = new HttpRequestMessage(HttpMethod.Post, "v1/oauth2/token");
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(id + ":" + secret)));
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string> { { "grant_type", "client_credentials" } });
        using var response = await client.SendAsync(request, token);
        Guard.Require(response.IsSuccessStatusCode, "provider_unavailable");
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync(token));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", json.RootElement.GetProperty("access_token").GetString());
    }

    public async Task<ProviderSubscription> Get(string id, CancellationToken token)
    {
        await Authorize(token);
        using var response = await client.GetAsync("v1/billing/subscriptions/" + Uri.EscapeDataString(id), token);
        Guard.Require(response.IsSuccessStatusCode, "provider_unavailable");
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync(token));
        var p = json.RootElement;
        var paid = p.TryGetProperty("billing_info", out var billing) && billing.TryGetProperty("last_payment", out var last) && last.TryGetProperty("time", out _);
        DateTimeOffset? until = null;
        if (p.TryGetProperty("billing_info", out billing) && billing.TryGetProperty("next_billing_time", out var next) && DateTimeOffset.TryParse(next.GetString(), out var date))
            until = date;
        return new(p.GetProperty("id").GetString()!, p.GetProperty("plan_id").GetString()!, p.GetProperty("status").GetString()!, p.TryGetProperty("custom_id", out var custom) ? custom.GetString() : null, until, paid);
    }

    public async Task Cancel(string id, CancellationToken token)
    {
        await Authorize(token);
        using var response = await client.PostAsJsonAsync("v1/billing/subscriptions/" + Uri.EscapeDataString(id) + "/cancel", new { reason = "Customer requested cancellation of future renewals" }, token);
        Guard.Require(response.IsSuccessStatusCode, "provider_unavailable");
    }

    public async Task ValidatePlan(string id, decimal amount, string currency, CancellationToken token)
    {
        await Authorize(token);
        using var response = await client.GetAsync("v1/billing/plans/" + Uri.EscapeDataString(id), token);
        Guard.Require(response.IsSuccessStatusCode, "provider_unavailable");
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync(token));
        var plan = json.RootElement;
        Guard.Require(plan.GetProperty("status").GetString() == "ACTIVE", "conflict");
        var cycles = plan.GetProperty("billing_cycles").EnumerateArray().ToList();
        Guard.Require(cycles.Count == 1 && cycles[0].GetProperty("tenure_type").GetString() == "REGULAR", "conflict");
        var cycle = cycles[0];
        var frequency = cycle.GetProperty("frequency");
        var price = cycle.GetProperty("pricing_scheme").GetProperty("fixed_price");
        Guard.Require(frequency.GetProperty("interval_unit").GetString() == "MONTH" && frequency.GetProperty("interval_count").GetInt32() == 1 && price.GetProperty("currency_code").GetString() == currency && decimal.Parse(price.GetProperty("value").GetString()!, CultureInfo.InvariantCulture) == amount, "conflict");
    }

    public async Task<List<ProviderTransaction>> Transactions(string id, DateTimeOffset start, DateTimeOffset end, CancellationToken token)
    {
        await Authorize(token);
        var path = "v1/billing/subscriptions/" + Uri.EscapeDataString(id) + "/transactions?start_time=" + Uri.EscapeDataString(start.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ")) + "&end_time=" + Uri.EscapeDataString(end.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        using var response = await client.GetAsync(path, token);
        Guard.Require(response.IsSuccessStatusCode, "provider_unavailable");
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync(token));
        var result = new List<ProviderTransaction>();
        foreach (var transaction in json.RootElement.GetProperty("transactions").EnumerateArray())
        {
            if (transaction.GetProperty("status").GetString() != "COMPLETED")
                continue;
            var amount = transaction.GetProperty("amount_with_breakdown").GetProperty("gross_amount");
            result.Add(new(transaction.GetProperty("id").GetString()!, decimal.Parse(amount.GetProperty("value").GetString()!, CultureInfo.InvariantCulture), amount.GetProperty("currency_code").GetString()!, DateTimeOffset.Parse(transaction.GetProperty("time").GetString()!, CultureInfo.InvariantCulture)));
        }

        return result;
    }

    public async Task<bool> VerifyWebhook(JsonElement payload, IReadOnlyDictionary<string, string> headers, CancellationToken token)
    {
        var webhookId = configuration["PayPal:WebhookId"];
        Guard.Require(!string.IsNullOrWhiteSpace(webhookId), "provider_unavailable");
        foreach (var header in new[]
        {
            "PAYPAL-AUTH-ALGO",
            "PAYPAL-CERT-URL",
            "PAYPAL-TRANSMISSION-ID",
            "PAYPAL-TRANSMISSION-SIG",
            "PAYPAL-TRANSMISSION-TIME"
        }

        )
            Guard.Require(headers.TryGetValue(header, out var value) && !string.IsNullOrWhiteSpace(value) && value.Length <= 4096, "validation", header);
        await Authorize(token);
        using var response = await client.PostAsJsonAsync("v1/notifications/verify-webhook-signature", new { auth_algo = headers["PAYPAL-AUTH-ALGO"].ToString(), cert_url = headers["PAYPAL-CERT-URL"].ToString(), transmission_id = headers["PAYPAL-TRANSMISSION-ID"].ToString(), transmission_sig = headers["PAYPAL-TRANSMISSION-SIG"].ToString(), transmission_time = headers["PAYPAL-TRANSMISSION-TIME"].ToString(), webhook_id = webhookId, webhook_event = payload }, token);
        Guard.Require(response.IsSuccessStatusCode, "provider_unavailable");
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync(token));
        return json.RootElement.GetProperty("verification_status").GetString() == "SUCCESS";
    }
}
