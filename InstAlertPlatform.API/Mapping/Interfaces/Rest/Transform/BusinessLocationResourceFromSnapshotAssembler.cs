using InstAlertPlatform.API.Mapping.Application.Services;
using InstAlertPlatform.API.Mapping.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Mapping.Interfaces.Rest.Transform;
public static class BusinessLocationResourceFromSnapshotAssembler
{
    public static BusinessLocationResource ToResourceFromSnapshot(BusinessLocationSnapshot snapshot) => new(snapshot.Id, snapshot.Name, snapshot.Address, snapshot.Latitude, snapshot.Longitude);
}
