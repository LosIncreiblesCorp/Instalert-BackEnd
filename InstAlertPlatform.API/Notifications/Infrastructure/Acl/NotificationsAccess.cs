using InstAlertPlatform.API.Notifications.Application.Internal.OutboundServices;
using InstAlertPlatform.API.Iam.Application.QueryServices;
using InstAlertPlatform.API.Iam.Domain.Model.Queries;
using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Notifications.Infrastructure.Acl;
public sealed class NotificationsAccess(IUserQueryService users) : INotificationsAccess
{
    public async Task Require(CancellationToken token)
    {
        var session = await users.Handle(new GetCurrentUserQuery(), token);
        Guard.Require(session.HasApplicationAccess, "forbidden");
    }
}
