namespace InstAlertPlatform.API.Notifications.Application.Errors;
public enum EChangeNotificationPreferencesError
{
    InvalidInput,
    Unauthenticated,
    Forbidden,
    NotFound,
    Conflict,
    ConcurrencyConflict,
    UnexpectedError,
}
