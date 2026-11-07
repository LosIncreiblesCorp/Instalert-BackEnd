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
public sealed class NotificationPreferencesCommandService(INotificationPreferencesRepository preferences, IBusinessProfileQueryService business, ICurrentActor actor, IUnitOfWork work) : INotificationPreferencesCommandService
{
    public async Task<Result<PreferencesSnapshot, EChangeNotificationPreferencesError>> Handle(ChangeNotificationPreferencesCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = command.UserId;
            var businessId = command.BusinessId;
            Guard.Require(userId == actor.UserId, "forbidden");
            await business.Handle(new RequireBusinessAccessQuery(businessId, userId, false), cancellationToken);
            var userPreferences = await preferences.FindByIdAsync(businessId + ":" + userId, cancellationToken);
            if (userPreferences is null)
            {
                userPreferences = new(userId, businessId);
                await preferences.AddAsync(userPreferences, cancellationToken);
            }

            userPreferences.Update(command.NearbyAlertsEnabled, command.NearbyRadiusMeters);
            await work.CompleteAsync(cancellationToken);
            return new Result<PreferencesSnapshot, EChangeNotificationPreferencesError>.Success(new(userId, businessId, userPreferences?.NearbyAlertsEnabled ?? true, userPreferences?.NearbyRadiusMeters.Value ?? 500, userPreferences?.Version));
        }
        catch (DomainException error)
        {
            return new Result<PreferencesSnapshot, EChangeNotificationPreferencesError>.Failure(error.Code switch
            {
                "validation" => EChangeNotificationPreferencesError.InvalidInput,
                "unauthenticated" => EChangeNotificationPreferencesError.Unauthenticated,
                "forbidden" => EChangeNotificationPreferencesError.Forbidden,
                "not_found" => EChangeNotificationPreferencesError.NotFound,
                "conflict" => EChangeNotificationPreferencesError.Conflict,
                "concurrency" => EChangeNotificationPreferencesError.ConcurrencyConflict,
                "unexpected" => EChangeNotificationPreferencesError.UnexpectedError,
                _ => EChangeNotificationPreferencesError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }
}
