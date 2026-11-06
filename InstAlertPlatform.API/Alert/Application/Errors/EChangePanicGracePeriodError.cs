namespace InstAlertPlatform.API.Alert.Application.Errors;
public enum EChangePanicGracePeriodError
{
    InvalidInput,
    Unauthenticated,
    Forbidden,
    NotFound,
    Conflict,
    ConcurrencyConflict,
    UnexpectedError,
    InvalidTransition,
    AlreadyCompleted,
}
