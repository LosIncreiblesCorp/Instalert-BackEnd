using InstAlertPlatform.API.Mapping.Application.Services;
using InstAlertPlatform.API.Mapping.Domain.Model.Queries;

namespace InstAlertPlatform.API.Mapping.Application.Services;
public interface IMappingQueryService
{
    Task<List<IncidentSnapshot>> Handle(FindMappedIncidentsQuery query, CancellationToken cancellationToken = default);
    Task<IncidentSnapshot> Handle(GetMappedIncidentByIdQuery query, CancellationToken cancellationToken = default);
    Task<List<BusinessLocationSnapshot>> Handle(GetMappedBusinessesQuery query, CancellationToken cancellationToken = default);
    Task<List<RiskZoneSnapshot>> Handle(GetRiskZonesQuery query, CancellationToken cancellationToken = default);
    Task<List<RiskZoneSnapshot>> Handle(GetRiskZoneClassificationsQuery query, CancellationToken cancellationToken = default);
    Task<List<IncidentSnapshot>> Handle(GetAllMappedIncidentsQuery query, CancellationToken cancellationToken = default);
}
