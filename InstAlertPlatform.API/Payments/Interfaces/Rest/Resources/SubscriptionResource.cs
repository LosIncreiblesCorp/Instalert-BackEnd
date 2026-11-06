namespace InstAlertPlatform.API.Payments.Interfaces.Rest.Resources;
public sealed record SubscriptionResource(string Id, string BusinessId, string PlanId, string Status, DateTimeOffset? CurrentPeriodEnd, bool CancelAtPeriodEnd, string Version, bool IsSimulated = false);
