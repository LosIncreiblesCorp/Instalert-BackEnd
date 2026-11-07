namespace InstAlertPlatform.API.Dashboard.Application.Services;
public sealed record StaffMetrics(int Active, int Pending, int Limit, int Available, bool OverCapacity);
