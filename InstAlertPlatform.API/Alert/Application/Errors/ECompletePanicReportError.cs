namespace InstAlertPlatform.API.Alert.Application.Errors;
public enum ECompletePanicReportError
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
