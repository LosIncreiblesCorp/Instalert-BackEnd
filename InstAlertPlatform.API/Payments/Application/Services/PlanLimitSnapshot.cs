namespace InstAlertPlatform.API.Payments.Application.Services;
public sealed record PlanLimitSnapshot(int MaxEmployees, string? PlanName, bool SubscriptionActive, DateTimeOffset? CurrentPeriodEnd = null, bool CancelAtPeriodEnd = false);
