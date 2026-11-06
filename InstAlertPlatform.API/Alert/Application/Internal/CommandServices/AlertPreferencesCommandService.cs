using InstAlertPlatform.API.Alert.Application.Errors;
using InstAlertPlatform.API.Alert.Application.Services;
using InstAlertPlatform.API.Alert.Domain.Model.Aggregates;
using InstAlertPlatform.API.Alert.Domain.Model.Commands;
using InstAlertPlatform.API.Alert.Domain.Model.Queries;
using InstAlertPlatform.API.Alert.Domain.Repositories;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Alert.Application.Internal.CommandServices;
public sealed class AlertPreferencesCommandService(IAlertPreferencesRepository preferences, ICurrentActor actor, IUnitOfWork work) : IAlertPreferencesCommandService
{
    public async Task<Result<AlertPreferencesSnapshot, EChangePanicGracePeriodError>> Handle(ChangePanicGracePeriodCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = command.UserId;
            int? seconds = command.PanicGracePeriodSeconds;
            Guard.Require(userId == actor.UserId, "forbidden");
            var p = await preferences.FindByIdAsync(userId, cancellationToken);
            if (seconds.HasValue)
            {
                if (p is null)
                {
                    p = new AlertPreferences(userId, seconds.Value);
                    await preferences.AddAsync(p, cancellationToken);
                }
                else
                    p.Change(seconds.Value);
                await work.CompleteAsync(cancellationToken);
            }

            return new Result<AlertPreferencesSnapshot, EChangePanicGracePeriodError>.Success(new AlertPreferencesSnapshot(userId, userId, p?.PanicGracePeriodSeconds ?? 5));
        }
        catch (DomainException error)
        {
            return new Result<AlertPreferencesSnapshot, EChangePanicGracePeriodError>.Failure(error.Code switch
            {
                "validation" => EChangePanicGracePeriodError.InvalidInput,
                "unauthenticated" => EChangePanicGracePeriodError.Unauthenticated,
                "forbidden" => EChangePanicGracePeriodError.Forbidden,
                "not_found" => EChangePanicGracePeriodError.NotFound,
                "conflict" => EChangePanicGracePeriodError.Conflict,
                "concurrency" => EChangePanicGracePeriodError.ConcurrencyConflict,
                "transition" => EChangePanicGracePeriodError.InvalidTransition,
                "already_completed" => EChangePanicGracePeriodError.AlreadyCompleted,
                "unexpected" => EChangePanicGracePeriodError.UnexpectedError,
                _ => EChangePanicGracePeriodError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }
}
