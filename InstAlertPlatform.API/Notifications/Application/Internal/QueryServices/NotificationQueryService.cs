using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Notifications.Application.Services;
using InstAlertPlatform.API.Notifications.Domain.Model.Aggregates;
using InstAlertPlatform.API.Notifications.Domain.Model.Queries;
using InstAlertPlatform.API.Notifications.Domain.Repositories;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Notifications.Application.Internal.QueryServices;
public sealed class NotificationQueryService(IReceivedNotificationRepository notifications, IDeliveryRequestRepository deliveries, IBusinessProfileQueryService business, ICurrentActor actor) : INotificationQueryService
{
    public async Task<List<NotificationSnapshot>> Handle(GetReceivedNotificationsQuery query, CancellationToken cancellationToken = default)
    {
        var businessId = query.BusinessId;
        var page = query.Page;
        var pageSize = query.PageSize;
        await business.Handle(new RequireBusinessAccessQuery(businessId, actor.UserId, false), cancellationToken);
        Guard.Require(page > 0 && pageSize is >= 1 and <= 100, "validation", "pagination");
        return (await notifications.FindByRecipientUserIdAndTargetBusinessIdAsync(actor.UserId, businessId, cancellationToken)).OrderByDescending(n => n.CreatedAt).Skip((int)Math.Min(int.MaxValue, ((long)page - 1) * pageSize)).Take(pageSize).Select(ToNotificationSnapshot).ToList();
    }

    public async Task<List<DeliveryStatusSnapshot>> Handle(GetDeliveryStatusesQuery query, CancellationToken cancellationToken = default)
    {
        var user = actor.UserId;
        return (await deliveries.FindByUserIdAsync(user, cancellationToken)).Select(d => new DeliveryStatusSnapshot(d.Id, d.SourceAlertId, d.SourceInvitationId, d.Channel, d.Status, d.Attempts, d.SentAt)).ToList();
    }

    public async Task<int> Handle(GetUnreadNotificationCountQuery query, CancellationToken cancellationToken = default)
    {
        var userId = query.UserId;
        var businessId = query.BusinessId;
        Guard.Require(userId == actor.UserId, "forbidden");
        await business.Handle(new RequireBusinessAccessQuery(businessId, userId, false), cancellationToken);
        return (await notifications.FindByRecipientUserIdAndTargetBusinessIdAndReadAtAsync(userId, businessId, null, cancellationToken)).Count;
    }

    private static NotificationSnapshot ToNotificationSnapshot(ReceivedNotification n) => new(n.Id, n.RecipientUserId, n.TargetBusinessId, n.Type, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "es" ? "Emergencia cercana" : n.Title, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "es" ? "Se confirmó una alerta de pánico cerca de tu negocio." : n.Message, n.SourceAlertId, n.DistanceMeters, n.CreatedAt, n.ReadAt);
}
