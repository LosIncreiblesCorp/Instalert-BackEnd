using InstAlertPlatform.API.Alert.Domain.Model.Queries;
using InstAlertPlatform.API.Alert.Application.Services;
using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Mapping.Application.Services;
using InstAlertPlatform.API.Mapping.Domain.Model.Aggregates;
using InstAlertPlatform.API.Mapping.Domain.Model.Queries;
using InstAlertPlatform.API.Mapping.Domain.Repositories;
using InstAlertPlatform.API.Mapping.Domain.Model.ValueObjects;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Mapping.Application.Internal.QueryServices;
public sealed class MappingQueryService(IIncidentProjectionRepository incidents, IBusinessProfileQueryService business, ICurrentActor actor, IAlertQueryService alerts, TimeProvider clock) : IMappingQueryService
{
    public async Task<List<IncidentSnapshot>> Handle(FindMappedIncidentsQuery query, CancellationToken cancellationToken = default)
    {
        var businessId = query.BusinessId;
        var radius = query.RadiusMeters;
        var category = query.Category;
        var since = query.Since;
        var center = await business.Handle(new RequireBusinessAccessQuery(businessId, actor.UserId, false), cancellationToken);
        Guard.Require(double.IsFinite(radius) && radius > 0 && radius <= 50000, "validation", "radiusMeters");
        var data = await Read(cancellationToken);
        return data.Where(x => (category is null || category == "all" || x.Category == category) && (since is null || x.ReportedAt >= since) && Geography.DistanceMeters(center.Latitude, center.Longitude, x.Latitude, x.Longitude) <= radius).OrderByDescending(x => x.ReportedAt).ToList();
    }

    public async Task<IncidentSnapshot> Handle(GetMappedIncidentByIdQuery query, CancellationToken cancellationToken = default)
    {
        var businessId = query.BusinessId;
        var id = query.Id;
        var radius = query.RadiusMeters;
        return (await Handle(new FindMappedIncidentsQuery(businessId, radius, null, null), cancellationToken)).SingleOrDefault(x => x.Id == id) ?? throw new DomainException("not_found");
    }

    public async Task<List<BusinessLocationSnapshot>> Handle(GetMappedBusinessesQuery query, CancellationToken cancellationToken = default)
    {
        var businessId = query.BusinessId;
        var radius = query.RadiusMeters;
        var center = await business.Handle(new RequireBusinessAccessQuery(businessId, actor.UserId, false), cancellationToken);
        Guard.Require(double.IsFinite(radius) && radius > 0 && radius <= 50000, "validation", "radiusMeters");
        return (await business.Handle(new GetBusinessLocationsQuery(), cancellationToken)).Where(b => Geography.DistanceMeters(center.Latitude, center.Longitude, b.Latitude, b.Longitude) <= radius).Select(b => new BusinessLocationSnapshot(b.Id, b.Name, b.Address, b.Latitude, b.Longitude)).ToList();
    }

    public async Task<List<RiskZoneSnapshot>> Handle(GetRiskZonesQuery query, CancellationToken cancellationToken = default)
    {
        var businessId = query.BusinessId;
        var radius = query.RadiusMeters;
        var category = query.Category;
        var since = query.Since;
        var data = await Handle(new FindMappedIncidentsQuery(businessId, radius, category, since), cancellationToken);
        var visibleCells = data.Where(x => Math.Abs(x.Latitude) < 85).Select(x => ClassifiedRiskCell.CellId(x.Latitude, x.Longitude)).ToHashSet();
        return (await Handle(new GetRiskZoneClassificationsQuery(clock.GetUtcNow(), false), cancellationToken)).Where(cell => visibleCells.Contains(cell.Id)).ToList();
    }

    public async Task<List<RiskZoneSnapshot>> Handle(GetRiskZoneClassificationsQuery query, CancellationToken cancellationToken = default)
    {
        var asOf = query.AsOf;
        var historical = query.Historical;
        List<IncidentSnapshot> source;
        if (historical)
        {
            source = [];
            foreach (var fact in await alerts.Handle(new GetAlertFactsAtQuery(asOf), cancellationToken))
            {
                var projection = new IncidentProjection(fact.Id);
                projection.Apply(fact.Revision, fact.BusinessId, fact.Kind, fact.Status, fact.ReportStatus, fact.Category, fact.Description, fact.Latitude, fact.Longitude, fact.CreatedAt);
                if (projection.Visible)
                    source.Add(new(fact.Id, fact.BusinessId, projection.Title, projection.Description, fact.Kind, fact.Status, fact.Latitude!.Value, fact.Longitude!.Value, fact.CreatedAt, fact.Category));
            }
        }
        else
            source = await Read(cancellationToken);
        return ClassifiedRiskCell.Classify(source.Select(x => new RiskObservation(x.Id, x.Latitude, x.Longitude, x.ReportedAt)), asOf).Select(cell => new RiskZoneSnapshot(cell.Id, "Incident concentration", cell.Level, cell.IncidentCount, cell.Bounds, asOf.AddDays(-ClassifiedRiskCell.WindowDays), asOf)).ToList();
    }

    public async Task<List<IncidentSnapshot>> Handle(GetAllMappedIncidentsQuery query, CancellationToken cancellationToken = default)
    {
        return await Read(cancellationToken);
    }

    private async Task<List<IncidentSnapshot>> Read(CancellationToken cancellationToken) => (await incidents.FindByVisibleAsync(true, cancellationToken)).Select(x => new IncidentSnapshot(x.Id, x.BusinessId, x.Title, x.Description, x.Type, x.Status, x.Latitude!.Value, x.Longitude!.Value, x.ReportedAt, x.Category)).ToList();
}
