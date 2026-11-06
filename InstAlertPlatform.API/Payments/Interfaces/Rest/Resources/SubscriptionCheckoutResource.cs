namespace InstAlertPlatform.API.Payments.Interfaces.Rest.Resources;
public sealed record SubscriptionCheckoutResource(string CheckoutId, string PlanId, [property: System.Text.Json.Serialization.JsonPropertyName("paypalPlanId")] string PayPalPlanId, string? CustomId, DateTimeOffset ExpiresAt, string? SubscriptionId);
