using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Notifications.Application.Services;
using InstAlertPlatform.API.Notifications.Domain.Model.Aggregates;
using InstAlertPlatform.API.Notifications.Domain.Model.Commands;
using InstAlertPlatform.API.Notifications.Domain.Model.Queries;
using InstAlertPlatform.API.Notifications.Domain.Repositories;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Notifications.Application.Internal.QueryServices;
public sealed class NotificationPreferencesQueryService(INotificationPreferencesRepository preferences, IBusinessProfileQueryService business, ICurrentActor actor) : INotificationPreferencesQueryService
{
    public async Task<PreferencesSnapshot> Handle(GetNotificationPreferencesQuery query, CancellationToken cancellationToken = default)
    {
        var userId = query.UserId;
        var businessId = query.BusinessId;
        Guard.Require(userId == actor.UserId, "forbidden");
        await business.Handle(new RequireBusinessAccessQuery(businessId, userId, false), cancellationToken);
        var p = await preferences.FindByIdAsync(businessId + ":" + userId, cancellationToken);
        return new PreferencesSnapshot(userId, businessId, p?.NearbyAlertsEnabled ?? true, p?.NearbyRadiusMeters.Value ?? 500, p?.Version);
    }
}
