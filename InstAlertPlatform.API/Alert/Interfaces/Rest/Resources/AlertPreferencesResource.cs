namespace InstAlertPlatform.API.Alert.Interfaces.Rest.Resources;
public sealed record AlertPreferencesResource(string UserId, string EmployeeId, int PanicGracePeriodSeconds);
