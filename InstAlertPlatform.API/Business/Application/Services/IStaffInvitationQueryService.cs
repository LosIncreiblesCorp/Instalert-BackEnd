using InstAlertPlatform.API.Business.Domain.Model.Queries;

namespace InstAlertPlatform.API.Business.Application.Services;

public interface IStaffInvitationQueryService
{
    Task<List<InvitationSnapshot>> Handle(GetStaffInvitationsQuery query, CancellationToken cancellationToken = default);
}
