using InstAlertPlatform.API.Business.Domain.Model.Queries;

namespace InstAlertPlatform.API.Business.Application.Services;

public interface IBusinessMemberQueryService
{
    Task<List<MemberSnapshot>> Handle(GetBusinessMembersQuery query, CancellationToken cancellationToken = default);
}
