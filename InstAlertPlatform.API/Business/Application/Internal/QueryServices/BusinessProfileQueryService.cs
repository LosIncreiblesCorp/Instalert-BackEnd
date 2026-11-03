using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Business.Domain.Model.Aggregates;
using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Business.Domain.Repositories;
using InstAlertPlatform.API.Payments.Application.Services;
using InstAlertPlatform.API.Payments.Domain.Model.Queries;
using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Business.Application.Internal.QueryServices;

public class BusinessProfileQueryService(
    IBusinessProfileRepository repository,
    IBusinessMemberRepository members,
    IStaffInvitationRepository invitations,
    IPlanQueryService plans) : IBusinessProfileQueryService
{
    public Task<IEnumerable<BusinessProfile>> Handle(GetBusinessProfilesByOwnerUserIdQuery query,
        CancellationToken cancellationToken = default) =>
        repository.FindByOwnerUserIdAsync(query.OwnerUserId, cancellationToken);

    public async Task<BusinessProfile?> Handle(GetBusinessProfileByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var profile = await repository.FindByIdAsync(query.Id, cancellationToken);
        if (profile is null || profile.OwnerUserId == query.OwnerUserId)
            return profile;

        var activeMemberships = await members.FindByBusinessIdAndUserIdAndStatusAsync(
            query.Id.ToString(), query.OwnerUserId, "active", cancellationToken);
        return activeMemberships.Count > 0 ? profile : null;
    }

    public async Task<List<BusinessProfile>> Handle(GetBusinessesQuery query,
        CancellationToken cancellationToken = default)
    {
        var activeMemberships = await members.FindByUserIdAndStatusAsync(
            query.UserId, "active", cancellationToken);
        var businessIds = activeMemberships.Select(member => member.BusinessId).ToHashSet();
        var businesses = await repository.ListAsync(cancellationToken);

        return businesses
            .Where(business => business.OwnerUserId == query.UserId || businessIds.Contains(business.Id))
            .OrderBy(business => business.Id)
            .ToList();
    }

    public async Task<BusinessProfile> Handle(RequireBusinessAccessQuery query,
        CancellationToken cancellationToken = default)
    {
        var profile = await repository.FindByIdAsync(query.BusinessId, cancellationToken)
            ?? throw new DomainException("not_found");
        if (profile.OwnerUserId == query.UserId)
            return profile;

        var allowed = !query.OwnerOnly && (await members.FindByBusinessIdAndUserIdAndStatusAsync(
            query.BusinessId.ToString(), query.UserId, "active", cancellationToken)).Count > 0;
        if (!allowed)
            throw new DomainException("forbidden");

        return profile;
    }

    public async Task<SeatSnapshot> Handle(GetBusinessSeatsQuery query,
        CancellationToken cancellationToken = default)
    {
        var business = await Handle(new RequireBusinessAccessQuery(query.Id, query.UserId, OwnerOnly: true), cancellationToken);
        var active = (await members.FindByBusinessIdAndStatusAsync(
            business.Id.ToString(), "active", cancellationToken)).Count;
        var pending = (await invitations.FindByBusinessIdAndStatusAsync(
            business.Id.ToString(), "pending", cancellationToken)).Count;
        var plan = await plans.Handle(new GetPlanLimitsQuery(business.Id.ToString()), cancellationToken);

        var available = plan.SubscriptionActive
            ? Math.Max(0, plan.MaxEmployees - active - pending)
            : 0;
        return new SeatSnapshot(plan.MaxEmployees, active, pending, available, plan.SubscriptionActive);
    }
}
