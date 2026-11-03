namespace InstAlertPlatform.API.Business.Interfaces.Rest.Resources;
public sealed record BusinessSeatsResource(int Limit, int ActiveEmployees, int ReservedSeats, int AvailableSeats, bool SubscriptionActive);
