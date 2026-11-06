namespace InstAlertPlatform.API.Alert.Application.Errors;
public enum EConfirmPanicError
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
