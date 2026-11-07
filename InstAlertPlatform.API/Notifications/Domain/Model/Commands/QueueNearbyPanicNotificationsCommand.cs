namespace InstAlertPlatform.API.Notifications.Domain.Model.Commands;
public sealed record QueueNearbyPanicNotificationsCommand(string Id, string EmployeeId, string Kind, string Status, double? Latitude, double? Longitude, int Revision);
