namespace InstAlertPlatform.API.Alert.Application.Services;
public sealed record AlertPreferencesSnapshot(string UserId, string EmployeeId, int PanicGracePeriodSeconds);
