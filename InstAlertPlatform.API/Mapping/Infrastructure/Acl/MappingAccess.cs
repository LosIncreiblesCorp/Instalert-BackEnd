using InstAlertPlatform.API.Mapping.Application.Internal.OutboundServices;
using InstAlertPlatform.API.Iam.Application.QueryServices;
using InstAlertPlatform.API.Iam.Domain.Model.Queries;
using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Mapping.Infrastructure.Acl;
public sealed class MappingAccess(IUserQueryService users) : IMappingAccess
{
    public async Task Require(string businessId, CancellationToken token)
    {
        var session = await users.Handle(new GetCurrentUserQuery(), token);
        Guard.Require(session.HasApplicationAccess && session.BusinessId == businessId, "forbidden");
    }
}
