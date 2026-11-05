using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Business.Domain.Model.Queries;

namespace InstAlertPlatform.API.Business.Application.Services;
public interface IBusinessMemberQueryService
{
    Task<List<MembershipStatusSnapshot>> Handle(GetOwnMembershipsQuery query, CancellationToken cancellationToken = default);
    Task<List<MemberSnapshot>> Handle(GetBusinessMembersQuery query, CancellationToken cancellationToken = default);
}
