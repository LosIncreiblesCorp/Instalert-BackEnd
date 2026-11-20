using InstAlertPlatform.API.Business.Application.Services.Snapshots;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Business.Domain.Model.Queries;

namespace InstAlertPlatform.API.Business.Application.QueryServices;
public interface IStaffInvitationQueryService
{
    Task<List<InvitationSnapshot>> Handle(GetOwnStaffInvitationsQuery query, CancellationToken cancellationToken = default);
    Task<List<InvitationSnapshot>> Handle(GetStaffInvitationsQuery query, CancellationToken cancellationToken = default);
}
