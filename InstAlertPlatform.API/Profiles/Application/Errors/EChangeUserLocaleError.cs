namespace InstAlertPlatform.API.Profiles.Application.Errors;
public enum EChangeUserLocaleError
{
    InvalidInput,
    Unauthenticated,
    Forbidden,
    NotFound,
    Conflict,
    ConcurrencyConflict,
    UnexpectedError,
}
