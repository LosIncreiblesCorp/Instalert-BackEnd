namespace InstAlertPlatform.API.Dashboard.Application.Services;
public sealed record DashboardMetrics(int ActiveAlerts, int PendingReports, int Reports, int PreviousReports, int NearbyIncidents, int? UnreadNotifications, int InvalidLocationReports, int UndatedReports, string? LocalRiskLevel);
