using InstAlertPlatform.API.Mapping.Domain.Model.Queries;
using InstAlertPlatform.API.Mapping.Application.Services;
using InstAlertPlatform.API.Notifications.Domain.Model.Queries;
using InstAlertPlatform.API.Notifications.Application.Services;
using InstAlertPlatform.API.Alert.Domain.Model.Queries;
using InstAlertPlatform.API.Alert.Application.Services;
using InstAlertPlatform.API.Payments.Domain.Model.Queries;
using InstAlertPlatform.API.Payments.Application.Services;
using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Dashboard.Application.Services;
using InstAlertPlatform.API.Dashboard.Domain.Model.ValueObjects;
using InstAlertPlatform.API.Dashboard.Domain.Repositories;
using InstAlertPlatform.API.Shared.Application.Patterns;
using System.Text.Json;
using InstAlertPlatform.API.Dashboard.Domain.Model.Aggregates;
using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Dashboard.Application.Internal.QueryServices;
public sealed class DashboardProjectionBuilder(IAlertQueryService alerts, IBusinessProfileQueryService business, IPlanQueryService limits, INotificationQueryService notifications, TimeProvider clock, IMappingQueryService mapping)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    public async Task<DashboardSnapshot> Compose(BusinessSnapshot b, string role, string? userId, string month, DateTimeOffset cutoff, int radius, bool closed, CancellationToken token)
    {
        Guard.Require(radius is 100 or 500 or 1000, "validation", "radiusMeters");
        var bounds = ReportingPeriod.Bounds(month);
        var previous = bounds.Start.AddMonths(-1);
        var elapsed = cutoff - bounds.Start;
        var previousEnd = closed ? bounds.Start : new[]
        {
            previous + elapsed,
            bounds.Start
        }.Min();
        return await ComposePeriod(b, role, userId, new("month", month, bounds.Start, cutoff, previous, previousEnd, "America/Lima"), radius, closed, token);
    }

    public Task<DashboardSnapshot> ComposeReport(BusinessSnapshot b, string userId, ReportingPeriod bounds, CancellationToken token)
    {
        var elapsed = bounds.End - bounds.Start;
        return ComposePeriod(b, "administrator", userId, new("range", null, bounds.Start, bounds.End, bounds.Start - elapsed, bounds.Start, "America/Lima"), 500, true, token);
    }

    private async Task<DashboardSnapshot> ComposePeriod(BusinessSnapshot b, string role, string? userId, DashboardPeriod period, int radius, bool closed, CancellationToken token)
    {
        var cutoff = period.End;
        var source = closed ? await alerts.Handle(new GetAlertFactsAtQuery(cutoff), token) : await alerts.Handle(new GetAllAlertFactsQuery(), token);
        var scoped = source.Where(a => a.BusinessId == b.Id && (role == "administrator" || a.EmployeeId == userId) && a.CreatedAt < cutoff).ToList();
        var current = scoped.Where(a => a.CreatedAt >= period.Start && a.CreatedAt < cutoff).ToList();
        var prior = scoped.Where(a => a.CreatedAt >= period.PreviousStart && a.CreatedAt < period.PreviousEnd).ToList();
        var nearby = source.Where(a => a.CreatedAt >= period.Start && a.CreatedAt < cutoff && a.Latitude.HasValue && a.Longitude.HasValue && Geography.DistanceMeters(b.Latitude, b.Longitude, a.Latitude.Value, a.Longitude.Value) <= radius).ToList();
        var classifiedZones = await mapping.Handle(new GetRiskZoneClassificationsQuery(cutoff, closed), token);
        string? RiskAt(double latitude, double longitude) => classifiedZones.FirstOrDefault(zone => longitude >= zone.Bounds[0][0] && longitude < zone.Bounds[1][0] && latitude >= zone.Bounds[0][1] && latitude < zone.Bounds[2][1])?.RiskLevel;
        var riskCounts = nearby.GroupBy(a => RiskAt(a.Latitude!.Value, a.Longitude!.Value) ?? "unknown").Select(group => new SeriesPoint(group.Key, group.Count())).OrderBy(row => row.Key).ToList();
        var nearbyZones = classifiedZones.Where(zone => Geography.DistanceMeters(b.Latitude, b.Longitude, Math.Clamp(b.Latitude, zone.Bounds[0][1], zone.Bounds[2][1]), Math.Clamp(b.Longitude, zone.Bounds[0][0], zone.Bounds[1][0])) <= radius).OrderByDescending(zone => zone.RiskLevel == "high" ? 3 : zone.RiskLevel == "medium" ? 2 : 1).ThenByDescending(zone => zone.IncidentCount).ThenBy(zone => zone.Id).Take(5).Select(zone => (object)new { id = zone.Id, name = zone.Name, riskLevel = zone.RiskLevel, incidentCount = zone.IncidentCount }).ToArray();
        StaffMetrics? staff = null;
        object? subscription = null;
        if (role == "administrator" && !closed)
        {
            var usage = await business.Handle(new GetBusinessSeatUsageQuery(b.Id), token);
            var plan = await limits.Handle(new GetPlanLimitsQuery(b.Id), token);
            staff = new(usage.ActiveEmployees, usage.ReservedSeats, plan.MaxEmployees, Math.Max(0, plan.MaxEmployees - usage.ActiveEmployees - usage.ReservedSeats), usage.ActiveEmployees + usage.ReservedSeats > plan.MaxEmployees);
            subscription = new
            {
                name = plan.PlanName,
                status = plan.SubscriptionActive ? "active" : "none",
                currentPeriodEnd = plan.CurrentPeriodEnd,
                cancelAtPeriodEnd = plan.CancelAtPeriodEnd
            };
        }

        static List<SeriesPoint> Group(IEnumerable<AlertFact> rows, Func<AlertFact, string> key) => rows.GroupBy(key).Select(g => new SeriesPoint(g.Key, g.Count())).OrderBy(x => x.Key).ToList();
        static DateTimeOffset Lima(DateTimeOffset value) => TimeZoneInfo.ConvertTime(value, TimeZoneInfo.FindSystemTimeZoneById("America/Lima"));
        var daily = new List<SeriesPoint>();
        for (var day = period.Start; day < cutoff; day = day.AddDays(1))
            daily.Add(new(day.ToString("yyyy-MM-dd"), current.Count(a => Lima(a.CreatedAt).Date == day.Date)));
        var metrics = new DashboardMetrics(scoped.Count(a => a.Kind == "panic" && a.Status == "active"), scoped.Count(a => a.Kind == "panic" && a.Status == "resolved" && a.ReportStatus == "pending"), current.Count, prior.Count, nearby.Count, closed ? null : await notifications.Handle(new GetUnreadNotificationCountQuery(userId!, b.Id), token), current.Count(a => !a.Latitude.HasValue), 0, RiskAt(b.Latitude, b.Longitude));
        return new(role, b.Id, userId, b.Name, period, radius, closed ? cutoff : clock.GetUtcNow(), metrics, staff, subscription, daily, Group(current, a => a.Category.Length == 0 ? "other" : a.Category), Group(current, a => a.Status), Enumerable.Range(0, 24).Select(h => new SeriesPoint(h.ToString("00"), current.Count(a => Lima(a.CreatedAt).Hour == h))).ToList(), riskCounts, nearbyZones, closed ? null : current.OrderByDescending(a => a.CreatedAt).Take(8).Select(a => (object)new { id = a.Id, kind = a.Kind, status = a.Status, category = a.Category, registeredAt = a.CreatedAt }).ToArray(), riskCounts.Any(row => row.Key == "unknown" && row.Value > 0) ? ["classification"] : []);
    }
}
