using InstAlertPlatform.API.Mapping.Application.Services;
using InstAlertPlatform.API.Mapping.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Mapping.Interfaces.Rest.Transform;
public static class IncidentResourceFromSnapshotAssembler
{
    public static IncidentResource ToResourceFromSnapshot(IncidentSnapshot snapshot) => new(snapshot.Id, snapshot.BusinessId, snapshot.Title, snapshot.Description, snapshot.Type, snapshot.Status, snapshot.Latitude, snapshot.Longitude, snapshot.ReportedAt, snapshot.Category);
}
