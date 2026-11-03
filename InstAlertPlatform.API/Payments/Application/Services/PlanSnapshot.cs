namespace InstAlertPlatform.API.Payments.Application.Services;
public sealed record PlanSnapshot(string Id, string Name, decimal Price, decimal PriceUsd, string Currency, string BillingCycle, int MaxEmployees, string ProviderPlanId);
