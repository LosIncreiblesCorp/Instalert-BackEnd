namespace InstAlertPlatform.API.Alert.Application.Errors;
public enum EUpdatePanicReportDraftError
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
