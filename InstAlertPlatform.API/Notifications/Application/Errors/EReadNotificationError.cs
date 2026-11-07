namespace InstAlertPlatform.API.Notifications.Application.Errors;
public enum EReadNotificationError
{
    InvalidInput,
    Unauthenticated,
    Forbidden,
    NotFound,
    Conflict,
    ConcurrencyConflict,
    UnexpectedError,
}
