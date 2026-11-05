namespace InstAlertPlatform.API.Alert.Domain.Model.Queries;
public sealed record GetAlertsQuery(string BusinessId, string? Status, int Page, int PageSize);
