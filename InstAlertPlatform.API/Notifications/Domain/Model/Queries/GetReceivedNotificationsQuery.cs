namespace InstAlertPlatform.API.Notifications.Domain.Model.Queries;
public sealed record GetReceivedNotificationsQuery(string BusinessId, int Page, int PageSize);
