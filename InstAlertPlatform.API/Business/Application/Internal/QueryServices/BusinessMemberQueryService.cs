using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Business.Domain.Model.Aggregates;
using InstAlertPlatform.API.Business.Domain.Model.Commands;
using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Business.Domain.Repositories;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Business.Application.Internal.QueryServices;
public sealed class BusinessMemberQueryService(IBusinessMemberRepository members, IBusinessProfileQueryService access, ICurrentActor actor) : IBusinessMemberQueryService
{
    public async Task<List<MembershipStatusSnapshot>> Handle(GetOwnMembershipsQuery query, CancellationToken cancellationToken = default)
    {
        var userId = actor.UserId;
        return (await members.FindByUserIdAsync(userId, cancellationToken)).OrderBy(m => m.BusinessId.ToString()).Select(m => new MembershipStatusSnapshot(m.BusinessId.ToString(), m.Status)).ToList();
    }

    public async Task<List<MemberSnapshot>> Handle(GetBusinessMembersQuery query, CancellationToken cancellationToken = default)
    {
        var id = query.Id;
        await access.Handle(new RequireBusinessAccessQuery(id, actor.UserId, true), cancellationToken);
        return (await members.FindByBusinessIdAsync(id, cancellationToken)).Select(ToMemberSnapshot).ToList();
    }

    private static MemberSnapshot ToMemberSnapshot(BusinessMember m) => new(m.Id, m.BusinessId.ToString(), m.UserId, m.DisplayName, m.Email, m.Role, m.Status, m.Version);
}
