using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Dashboard.Application.Services;
using InstAlertPlatform.API.Dashboard.Domain.Model.ValueObjects;
using InstAlertPlatform.API.Dashboard.Application.Internal.QueryServices;
using InstAlertPlatform.API.Dashboard.Domain.Model.Aggregates;
using InstAlertPlatform.API.Dashboard.Domain.Model.Queries;
using InstAlertPlatform.API.Dashboard.Domain.Repositories;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Shared.Domain.Repositories;
using System.Text.Json;

namespace InstAlertPlatform.API.Dashboard.Application.Internal.QueryServices;
public sealed class DashboardQueryService(IBusinessProfileQueryService business, IReportRepository reports, ICurrentActor actor, TimeProvider clock, DashboardProjectionBuilder projectionBuilder) : IDashboardQueryService
{
    public async Task<DashboardSnapshot> Handle(GetCurrentDashboardQuery query, CancellationToken cancellationToken = default)
    {
        var businessId = query.BusinessId;
        var radius = query.RadiusMeters;
        var b = await business.Handle(new RequireBusinessAccessQuery(businessId, actor.UserId, false), cancellationToken);
        var admin = b.OwnerUserId == actor.UserId;
        return await projectionBuilder.Compose(b, admin ? "administrator" : "employee", actor.UserId, ReportingPeriod.CurrentMonth(clock.GetUtcNow()), clock.GetUtcNow(), radius, false, cancellationToken);
    }

    public async Task<List<ReportSnapshot>> Handle(GetReportsQuery query, CancellationToken cancellationToken = default)
    {
        var businessId = query.BusinessId;
        await business.Handle(new RequireBusinessAccessQuery(businessId, actor.UserId, true), cancellationToken);
        return (await reports.FindByBusinessIdAsync(businessId, cancellationToken)).OrderByDescending(r => r.GeneratedAt).ThenByDescending(r => r.Id).Select(ToReportSnapshot).ToList();
    }

    public async Task<ReportSnapshot> Handle(GetReportByIdQuery query, CancellationToken cancellationToken = default)
    {
        var businessId = query.BusinessId;
        await business.Handle(new RequireBusinessAccessQuery(businessId, actor.UserId, true), cancellationToken);
        var report = await reports.FindByIdAsync(query.Id, cancellationToken) ?? throw new DomainException("not_found");
        Guard.Require(report.BusinessId == businessId, "not_found");
        return ToReportSnapshot(report);
    }

    private static ReportSnapshot ToReportSnapshot(Report r) => new(r.Id, r.BusinessId, r.StartDate, r.EndDate, r.CutoffAt, r.GeneratedAt, r.GenerationSource, r.GeneratedByUserId, r.SchemaVersion, JsonSerializer.Deserialize<JsonElement>(r.SummaryJson));
}
