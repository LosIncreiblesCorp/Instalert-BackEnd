using InstAlertPlatform.API.Dashboard.Application.Services;
using InstAlertPlatform.API.Dashboard.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Dashboard.Interfaces.Rest.Transform;
public static class DashboardResourceFromSnapshotAssembler
{
    public static DashboardResource ToResourceFromSnapshot(DashboardSnapshot snapshot) => new(snapshot.Role, snapshot.BusinessId, snapshot.UserId, snapshot.BusinessName, snapshot.Period, snapshot.RadiusMeters, snapshot.RefreshedAt, snapshot.Metrics, snapshot.Staff, snapshot.Subscription, snapshot.Daily, snapshot.Categories, snapshot.Statuses, snapshot.Hourly, snapshot.RiskCounts, snapshot.RiskZones, snapshot.Activity, snapshot.Warnings);
}
