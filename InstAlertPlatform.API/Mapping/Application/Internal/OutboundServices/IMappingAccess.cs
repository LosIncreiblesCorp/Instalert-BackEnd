namespace InstAlertPlatform.API.Mapping.Application.Internal.OutboundServices;
public interface IMappingAccess
{
    Task Require(string businessId, CancellationToken token);
}
