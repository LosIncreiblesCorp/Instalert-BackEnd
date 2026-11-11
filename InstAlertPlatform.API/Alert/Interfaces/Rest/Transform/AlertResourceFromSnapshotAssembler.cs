using InstAlertPlatform.API.Alert.Application.Services;
using InstAlertPlatform.API.Alert.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Alert.Interfaces.Rest.Transform;
public static class AlertResourceFromSnapshotAssembler
{
    public static AlertResource ToResourceFromSnapshot(AlertFact snapshot) => new(snapshot.Id, snapshot.BusinessId, snapshot.EmployeeId, snapshot.Kind, snapshot.Status, snapshot.ReportStatus, snapshot.Category, snapshot.Description, snapshot.Location, snapshot.Latitude, snapshot.Longitude, snapshot.Moment, snapshot.CreatedAt, snapshot.ActivatedAt, snapshot.ResolvedAt, snapshot.CompletedAt);
}
