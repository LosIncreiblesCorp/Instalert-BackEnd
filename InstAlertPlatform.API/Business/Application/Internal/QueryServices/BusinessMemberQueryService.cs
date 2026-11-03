using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Business.Domain.Repositories;

namespace InstAlertPlatform.API.Business.Application.Internal.QueryServices;

public sealed class BusinessMemberQueryService(
    IBusinessMemberRepository members,
    IBusinessProfileQueryService businesses) : IBusinessMemberQueryService
{
    public async Task<List<MemberSnapshot>> Handle(GetBusinessMembersQuery query,
        CancellationToken cancellationToken = default)
    {
        await businesses.Handle(
            new RequireBusinessAccessQuery(query.Id, query.UserId, OwnerOnly: true), cancellationToken);

        return (await members.FindByBusinessIdAsync(query.Id.ToString(), cancellationToken))
            .Select(member => new MemberSnapshot(member.Id, member.BusinessId.ToString(), member.UserId,
                member.DisplayName, member.Email, member.Role, member.Status, member.Version))
            .ToList();
    }
}
