using InstAlertPlatform.API.Business.Application.QueryServices;
using InstAlertPlatform.API.Iam.Application.Services.Snapshots;
using InstAlertPlatform.API.Payments.Application.QueryServices;
using InstAlertPlatform.API.Profiles.Application.QueryServices;
using InstAlertPlatform.API.Iam.Application.QueryServices;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Payments.Application.Services;
using InstAlertPlatform.API.Iam.Domain.Model.Queries;
using InstAlertPlatform.API.Iam.Domain.Repositories;
using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Profiles.Domain.Model.Queries;
using InstAlertPlatform.API.Payments.Domain.Model.Queries;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Iam.Application.Internal.QueryServices;

public sealed class UserQueryService(IUserRepository users, ICurrentActor actor, IProfileQueryService profiles,
    IBusinessProfileQueryService businesses, IBusinessMemberQueryService memberships, IPlanQueryService plans) : IUserQueryService
{
    public async Task<UserSnapshot> Handle(GetCurrentUserQuery query, CancellationToken cancellationToken = default)
    {
        var user = await users.FindByIdAsync(actor.UserId, cancellationToken) ?? throw new DomainException("unauthenticated");
        var profile = await profiles.Handle(new GetUserProfileQuery(user.Id), cancellationToken);
        var profileCompleted = !string.IsNullOrWhiteSpace(profile.FirstName) && !string.IsNullOrWhiteSpace(profile.LastName) && profile.Dni.Length == 8;
        var accessible = await businesses.Handle(new GetBusinessesQuery(), cancellationToken);
        var commerce = accessible.SingleOrDefault();
        SessionBusinessSnapshot? business = null;
        if (commerce is not null)
        {
            var limit = await plans.Handle(new GetPlanLimitsQuery(commerce.Id), cancellationToken);
            business = new(commerce.Id, commerce.Name, limit.SubscriptionActive);
        }
        var membership = user.Role == "employee" ? (await memberships.Handle(new GetOwnMembershipsQuery(), cancellationToken)).SingleOrDefault() : null;
        var canEnter = !user.MustChangePassword && profileCompleted && business?.SubscriptionActive == true
            && (user.Role == "administrator" || membership?.Status == "active");
        var step = user.MustChangePassword ? "password"
            : user.Role == "administrator" && business is null ? "business"
            : !profileCompleted ? "personal-profile"
            : membership?.Status == "inactive" ? "membership-inactive"
            : business is null ? "invitation"
            : !business.SubscriptionActive ? user.Role == "administrator" ? "subscription" : "subscription-inactive"
            : "ready";
        return new(user.Id, user.Email, user.Role, user.MustChangePassword, step, canEnter, profileCompleted, business?.Id, business);
    }
}
