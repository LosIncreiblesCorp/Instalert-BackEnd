using InstAlertPlatform.API.Dashboard.Application.Internal.OutboundServices;
using InstAlertPlatform.API.Iam.Application.QueryServices;
using InstAlertPlatform.API.Iam.Domain.Model.Queries;
using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Dashboard.Infrastructure.Acl;
public sealed class DashboardAccess(IUserQueryService users) : IDashboardAccess
{
    public async Task Require(string businessId, bool administratorOnly, CancellationToken token)
    {
        var session = await users.Handle(new GetCurrentUserQuery(), token);
        Guard.Require(session.HasApplicationAccess && session.BusinessId == businessId
            && (!administratorOnly || session.Role == "administrator"), "forbidden");
    }
}
