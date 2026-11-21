namespace InstAlertPlatform.API.Alert.Application.Internal.OutboundServices;
public interface IAlertAccess
{
    Task Require(CancellationToken token);
}
