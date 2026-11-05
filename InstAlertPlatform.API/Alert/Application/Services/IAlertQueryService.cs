using InstAlertPlatform.API.Alert.Domain.Model.Queries;

namespace InstAlertPlatform.API.Alert.Application.Services;
public interface IAlertQueryService
{
    Task<List<AlertFact>> Handle(GetAlertsQuery query, CancellationToken cancellationToken = default);
    Task<AlertFact> Handle(GetAlertByIdQuery query, CancellationToken cancellationToken = default);
}
