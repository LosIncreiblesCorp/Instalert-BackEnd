using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Business.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Business.Interfaces.Rest.Transform;
public static class BusinessResourceFromSnapshotAssembler
{
    public static BusinessResource ToResourceFromSnapshot(BusinessSnapshot snapshot) => new(snapshot.Id, snapshot.OwnerUserId, snapshot.TaxId, snapshot.Name, snapshot.Address, snapshot.Latitude, snapshot.Longitude, snapshot.CreatedAt, snapshot.UpdatedAt);
}
