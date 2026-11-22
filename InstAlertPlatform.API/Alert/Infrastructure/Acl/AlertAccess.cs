using InstAlertPlatform.API.Alert.Application.Internal.OutboundServices;
using InstAlertPlatform.API.Iam.Application.QueryServices;
using InstAlertPlatform.API.Iam.Domain.Model.Queries;
using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Alert.Infrastructure.Acl;
public sealed class AlertAccess(IUserQueryService users) : IAlertAccess
{
    public async Task Require(CancellationToken token)
    {
        var session = await users.Handle(new GetCurrentUserQuery(), token);
        Guard.Require(session.HasApplicationAccess, "forbidden");
    }
}
