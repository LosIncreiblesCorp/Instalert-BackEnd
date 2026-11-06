using InstAlertPlatform.API.Alert.Application.Services;
using InstAlertPlatform.API.Alert.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Alert.Interfaces.Rest.Transform;
public static class PanicPreparationResourceFromSnapshotAssembler
{
    public static PanicPreparationResource ToResourceFromSnapshot(PanicPreparationSnapshot snapshot) => new(snapshot.PreparationToken, snapshot.NotBefore, snapshot.ExpiresAt, snapshot.GracePeriodSeconds);
}
