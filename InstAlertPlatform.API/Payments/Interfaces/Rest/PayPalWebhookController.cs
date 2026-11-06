using InstAlertPlatform.API.Payments.Interfaces.Rest.Resources;
using InstAlertPlatform.API.Resources;
using Microsoft.Extensions.Localization;
using InstAlertPlatform.API.Payments.Interfaces.Rest.Transform;
using InstAlertPlatform.API.Payments.Application.Services;
using InstAlertPlatform.API.Payments.Domain.Model.Commands;
using InstAlertPlatform.API.Shared.Application.Patterns;
using System.Text.Json;
using InstAlertPlatform.API.Shared.Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace InstAlertPlatform.API.Payments.Interfaces.Rest;
[ApiController, Route("api/v1/payments/paypal/webhook"), Tags("Payments")]
[ProducesResponseType(typeof(ProblemDetails), 500)]
public sealed class PayPalWebhookController(IPayPalWebhookVerifier provider, IPaymentReconciliationCommandService reconciliation, IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpPost, RequestSizeLimit(65536)]
    [ProducesResponseType(typeof(PayPalWebhookResource), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 403)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    [ProducesResponseType(typeof(ProblemDetails), 503)]
    public async Task<IActionResult> Receive(JsonElement payload, CancellationToken cancellationToken)
    {
        Guard.Require(await provider.VerifyWebhook(payload, Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString(), StringComparer.OrdinalIgnoreCase), cancellationToken), "forbidden");
        Guard.Require(payload.TryGetProperty("id", out var id) && payload.TryGetProperty("event_type", out _), "validation");
        var type = payload.GetProperty("event_type").GetString()!;
        if (type is not ("PAYMENT.SALE.COMPLETED" or "BILLING.SUBSCRIPTION.ACTIVATED" or "BILLING.SUBSCRIPTION.CANCELLED" or "BILLING.SUBSCRIPTION.UPDATED"))
            return Ok(new PayPalWebhookResource(true, false));
        var resource = payload.GetProperty("resource");
        var subscriptionId = resource.TryGetProperty("billing_agreement_id", out var agreement) ? agreement.GetString() : resource.GetProperty("id").GetString();
        Guard.Require(!string.IsNullOrWhiteSpace(subscriptionId), "validation");
        var result = await reconciliation.Handle(new ReconcilePaymentCommand(id.GetString()!, type, subscriptionId!), cancellationToken);
        return ActionResultFromReconcilePaymentResultAssembler.ToActionResultFromReconcilePaymentResult(result, this, localizer, _ => Ok(new PayPalWebhookResource(true, true)));
    }
}
