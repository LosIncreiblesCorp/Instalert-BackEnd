namespace InstAlertPlatform.API.Payments.Interfaces.Rest.Resources;
public sealed record PlanResource(string Id, string Name, decimal Price, decimal PriceUsd, string Currency, string BillingCycle, int MaxEmployees, string ProviderPlanId);
