namespace InstAlertPlatform.API.Business.Application.Services;
public sealed record SeatSnapshot(int Limit, int ActiveEmployees, int ReservedSeats, int AvailableSeats, bool SubscriptionActive);
