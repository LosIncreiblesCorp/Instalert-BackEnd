namespace InstAlertPlatform.API.Payments.Application.Services;
public sealed record SubscriptionCheckoutSnapshot(string CheckoutId, string PlanId, [property: System.Text.Json.Serialization.JsonPropertyName("paypalPlanId")] string PayPalPlanId, string? CustomId, DateTimeOffset ExpiresAt, string? SubscriptionId);
