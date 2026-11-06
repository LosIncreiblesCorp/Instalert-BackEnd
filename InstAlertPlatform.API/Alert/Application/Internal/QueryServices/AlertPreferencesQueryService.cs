using InstAlertPlatform.API.Alert.Application.Services;
using InstAlertPlatform.API.Alert.Domain.Model.Aggregates;
using InstAlertPlatform.API.Alert.Domain.Model.Commands;
using InstAlertPlatform.API.Alert.Domain.Model.Queries;
using InstAlertPlatform.API.Alert.Domain.Repositories;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Alert.Application.Internal.QueryServices;
public sealed class AlertPreferencesQueryService(IAlertPreferencesRepository preferences, ICurrentActor actor) : IAlertPreferencesQueryService
{
    public async Task<AlertPreferencesSnapshot> Handle(GetPanicPreferencesQuery query, CancellationToken cancellationToken = default)
    {
        var userId = query.UserId;
        Guard.Require(userId == actor.UserId, "forbidden");
        var p = await preferences.FindByIdAsync(userId, cancellationToken);
        return new AlertPreferencesSnapshot(userId, userId, p?.PanicGracePeriodSeconds ?? 5);
    }
}
