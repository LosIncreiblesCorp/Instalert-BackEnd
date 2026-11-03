namespace InstAlertPlatform.API.Profiles.Application.Errors;
public enum EUpdateUserProfileError
{
    InvalidInput,
    Unauthenticated,
    Forbidden,
    NotFound,
    Conflict,
    ConcurrencyConflict,
    UnexpectedError,
}
