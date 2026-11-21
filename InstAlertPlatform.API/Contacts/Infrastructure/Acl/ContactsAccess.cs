using InstAlertPlatform.API.Contacts.Application.Internal.OutboundServices;
using InstAlertPlatform.API.Iam.Application.QueryServices;
using InstAlertPlatform.API.Iam.Domain.Model.Queries;
using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Contacts.Infrastructure.Acl;
public sealed class ContactsAccess(IUserQueryService users) : IContactsAccess
{
    public async Task Require(CancellationToken token)
    {
        var session = await users.Handle(new GetCurrentUserQuery(), token);
        Guard.Require(session.HasApplicationAccess && session.Role == "employee", "forbidden");
    }
}
