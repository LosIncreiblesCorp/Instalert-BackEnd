using InstAlertPlatform.API.Payments.Domain.Model.Queries;
using InstAlertPlatform.API.Payments.Application.Services;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Business.Domain.Model.Aggregates;
using InstAlertPlatform.API.Business.Domain.Model.Commands;
using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Business.Domain.Repositories;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Business.Application.Internal.QueryServices;
public sealed class BusinessProfileQueryService(IBusinessProfileRepository businesses, IBusinessMemberRepository members, IStaffInvitationRepository invitations, IPlanQueryService limits, ICurrentActor actor) : IBusinessProfileQueryService
{
    public async Task<List<BusinessSnapshot>> Handle(GetBusinessesQuery query, CancellationToken cancellationToken = default)
    {
        var user = actor.UserId;
        return (await Handle(new GetBusinessAudienceQuery(), cancellationToken)).Where(a => a.UserId == user).Select(a => a.BusinessId).Distinct().Join(await Handle(new GetBusinessLocationsQuery(), cancellationToken), x => x, b => b.Id, (_, b) => b).ToList();
    }

    public async Task<BusinessSnapshot> Handle(GetBusinessByIdQuery query, CancellationToken cancellationToken = default)
    {
        var id = query.Id;
        return await Handle(new RequireBusinessAccessQuery(id, actor.UserId, false), cancellationToken);
    }

    public async Task<SeatSnapshot> Handle(GetBusinessSeatsQuery query, CancellationToken cancellationToken = default)
    {
        var id = query.Id;
        await Handle(new RequireBusinessAccessQuery(id, actor.UserId, true), cancellationToken);
        var usage = await Handle(new GetBusinessSeatUsageQuery(id), cancellationToken);
        var plan = await limits.Handle(new GetPlanLimitsQuery(id), cancellationToken);
        return usage with
        {
            Limit = plan.MaxEmployees,
            AvailableSeats = Math.Max(0, plan.MaxEmployees - usage.ActiveEmployees - usage.ReservedSeats),
            SubscriptionActive = plan.SubscriptionActive
        };
    }

    public async Task<BusinessSnapshot> Handle(RequireBusinessAccessQuery query, CancellationToken token = default)
    {
        var businessId = query.BusinessId;
        var userId = query.UserId;
        var administrator = query.Administrator;
        var business = await businesses.FindByIdAsync(businessId, token) ?? throw new DomainException("not_found");
        var allowed = business.OwnerUserId == userId || !administrator && (await members.FindByBusinessIdAndUserIdAndStatusAsync(businessId, userId, "active", token)).Count > 0;
        Guard.Require(allowed, "forbidden");
        return ToBusinessSnapshot(business);
    }

    public async Task<bool> Handle(GetOperationalMembershipQuery query, CancellationToken token = default)
    {
        var userId = query.UserId;
        return (await members.FindByUserIdAndStatusAsync(userId, "active", token)).Count > 0;
    }

    public async Task<List<BusinessAudience>> Handle(GetBusinessAudienceQuery query, CancellationToken token = default)
    {
        var result = new List<BusinessAudience>();
        foreach (var b in await businesses.ListAsync(token))
        {
            result.Add(new(b.Id.ToString(), b.OwnerUserId, b.Latitude, b.Longitude));
            result.AddRange((await members.FindByBusinessIdAndStatusAsync(b.Id.ToString(), "active", token)).Select(m => new BusinessAudience(b.Id.ToString(), m.UserId, b.Latitude, b.Longitude)));
        }

        return result.DistinctBy(x => (x.BusinessId, x.UserId)).ToList();
    }

    public async Task<List<BusinessSnapshot>> Handle(GetBusinessLocationsQuery query, CancellationToken token = default) => (await businesses.ListAsync(token)).Select(ToBusinessSnapshot).ToList();
    public async Task<SeatSnapshot> Handle(GetBusinessSeatUsageQuery query, CancellationToken token = default)
    {
        var businessId = query.BusinessId;
        var active = (await members.FindByBusinessIdAndStatusAsync(businessId, "active", token)).Count;
        var pending = (await invitations.FindByBusinessIdAndStatusAsync(businessId, "pending", token)).Count;
        return new(0, active, pending, 0, false);
    }

    public async Task<InvitationDeliveryState?> Handle(GetInvitationDeliveryStateQuery query, CancellationToken token = default)
    {
        var invitationId = query.InvitationId;
        var invitation = await invitations.FindByIdAsync(invitationId, token);
        return invitation is null ? null : new(invitation.Email, invitation.Status == "pending", invitation.DeliverySequence);
    }

    private static BusinessSnapshot ToBusinessSnapshot(BusinessProfile b) => new(b.Id.ToString(), b.OwnerUserId, b.TaxId.Value, b.Name, b.Address, b.Latitude, b.Longitude, b.CreatedAt, b.UpdatedAt);
}
