using InstAlertPlatform.API.Alert.Application.Services;
using InstAlertPlatform.API.Alert.Domain.Model.Queries;

namespace InstAlertPlatform.API.Alert.Application.Services;
public interface IAlertPreferencesQueryService
{
    Task<AlertPreferencesSnapshot> Handle(GetPanicPreferencesQuery query, CancellationToken cancellationToken = default);
}
