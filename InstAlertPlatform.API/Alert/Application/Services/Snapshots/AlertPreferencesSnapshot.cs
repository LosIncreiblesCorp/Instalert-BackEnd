namespace InstAlertPlatform.API.Alert.Application.Services.Snapshots;
public sealed record AlertPreferencesSnapshot(string UserId, string EmployeeId, int PanicGracePeriodSeconds);
