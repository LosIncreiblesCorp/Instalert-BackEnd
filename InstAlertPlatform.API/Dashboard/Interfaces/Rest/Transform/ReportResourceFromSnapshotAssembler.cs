using InstAlertPlatform.API.Dashboard.Application.Services;
using InstAlertPlatform.API.Dashboard.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Dashboard.Interfaces.Rest.Transform;
public static class ReportResourceFromSnapshotAssembler
{
    public static ReportResource ToResourceFromSnapshot(ReportSnapshot snapshot) => new(snapshot.Id, snapshot.BusinessId, snapshot.StartDate, snapshot.EndDate, snapshot.CutoffAt, snapshot.GeneratedAt, snapshot.GenerationSource, snapshot.GeneratedByUserId, snapshot.SchemaVersion, snapshot.Summary);
}
