namespace InstAlertPlatform.API.Alert.Application.Errors;
public enum EPreparePanicError
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
