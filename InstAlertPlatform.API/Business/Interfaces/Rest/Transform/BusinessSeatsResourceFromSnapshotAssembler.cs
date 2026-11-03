using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Business.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Business.Interfaces.Rest.Transform;
public static class BusinessSeatsResourceFromSnapshotAssembler
{
    public static BusinessSeatsResource ToResourceFromSnapshot(SeatSnapshot snapshot) => new(snapshot.Limit, snapshot.ActiveEmployees, snapshot.ReservedSeats, snapshot.AvailableSeats, snapshot.SubscriptionActive);
}
