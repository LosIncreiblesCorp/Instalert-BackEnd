using InstAlertPlatform.API.Notifications.Application.Internal.OutboundServices;
using InstAlertPlatform.API.Business.Application.QueryServices;
using InstAlertPlatform.API.Notifications.Application.QueryServices;
using InstAlertPlatform.API.Notifications.Application.Services.Snapshots;
using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Notifications.Domain.Model.Aggregates;
using InstAlertPlatform.API.Notifications.Domain.Model.Commands;
using InstAlertPlatform.API.Notifications.Domain.Model.Queries;
using InstAlertPlatform.API.Notifications.Domain.Repositories;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Notifications.Application.Internal.QueryServices;
public sealed class NotificationPreferencesQueryService(INotificationPreferencesRepository preferences, IBusinessProfileQueryService business, ICurrentActor actor, INotificationsAccess access) : INotificationPreferencesQueryService
{
    public async Task<PreferencesSnapshot> Handle(GetNotificationPreferencesQuery query, CancellationToken cancellationToken = default)
    {
        var userId = query.UserId;
        await access.Require(cancellationToken);
        var businessId = query.BusinessId;
        Guard.Require(userId == actor.UserId, "forbidden");
        await business.Handle(new RequireBusinessAccessQuery(businessId, userId, false), cancellationToken);
        var p = await preferences.FindByIdAsync(businessId + ":" + userId, cancellationToken);
        return new PreferencesSnapshot(userId, businessId, p?.NearbyAlertsEnabled ?? true, p?.NearbyRadiusMeters.Value ?? 500, p?.Version);
    }
}
