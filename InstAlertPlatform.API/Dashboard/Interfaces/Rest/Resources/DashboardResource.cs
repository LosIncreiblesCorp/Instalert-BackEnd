using InstAlertPlatform.API.Dashboard.Application.Services;

namespace InstAlertPlatform.API.Dashboard.Interfaces.Rest.Resources;
public sealed record DashboardResource(string Role, string BusinessId, string? UserId, string BusinessName, DashboardPeriod Period, int RadiusMeters, DateTimeOffset RefreshedAt, DashboardMetrics Metrics, StaffMetrics? Staff, object? Subscription, List<SeriesPoint> Daily, List<SeriesPoint> Categories, List<SeriesPoint> Statuses, List<SeriesPoint> Hourly, List<SeriesPoint> RiskCounts, object[] RiskZones, object[]? Activity, string[] Warnings);
