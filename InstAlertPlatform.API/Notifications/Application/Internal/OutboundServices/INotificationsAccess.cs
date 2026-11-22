namespace InstAlertPlatform.API.Notifications.Application.Internal.OutboundServices;
public interface INotificationsAccess
{
    Task Require(CancellationToken token);
}
