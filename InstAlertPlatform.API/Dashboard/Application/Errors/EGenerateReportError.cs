namespace InstAlertPlatform.API.Dashboard.Application.Errors;
public enum EGenerateReportError
{
    InvalidInput,
    Unauthenticated,
    Forbidden,
    NotFound,
    Conflict,
    ConcurrencyConflict,
    UnexpectedError,
    InvalidTransition,
    PolicyUnavailable,
}
