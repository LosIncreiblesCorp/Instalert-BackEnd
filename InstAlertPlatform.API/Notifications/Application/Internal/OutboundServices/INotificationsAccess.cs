namespace InstAlertPlatform.API.Notifications.Application.Internal.OutboundServices;
public interface INotificationsAccess
{
    Task Require(CancellationToken token);
    Task<bool> CanReceive(string userId, string businessId, CancellationToken token);
}
