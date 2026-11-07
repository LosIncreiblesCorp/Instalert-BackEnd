using InstAlertPlatform.API.Mapping.Application.Services;
using InstAlertPlatform.API.Mapping.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Mapping.Interfaces.Rest.Transform;
public static class RiskZoneResourceFromSnapshotAssembler
{
    public static RiskZoneResource ToResourceFromSnapshot(RiskZoneSnapshot snapshot) => new(snapshot.Id, snapshot.Name, snapshot.RiskLevel, snapshot.IncidentCount, snapshot.Bounds, snapshot.WindowStart, snapshot.WindowEnd);
}
