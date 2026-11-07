using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Notifications.Application.Errors;
using InstAlertPlatform.API.Notifications.Application.Services;
using InstAlertPlatform.API.Notifications.Domain.Model.Aggregates;
using InstAlertPlatform.API.Notifications.Domain.Model.Commands;
using InstAlertPlatform.API.Notifications.Domain.Model.Queries;
using InstAlertPlatform.API.Notifications.Domain.Repositories;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Notifications.Application.Internal.CommandServices;
public sealed class NotificationCommandService(IReceivedNotificationRepository notifications, IBusinessProfileQueryService business, ICurrentActor actor, IUnitOfWork work, TimeProvider clock) : INotificationCommandService
{
    public async Task<Result<NotificationSnapshot, EReadNotificationError>> Handle(ReadNotificationCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var id = command.Id;
            var userId = actor.UserId;
            var n = await notifications.FindByIdAsync(id, cancellationToken);
            Guard.Require(n is not null && n.RecipientUserId == userId, "not_found");
            await business.Handle(new RequireBusinessAccessQuery(n!.TargetBusinessId, userId, false), cancellationToken);
            n.Read(clock.GetUtcNow());
            await work.CompleteAsync(cancellationToken);
            return new Result<NotificationSnapshot, EReadNotificationError>.Success(ToNotificationSnapshot(n));
        }
        catch (DomainException error)
        {
            return new Result<NotificationSnapshot, EReadNotificationError>.Failure(error.Code switch
            {
                "validation" => EReadNotificationError.InvalidInput,
                "unauthenticated" => EReadNotificationError.Unauthenticated,
                "forbidden" => EReadNotificationError.Forbidden,
                "not_found" => EReadNotificationError.NotFound,
                "conflict" => EReadNotificationError.Conflict,
                "concurrency" => EReadNotificationError.ConcurrencyConflict,
                "unexpected" => EReadNotificationError.UnexpectedError,
                _ => EReadNotificationError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }

    private static NotificationSnapshot ToNotificationSnapshot(ReceivedNotification n) => new(n.Id, n.RecipientUserId, n.TargetBusinessId, n.Type, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "es" ? "Emergencia cercana" : n.Title, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "es" ? "Se confirmó una alerta de pánico cerca de tu negocio." : n.Message, n.SourceAlertId, n.DistanceMeters, n.CreatedAt, n.ReadAt);
}
