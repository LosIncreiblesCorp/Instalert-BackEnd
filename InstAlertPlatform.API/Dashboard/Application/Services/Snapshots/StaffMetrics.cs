namespace InstAlertPlatform.API.Dashboard.Application.Services.Snapshots;
public sealed record StaffMetrics(int Active, int Pending, int Limit, int Available, bool OverCapacity);
