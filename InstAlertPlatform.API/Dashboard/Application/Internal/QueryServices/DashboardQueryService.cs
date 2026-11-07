using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Dashboard.Application.Services;
using InstAlertPlatform.API.Dashboard.Domain.Model.ValueObjects;
using InstAlertPlatform.API.Dashboard.Domain.Model.Queries;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Dashboard.Application.Internal.QueryServices;
public sealed class DashboardQueryService(IBusinessProfileQueryService business, ICurrentActor actor, TimeProvider clock, DashboardProjectionBuilder projectionBuilder) : IDashboardQueryService
{
    public async Task<DashboardSnapshot> Handle(GetCurrentDashboardQuery query, CancellationToken cancellationToken = default)
    {
        var businessId = query.BusinessId;
        var radius = query.RadiusMeters;
        var b = await business.Handle(new RequireBusinessAccessQuery(businessId, actor.UserId, false), cancellationToken);
        var admin = b.OwnerUserId == actor.UserId;
        return await projectionBuilder.Compose(b, admin ? "administrator" : "employee", actor.UserId, ReportingPeriod.CurrentMonth(clock.GetUtcNow()), clock.GetUtcNow(), radius, false, cancellationToken);
    }

}
