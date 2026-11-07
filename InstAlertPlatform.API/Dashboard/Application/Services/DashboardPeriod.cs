namespace InstAlertPlatform.API.Dashboard.Application.Services;
public sealed record DashboardPeriod(string Kind, string? Month, DateTimeOffset Start, DateTimeOffset End, DateTimeOffset PreviousStart, DateTimeOffset PreviousEnd, string TimeZone);
