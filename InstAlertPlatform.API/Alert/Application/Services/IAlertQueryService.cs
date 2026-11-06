using InstAlertPlatform.API.Alert.Application.Services;
using InstAlertPlatform.API.Alert.Domain.Model.Queries;

namespace InstAlertPlatform.API.Alert.Application.Services;
public interface IAlertQueryService
{
    Task<List<AlertFact>> Handle(GetAlertsQuery query, CancellationToken cancellationToken = default);
    Task<AlertFact> Handle(GetAlertByIdQuery query, CancellationToken cancellationToken = default);
    Task<List<AlertFact>> Handle(GetAllAlertFactsQuery query, CancellationToken cancellationToken = default);
    Task<List<AlertFact>> Handle(GetAlertFactsAtQuery query, CancellationToken cancellationToken = default);
}
