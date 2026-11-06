namespace InstAlertPlatform.API.Payments.Domain.Model.Commands;
public sealed record ChangeSubscriptionPlanCommand(string BusinessId, string PlanId);
