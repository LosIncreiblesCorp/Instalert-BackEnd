using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Business.Domain.Model.Queries;

namespace InstAlertPlatform.API.Business.Application.Services;
public interface IBusinessProfileQueryService
{
    Task<List<BusinessSnapshot>> Handle(GetBusinessesQuery query, CancellationToken cancellationToken = default);
    Task<BusinessSnapshot> Handle(GetBusinessByIdQuery query, CancellationToken cancellationToken = default);
    Task<SeatSnapshot> Handle(GetBusinessSeatsQuery query, CancellationToken cancellationToken = default);
    Task<BusinessSnapshot> Handle(RequireBusinessAccessQuery query, CancellationToken cancellationToken = default);
    Task<bool> Handle(GetOperationalMembershipQuery query, CancellationToken cancellationToken = default);
    Task<List<BusinessAudience>> Handle(GetBusinessAudienceQuery query, CancellationToken cancellationToken = default);
    Task<List<BusinessSnapshot>> Handle(GetBusinessLocationsQuery query, CancellationToken cancellationToken = default);
    Task<SeatSnapshot> Handle(GetBusinessSeatUsageQuery query, CancellationToken cancellationToken = default);
    Task<InvitationDeliveryState?> Handle(GetInvitationDeliveryStateQuery query, CancellationToken cancellationToken = default);
}
