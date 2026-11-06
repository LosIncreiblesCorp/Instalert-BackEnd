namespace InstAlertPlatform.API.Payments.Application.Services;
public sealed record SubscriptionSnapshot(string Id, string BusinessId, string PlanId, string Status, DateTimeOffset? CurrentPeriodEnd, bool CancelAtPeriodEnd, string Version, bool IsSimulated = false);
