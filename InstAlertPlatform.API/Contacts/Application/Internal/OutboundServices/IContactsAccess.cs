namespace InstAlertPlatform.API.Contacts.Application.Internal.OutboundServices;
public interface IContactsAccess
{
    Task Require(CancellationToken token);
}
