using InstAlertPlatform.API.Alert.Application.Services;
using InstAlertPlatform.API.Alert.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Alert.Interfaces.Rest.Transform;
public static class AlertPreferencesResourceFromSnapshotAssembler
{
    public static AlertPreferencesResource ToResourceFromSnapshot(AlertPreferencesSnapshot snapshot) => new(snapshot.UserId, snapshot.EmployeeId, snapshot.PanicGracePeriodSeconds);
}
