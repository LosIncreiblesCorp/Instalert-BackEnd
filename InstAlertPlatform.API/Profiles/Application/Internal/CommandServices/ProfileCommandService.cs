using InstAlertPlatform.API.Profiles.Application.Errors;
using InstAlertPlatform.API.Profiles.Application.Services;
using InstAlertPlatform.API.Profiles.Domain.Model.Aggregates;
using InstAlertPlatform.API.Profiles.Domain.Model.Commands;
using InstAlertPlatform.API.Profiles.Domain.Repositories;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Profiles.Application.Internal.CommandServices;
public sealed class ProfileCommandService(IUserProfileRepository profiles, IUserPreferencesRepository preferences, IUnitOfWork work, ICurrentActor actor) : IProfileCommandService
{
    public async Task<Result<ProfileSnapshot, EUpdateUserProfileError>> Handle(UpdateUserProfileCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = command.UserId;
            Guard.Require(userId == actor.UserId, "forbidden");
            var profile = await profiles.FindByIdAsync(userId, cancellationToken);
            if (profile is null)
            {
                profile = new UserProfile(userId, command.FirstName, command.LastName, command.Dni, command.Phone);
                await profiles.AddAsync(profile, cancellationToken);
            }
            else profile.Update(command.FirstName, command.LastName, command.Dni, command.Phone);
            await work.CompleteAsync(cancellationToken);
            return new Result<ProfileSnapshot, EUpdateUserProfileError>.Success(ToProfileSnapshot(profile));
        }
        catch (DomainException error)
        {
            return new Result<ProfileSnapshot, EUpdateUserProfileError>.Failure(error.Code switch
            {
                "validation" => EUpdateUserProfileError.InvalidInput,
                "unauthenticated" => EUpdateUserProfileError.Unauthenticated,
                "forbidden" => EUpdateUserProfileError.Forbidden,
                "not_found" => EUpdateUserProfileError.NotFound,
                "conflict" => EUpdateUserProfileError.Conflict,
                "concurrency" => EUpdateUserProfileError.ConcurrencyConflict,
                "unexpected" => EUpdateUserProfileError.UnexpectedError,
                _ => EUpdateUserProfileError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }

    public async Task<Result<UserPreferencesSnapshot, EChangeUserLocaleError>> Handle(ChangeUserLocaleCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = command.UserId;
            var locale = command.Locale;
            Guard.Require(userId == actor.UserId, "forbidden");
            var preference = await preferences.FindByIdAsync(userId, cancellationToken);
            if (preference is null)
            {
                preference = new UserPreferences(userId, locale);
                await preferences.AddAsync(preference, cancellationToken);
            }
            else preference.ChangeLocale(locale);
            await work.CompleteAsync(cancellationToken);
            return new Result<UserPreferencesSnapshot, EChangeUserLocaleError>.Success(new(preference.Id, preference.Locale.Value, preference.UpdatedAt, preference.Version));
        }
        catch (DomainException error)
        {
            return new Result<UserPreferencesSnapshot, EChangeUserLocaleError>.Failure(error.Code switch
            {
                "validation" => EChangeUserLocaleError.InvalidInput,
                "unauthenticated" => EChangeUserLocaleError.Unauthenticated,
                "forbidden" => EChangeUserLocaleError.Forbidden,
                "not_found" => EChangeUserLocaleError.NotFound,
                "conflict" => EChangeUserLocaleError.Conflict,
                "concurrency" => EChangeUserLocaleError.ConcurrencyConflict,
                "unexpected" => EChangeUserLocaleError.UnexpectedError,
                _ => EChangeUserLocaleError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }

    private static ProfileSnapshot ToProfileSnapshot(UserProfile p) => new(p.Id, p.FirstName, p.LastName, p.Dni.Value, p.Phone, p.UpdatedAt, p.Version);
}
