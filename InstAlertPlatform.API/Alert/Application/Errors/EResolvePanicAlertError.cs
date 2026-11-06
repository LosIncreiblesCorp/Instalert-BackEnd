namespace InstAlertPlatform.API.Alert.Application.Errors;
public enum EResolvePanicAlertError
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
