namespace InstAlertPlatform.API.Dashboard.Application.Internal.OutboundServices;
public interface IDashboardAccess
{
    Task Require(string businessId, bool administratorOnly, CancellationToken token);
}
