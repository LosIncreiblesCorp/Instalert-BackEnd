using InstAlertPlatform.API.Mapping.Application.Services;
using InstAlertPlatform.API.Mapping.Domain.Model.Queries;
using InstAlertPlatform.API.Mapping.Interfaces.Rest.Resources;
using InstAlertPlatform.API.Mapping.Interfaces.Rest.Transform;
using InstAlertPlatform.API.Shared.Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace InstAlertPlatform.API.Mapping.Interfaces.Rest;
[ApiController, Route("api/v1/[controller]"), Tags("Mapping")]
[ProducesResponseType(typeof(ProblemDetails), 400), ProducesResponseType(typeof(ProblemDetails), 401), ProducesResponseType(typeof(ProblemDetails), 403), ProducesResponseType(typeof(ProblemDetails), 404), ProducesResponseType(typeof(ProblemDetails), 409), ProducesResponseType(typeof(ProblemDetails), 503)]
[ProducesResponseType(typeof(ProblemDetails), 500)]
public sealed class MappingController(IMappingQueryService mappingQueries) : ControllerBase
{
    [HttpGet("active-alerts"), ProducesResponseType(typeof(List<IncidentResource>), 200)]
    public async Task<IActionResult> ActiveAlerts(string businessId, CancellationToken cancellationToken, double radiusMeters = 500)
    {
        var snapshot = await mappingQueries.Handle(new FindMappedIncidentsQuery(businessId, radiusMeters, null, null), cancellationToken);
        snapshot = snapshot.Where(item => item.Type == "panic" && item.Status == "active").ToList();
        return Ok(snapshot.Select(IncidentResourceFromSnapshotAssembler.ToResourceFromSnapshot).ToList());
    }

    [HttpGet("incidents")]
    [ProducesResponseType(typeof(List<IncidentResource>), 200)]
    public async Task<IActionResult> Incidents(string businessId, CancellationToken cancellationToken, double radiusMeters = 1000, string? category = null, DateTimeOffset? since = null)
    {
        var snapshot = await mappingQueries.Handle(new FindMappedIncidentsQuery(businessId, radiusMeters, category, since), cancellationToken);
        return Ok(snapshot.Select(IncidentResourceFromSnapshotAssembler.ToResourceFromSnapshot).ToList());
    }

    [HttpGet("incidents/{id}")]
    [ProducesResponseType(typeof(IncidentResource), 200)]
    public async Task<IActionResult> Incident(string businessId, string id, CancellationToken cancellationToken, double radiusMeters = 1000)
    {
        var snapshot = await mappingQueries.Handle(new GetMappedIncidentByIdQuery(businessId, id, radiusMeters), cancellationToken);
        return Ok(IncidentResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot));
    }

    [HttpGet("businesses")]
    [ProducesResponseType(typeof(List<BusinessLocationResource>), 200)]
    public async Task<IActionResult> Businesses(string businessId, CancellationToken cancellationToken, double radiusMeters = 1000)
    {
        var snapshot = await mappingQueries.Handle(new GetMappedBusinessesQuery(businessId, radiusMeters), cancellationToken);
        return Ok(snapshot.Select(BusinessLocationResourceFromSnapshotAssembler.ToResourceFromSnapshot).ToList());
    }

    [HttpGet("risk-zones")]
    [ProducesResponseType(typeof(List<RiskZoneResource>), 200)]
    public async Task<IActionResult> Zones(string businessId, CancellationToken cancellationToken, double radiusMeters = 1000, string? category = null, DateTimeOffset? since = null)
    {
        var snapshot = await mappingQueries.Handle(new GetRiskZonesQuery(businessId, radiusMeters, category, since), cancellationToken);
        return Ok(snapshot.Select(RiskZoneResourceFromSnapshotAssembler.ToResourceFromSnapshot).ToList());
    }

    [HttpGet("risk-zones/{id}")]
    [ProducesResponseType(typeof(RiskZoneResource), 200)]
    public async Task<IActionResult> Zone(string businessId, string id, CancellationToken cancellationToken, double radiusMeters = 1000, string? category = null, DateTimeOffset? since = null)
    {
        var snapshot = await mappingQueries.Handle(new GetRiskZonesQuery(businessId, radiusMeters, category, since), cancellationToken);
        var zone = snapshot.SingleOrDefault(item => item.Id == id) ?? throw new DomainException("not_found");
        return Ok(RiskZoneResourceFromSnapshotAssembler.ToResourceFromSnapshot(zone));
    }
}
